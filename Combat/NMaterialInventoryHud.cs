using System.Globalization;
using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Godot.NodeAttachments;

namespace STS2_WineFox.Combat
{
    [RegisterNodeAttachment(typeof(NCombatUi), AttachmentId,
        NodeName = NodeName,
        DuplicatePolicy = NodeAttachmentDuplicatePolicy.ReuseExistingByName,
        SetupTiming = NodeAttachmentSetupTiming.AfterAdd)]
    public partial class NMaterialInventoryHud : Control
    {
        public const string AttachmentId = "material_inventory_hud";
        public const string NodeName = "WineFoxMaterialInventoryHud";

        private static readonly Vector2 FallbackHudPosition = new(335f, 630f);
        private static readonly Vector2 HudOffsetFromCreatureStateDisplay = new(-145f, -120f);
        private static readonly Vector2 HudSize = new(100f, 100f);
        private static readonly Vector2 SourceImageSize = new(198f, 190f);
        private static readonly Vector2 IconSize = new(30f, 30f);
        private static readonly Vector2 ScaleFromSource = new(HudSize.X / SourceImageSize.X, HudSize.Y / SourceImageSize.Y);

        private static readonly MaterialSlotDefinition[] SlotDefinitions =
        [
            new(typeof(WoodPower), Const.Paths.WoodPowerIcon, new(62f, 56f)),
            new(typeof(StonePower), Const.Paths.StonePowerIcon, new(132f, 56f)),
            new(typeof(IronPower), Const.Paths.IronPowerIcon, new(62f, 126f)),
            new(typeof(DiamondPower), Const.Paths.DiamondPowerIcon, new(132f, 126f))
        ];

        private readonly List<MaterialInventoryPanel> _panels = [];

        public override void _Ready()
        {
            MouseFilter = MouseFilterEnum.Ignore; // HUD 不接收鼠标输入。
            AnchorLeft = 0f; // 使用绝对定位，不使用水平锚点。
            AnchorTop = 0f; // 使用绝对定位，不使用垂直锚点。
            AnchorRight = 0f; // 固定右侧锚点，便于手动控制尺寸。
            AnchorBottom = 0f; // 固定底部锚点，便于手动控制尺寸。
            Position = FallbackHudPosition; // 玩家节点可用前的安全默认位置。
            Size = Vector2.Zero; // 根节点只负责承载多个玩家的物品栏面板。
            ZIndex = 0; // 与普通战斗 HUD 保持同层级。
            Modulate = Modulate with { A = 0.9f }; // HUD 整体不透明度；底图单独设置透明度。
            Visible = false; // 绑定酒狐玩家前保持隐藏。
        }

        public override void _Process(double delta)
        {
            if (_panels.Count == 0)
                return;

            foreach (var panel in _panels)
                panel.Refresh();

            Visible = _panels.Any(panel => panel.Visible);
        }

        public void BindPlayers(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                if (_panels.Any(panel => ReferenceEquals(panel.Player, player))) continue;

                var panel = new MaterialInventoryPanel(player);
                _panels.Add(panel);
                panel.Refresh();
            }

            Visible = _panels.Count > 0;
        }

        public void Unbind()
        {
            foreach (var panel in _panels)
                panel.QueueFree();

            _panels.Clear();
            Visible = false;
        }

        private static string FormatAmount(decimal amount)
        {
            return amount == decimal.Truncate(amount)
                ? amount.ToString("0", CultureInfo.InvariantCulture)
                : amount.ToString("0.#", CultureInfo.InvariantCulture);
        }

        private static Vector2 ScaleSourcePoint(Vector2 point)
        {
            return new(point.X * ScaleFromSource.X, point.Y * ScaleFromSource.Y);
        }

        private sealed record MaterialSlotDefinition(Type PowerType, string IconPath, Vector2 SourceCenter);

        private partial class MaterialInventoryPanel : Control
        {
            private readonly MaterialSlot[] _slots = new MaterialSlot[SlotDefinitions.Length];
            private readonly Callable _focusCallable;
            private readonly Callable _unfocusCallable;
            private NCreature? _creatureNode;
            private bool _isHoverConnected;
            private bool _isHovered;

            public MaterialInventoryPanel(Player player)
            {
                Player = player;
                _focusCallable = Callable.From(OnFocus);
                _unfocusCallable = Callable.From(OnUnfocus);
                MouseFilter = MouseFilterEnum.Ignore;
                Size = HudSize;
                CustomMinimumSize = HudSize;
                ZAsRelative = true;
                ZIndex = 1;
                Visible = false;

                var background = new TextureRect // 物品栏底图。
                {
                    MouseFilter = MouseFilterEnum.Ignore, // 底图仅作装饰，不接收鼠标输入。
                    Texture = ResourceLoader.Load<Texture2D>(Const.Paths.MaterialInventoryBox), // 2x2 物品栏底图贴图。
                    ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, // 由 Size 控制实际显示尺寸。
                    StretchMode = TextureRect.StretchModeEnum.Scale, // 将原图缩放到 HudSize。
                    Position = Vector2.Zero, // 从 HUD 原点开始铺满。
                    Size = HudSize, // 与 HUD 边界一致。
                    Modulate = Colors.White with { A = 0.6f } // 仅将物品栏底图设为 60% 不透明度。
                };
                AddChild(background);

                for (var i = 0; i < SlotDefinitions.Length; i++)
                {
                    var slot = new MaterialSlot(SlotDefinitions[i]);
                    _slots[i] = slot;
                    AddChild(slot);
                }
            }

