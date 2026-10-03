using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Godot.NodeAttachments;

namespace STS2_WineFox.Nodes
{
    /// <summary>
    ///     法杖槽位预览条。
    ///     <para>
    ///         渲染一排方形槽位，按装填顺序显示 <see cref="MagicWineFoxSpellSlotPower" />
    ///         中已装填法术的图标；空槽显示占位图。
    ///     </para>
    ///     <para>
    ///         视觉参考 Noita 的「法杖横排」：固定间距、整排水平居中。
    ///         位置以角色节点为锚——挂到 <c>Creature.GetCreatureNode()</c> 后取血条上方，
    ///         因此会跟随角色（含多人模式下的各自站位），而不是固定在屏幕某处。
    ///         它只读取 <c>Power</c> 的槽位状态，不参与任何结算。
    ///     </para>
    /// </summary>
    [RegisterNodeAttachment(typeof(NCombatUi), AttachmentId,
        NodeName = NodeName,
        DuplicatePolicy = NodeAttachmentDuplicatePolicy.ReuseExistingByName,
        SetupTiming = NodeAttachmentSetupTiming.AfterAdd)]
    public partial class NSpellSlotBar : Control
    {
        public const string AttachmentId = "winefox_spell_slot_bar";
        public const string NodeName = "WineFoxSpellSlotBar";

        /// <summary>单个槽位的边长与间距（像素）。</summary>
        private const float SlotSpacing = 72f;

        /// <summary>边框铺满槽位的比例。</summary>
        private const float SlotOutlineFill = 1f;

        /// <summary>法术图标占槽位的比例。</summary>
        private const float IconFill = 0.82f;

        /// <summary>
        ///     整排相对锚点的偏移：垂直抬到角色头顶上方。
        ///     实测锚点在当地坐标 y≈-13（血条底边）。
        /// </summary>
        private static readonly Vector2 BarOffsetFromAnchor = new(0f, -345f);

        private static Texture2D? _outline;

        private MagicWineFoxSpellSlotPower? _power;
        private Player? _boundPlayer;
        private NCreature? _creatureNode;
        private string _signature = string.Empty;

        public override void _Ready()
        {
            MouseFilter = MouseFilterEnum.Ignore; // HUD 不接收鼠标输入。
            Size = Vector2.Zero; // 根节点只承载整排槽位。
            ZIndex = 0;
            Visible = false; // 绑定成功前保持隐藏。
        }

        public override void _Process(double delta)
        {
            if (_boundPlayer == null)
                return;

            // Power 由遗物在战斗回合开始时创建，可能晚于本节点绑定，因此惰性解析。
            _power ??= MagicWineFoxSpellCmd.GetPower(_boundPlayer);

            if (!AttachToCreature())
                return;

            if (_power == null || _power.SlotCapacity <= 0)
            {
                Visible = false;
                return;
            }

            Visible = true;
            ApplyLayout();
            Refresh();
        }

        /// <summary>绑定要显示其槽位的玩家；传 null 表示解绑并隐藏。</summary>
        public void Bind(Player? player)
        {
            _boundPlayer = player;
            _power = player == null ? null : MagicWineFoxSpellCmd.GetPower(player);
            _creatureNode = null; // 重新挂到新的角色节点上。
            _signature = string.Empty;

            if (player == null)
                Visible = false;
        }

        public void Unbind()
        {
            Bind(null);
        }

        /// <summary>
        ///     把本节点挂到玩家角色节点下，使其随角色移动。
        ///     参考 <c>NMaterialInventoryHud.UpdateCreatureAttachment</c> 的做法。
        /// </summary>
        private bool AttachToCreature()
        {
            var creatureNode = _boundPlayer?.Creature?.GetCreatureNode();
            if (creatureNode == null || !GodotObject.IsInstanceValid(creatureNode))
                return false;

            if (ReferenceEquals(_creatureNode, creatureNode) && GetParent() == creatureNode)
                return true;

            GetParent()?.RemoveChild(this);
            creatureNode.AddChild(this);
            _creatureNode = creatureNode;
            return true;
        }

        /// <summary>
        ///     整排水平居中到锚点上方。
        ///     本节点已是角色节点的子节点，因此这里用的是相对角色的**局部坐标**。
        /// </summary>
        private void ApplyLayout()
        {
            var width = ResolveBarWidth();
            var anchorY = ResolveAnchorY();

            Position = new Vector2(-width * 0.5f, anchorY + BarOffsetFromAnchor.Y);

            LogAnchorOnce(anchorY, width);
        }

        /// <summary>锚点取血条底边的局部 Y；取不到则回落到 0。</summary>
        private float ResolveAnchorY()
        {
            var creatureNode = _creatureNode;
            if (creatureNode == null || !GodotObject.IsInstanceValid(creatureNode))
                return 0f;

            var stateDisplay = creatureNode.GetNodeOrNull<Control>("%HealthBar")
                               ?? creatureNode.GetNodeOrNull<Control>("HealthBar");

            return stateDisplay == null ? 0f : stateDisplay.Position.Y + stateDisplay.Size.Y;
        }

        private bool _anchorLogged;

        /// <summary>
        ///     临时诊断：同时打印「局部坐标」与「屏幕全局坐标」，用于精确校准偏移量。
        ///     只有拿到屏幕坐标才能确定换算比例，避免反复试错。定位完成后删除。
        /// </summary>
        private void LogAnchorOnce(float anchorY, float width)
        {
            if (_anchorLogged)
                return;

            _anchorLogged = true;

            var creature = _creatureNode;
            var healthBar = creature?.GetNodeOrNull<Control>("%HealthBar")
                            ?? creature?.GetNodeOrNull<Control>("HealthBar");

            Main.Logger.Info(
                $"[SlotBar] viewport={GetViewportRect().Size} " +
                $"creatureGlobal={creature?.GlobalPosition.ToString() ?? "?"} " +
                $"健康条Global={healthBar?.GlobalPosition.ToString() ?? "?"} " +
                $"条LocalPos={Position} 条GlobalPos={GlobalPosition} " +
                $"锚点Y={anchorY} 排宽={width} 容量={_power?.SlotCapacity}");
        }