            public Player Player { get; }

            public override void _ExitTree()
            {
                DisconnectHoverSignals();
                base._ExitTree();
            }

            public void Refresh()
            {
                if (Player.Creature == null)
                {
                    Visible = false;
                    return;
                }

                UpdateCreatureAttachment();
                UpdateHudPosition();
                Visible = ShouldStayVisible() && ShouldBeVisibleForFocus();
                if (!Visible) return;

                for (var i = 0; i < _slots.Length; i++)
                    _slots[i].SetAmount(GetMaterialAmount(SlotDefinitions[i].PowerType));
            }

            private decimal GetMaterialAmount(Type powerType)
            {
                return Player.Creature.Powers.FirstOrDefault(power => power.GetType() == powerType)?.Amount ?? 0m;
            }

            private bool ShouldStayVisible()
            {
                return Player.Character is WineFox ||
                       Player.Character.Id.Entry.Contains("winefox", StringComparison.OrdinalIgnoreCase) ||
                       SlotDefinitions.Any(slot => GetMaterialAmount(slot.PowerType) > 0m);
            }

            private void UpdateCreatureAttachment()
            {
                var creatureNode = Player.Creature.GetCreatureNode();
                if (creatureNode == null || !GodotObject.IsInstanceValid(creatureNode)) return;
                if (ReferenceEquals(_creatureNode, creatureNode) && GetParent() == creatureNode) return;

                DisconnectHoverSignals();
                GetParent()?.RemoveChild(this);
                creatureNode.AddChild(this);
                _creatureNode = creatureNode;
                ConnectHoverSignals();
            }

            private void UpdateHudPosition()
            {
                var creatureNode = _creatureNode;
                if (creatureNode == null || !GodotObject.IsInstanceValid(creatureNode))
                {
                    Position = FallbackHudPosition;
                    return;
                }

                var stateDisplay = creatureNode.GetNodeOrNull<Control>("%HealthBar")
                                   ?? creatureNode.GetNodeOrNull<Control>("HealthBar")
                                   ?? creatureNode;
                GlobalPosition = stateDisplay.GlobalPosition + HudOffsetFromCreatureStateDisplay;
            }

            private bool ShouldBeVisibleForFocus()
            {
                if (LocalContext.IsMe(Player.Creature)) return true;
                return _isHovered || _creatureNode?.IsFocused == true;
            }

            private void ConnectHoverSignals()
            {
                if (_creatureNode?.Hitbox == null || _isHoverConnected) return;

                _creatureNode.Hitbox.Connect(Control.SignalName.MouseEntered, _focusCallable);
                _creatureNode.Hitbox.Connect(Control.SignalName.MouseExited, _unfocusCallable);
                _creatureNode.Hitbox.Connect(Control.SignalName.FocusEntered, _focusCallable);
                _creatureNode.Hitbox.Connect(Control.SignalName.FocusExited, _unfocusCallable);
                _isHoverConnected = true;
            }

            private void DisconnectHoverSignals()
            {
                if (_creatureNode?.Hitbox == null || !_isHoverConnected) return;

                _creatureNode.Hitbox.Disconnect(Control.SignalName.MouseEntered, _focusCallable);
                _creatureNode.Hitbox.Disconnect(Control.SignalName.MouseExited, _unfocusCallable);
                _creatureNode.Hitbox.Disconnect(Control.SignalName.FocusEntered, _focusCallable);
                _creatureNode.Hitbox.Disconnect(Control.SignalName.FocusExited, _unfocusCallable);
                _isHoverConnected = false;
                _isHovered = false;
            }

            private void OnFocus()
            {
                _isHovered = true;
            }

            private void OnUnfocus()
            {
                _isHovered = false;
            }
        }

        private partial class MaterialSlot : Control
        {
            private readonly Label _amountLabel;

            public MaterialSlot(MaterialSlotDefinition definition)
            {
                MouseFilter = MouseFilterEnum.Ignore;
                Position = Vector2.Zero;
                Size = HudSize;
                CustomMinimumSize = HudSize;

                var center = ScaleSourcePoint(definition.SourceCenter);
                var icon = new TextureRect
                {
                    MouseFilter = MouseFilterEnum.Ignore,
                    Texture = ResourceLoader.Load<Texture2D>(definition.IconPath),
                    ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                    StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                    Position = center - IconSize * 0.5f,
                    Size = IconSize
                };
                AddChild(icon);

                _amountLabel = new()
                {
                    MouseFilter = MouseFilterEnum.Ignore,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Position = center + new Vector2(-2f, 1f),
                    Size = new(16f, 16f)
                };
                _amountLabel.AddThemeFontSizeOverride("font_size", 16);
                _amountLabel.AddThemeColorOverride("font_color", Colors.White);
                _amountLabel.AddThemeColorOverride("font_outline_color", Colors.Black);
                _amountLabel.AddThemeConstantOverride("outline_size", 3);
                AddChild(_amountLabel);
            }

            public void SetAmount(decimal amount)
            {
                _amountLabel.Text = FormatAmount(amount);
            }
        }
    }
}