        private float ResolveBarWidth()
        {
            var count = Math.Max(1, _power?.SlotCapacity ?? 1);
            return SlotSpacing * count;
        }

        private void Refresh()
        {
            var signature = BuildSignature();
            if (signature == _signature)
                return;

            _signature = signature;
            Rebuild();
        }

        /// <summary>容量 / 已装填数 / 每张卡的身份——任一变化都需重绘。</summary>
        private string BuildSignature()
        {
            if (_power == null)
                return "none";

            var parts = new List<string>
            {
                _power.SlotCapacity.ToString(),
                _power.LoadedCount.ToString(),
            };

            foreach (var slot in _power.Slots)
                parts.Add(slot == null ? "-" : slot.Card.GetHashCode().ToString());

            return string.Join('|', parts);
        }

        private void Rebuild()
        {
            foreach (var child in GetChildren())
            {
                RemoveChild(child);
                child.QueueFree();
            }

            if (_power == null)
                return;

            var capacity = _power.SlotCapacity;
            if (capacity <= 0)
                return;

            var slots = _power.Slots;
            for (var i = 0; i < capacity; i++)
            {
                var snapshot = i < slots.Count ? slots[i] : null;
                AddChild(BuildSlot(snapshot, i));
            }
        }

        private static Control BuildSlot(MagicWineFoxSpellSlotSnapshot? snapshot, int index)
        {
            var side = new Vector2(SlotSpacing, SlotSpacing);
            var box = new Control
            {
                // 只用 Size 定位排布，不用 CustomMinimumSize——两者混用会让实际尺寸被最小尺寸顶掉，
                // 出现「间距按 SlotSpacing、尺寸却按别的值」的错位。
                Position = new Vector2(index * SlotSpacing, 0f),
                Size = side,
                // 需要接收鼠标才能弹悬停提示；其余区域仍穿透。
                MouseFilter = MouseFilterEnum.Stop,
            };

            // 绘制顺序 = 子节点顺序：先边框、后图标，图标才能显示在边框之上。
            box.AddChild(BuildLayer(_outline ??= LoadTexture(Const.Paths.SpellSlotOutline),
                SlotOutlineFill, Colors.White));

            // 空槽不贴图标层，只剩边框——避免显示任何占位图。
            // 图标一律用中性 Modulate，不做偏色：颜色相乘会污染图标本身的配色，
            // 修正器与普通法术已由各自的专属图标区分。
            var icon = ResolveIcon(snapshot);
            if (icon != null)
                box.AddChild(BuildLayer(icon, IconFill, Colors.White));

            if (snapshot?.Card != null)
                AttachHoverTip(box, snapshot.Card);

            return box;
        }

        /// <summary>悬停槽位时显示该槽位法术的卡牌提示（与 Noita 的做法一致）。</summary>
        private static void AttachHoverTip(Control slot, CardModel card)
        {
            slot.MouseEntered += () =>
            {
                if (!GodotObject.IsInstanceValid(slot))
                    return;

                NHoverTipSet.Remove(slot);
                NHoverTipSet.CreateAndShow(slot, [new CardHoverTip(card)],
                    HoverTip.GetHoverTipAlignment(slot))?.SetFollowOwner();
            };

            slot.MouseExited += () =>
            {
                if (GodotObject.IsInstanceValid(slot))
                    NHoverTipSet.Remove(slot);
            };
        }

        /// <summary>
        ///     构造槽位里的一层贴图，并让它铺满槽位。
        ///     <para>
        ///         注意两点：
        ///         一是必须先赋 <c>Texture</c> 再设 <c>Size</c>，否则尺寸会被纹理的最小尺寸覆盖；
        ///         二是不要用 <c>Scale</c>——Godot 的缩放绕控件左上角进行，会让贴图偏移到左上角。
        ///         因此这里靠固定的槽位尺寸 + <c>KeepAspectCentered</c> 来完成居中与留白。
        ///     </para>
        /// </summary>
        private static TextureRect BuildLayer(Texture2D? texture, float fillRatio, Color modulate)
        {
            var box = new Vector2(SlotSpacing, SlotSpacing);
            var side = SlotSpacing * fillRatio;
            var inset = (SlotSpacing - side) * 0.5f;

            var rect = new TextureRect
            {
                Texture = texture,
                Modulate = modulate,
                MouseFilter = MouseFilterEnum.Ignore,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            };

            rect.Position = new Vector2(inset, inset);
            rect.Size = new Vector2(side, side);
            return rect;
        }

        /// <summary>
        ///     解析槽位图标。
        ///     空槽与「卡牌未实现 <see cref="IMagicWineFoxSpellIconProvider" />」都返回 null，
        ///     此时该层不贴图，槽位只剩外框。
        /// </summary>
        private static Texture2D? ResolveIcon(MagicWineFoxSpellSlotSnapshot? snapshot)
        {
            if (snapshot?.Card is not IMagicWineFoxSpellIconProvider provider)
                return null;

            return LoadTexture(provider.SpellIconPath);
        }

        private static Texture2D? LoadTexture(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            return ResourceLoader.Exists(path) ? ResourceLoader.Load<Texture2D>(path) : null;
        }
    }
}
