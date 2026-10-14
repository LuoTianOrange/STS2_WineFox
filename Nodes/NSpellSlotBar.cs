using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
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
    /// <para>
    ///         渲染一排方形槽位，按装填顺序显示 <see cref="MagicWineFoxSpellSlotPower" />
    ///         中已装填法术的图标；空槽显示占位图。
    /// </para>
    /// <para>
    /// 视觉参考 Noita 的法杖横排：固定间距、整排水平居中。
    ///         位置以角色节点为锚——挂到 <c>Creature.GetCreatureNode()</c> 后取血条上方，
    ///         因此会跟随角色（含多人模式下的各自站位），而不是固定在屏幕某处。
    ///         它只读取 <c>Power</c> 的槽位状态，不参与任何结算。
    /// </para>
    /// </summary>
    [RegisterNodeAttachment(typeof(NCreature), AttachmentId,
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
        private bool _isLocal;

        /// <summary>
        ///     角色可视节点的缓存：只在角色节点更换时遍历一次树（缓存为空时有限次重试，
        ///     因为角色贴图可能比节点晚若干帧才创建），避免每帧递归子节点。
        /// </summary>
        private readonly List<CanvasItem> _visuals = [];

        private int _visualRetryFrames;

        /// <summary>
        /// 血条节点缓存。
        /// </summary>

        /// <summary>
        ///     角色可视范围的缓存值（缩放系数与最上沿）。
        /// </summary>
        private float? _cachedScale;


        /// <summary>刷新可视范围缓存的帧间隔（60fps 下约 0.1 秒）。</summary>
        private const int VisualCacheRefreshFrames = 6;

        private int _visualRefreshCountdown;

        /// <summary>上一次重绘时的槽位内容,用于无分配地判断是否需要重绘。</summary>
        private readonly List<CardModel?> _lastSlots = [];

        private int _lastCapacity = -1;
        private int _lastLoaded = -1;
        private bool _forceRedraw = true;

        private Tween? _hoverTween;

        /// <summary>队友的槽位默认压暗，鼠标悬浮到该角色身上时恢复原色。</summary>
        private static readonly Color RemoteModulate = new(0.55f, 0.55f, 0.6f);

        private void OnCreatureFocus()
        {
            TweenModulateTo(Colors.White);
        }

        private void OnCreatureUnfocus()
        {
            TweenModulateTo(_isLocal ? Colors.White : RemoteModulate);
        }

        /// <summary>过渡到目标颜色，时长与血条的悬浮过渡一致。</summary>
        private void TweenModulateTo(Color target)
        {
            _hoverTween?.Kill();
            _hoverTween = CreateTween().SetParallel();
            _hoverTween.TweenProperty(this, "modulate", target, 0.2)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Sine);
        }

        public override void _Ready()
        {
            MouseFilter = MouseFilterEnum.Ignore;
            Size = Vector2.Zero;
            Visible = false;

            var parent = GetParent();

            if (parent is not NCreature creature || creature.Entity.Player is not { } player)
            {
                SetProcess(false);
                return;
            }

            _creatureNode = creature;
            _boundPlayer = player;
            _isLocal = LocalContext.IsMe(player);
            _forceRedraw = true;

            Modulate = _isLocal ? Colors.White : RemoteModulate;

            if (creature.Hitbox is { } hitbox)
            {
                hitbox.Connect(Control.SignalName.MouseEntered, Callable.From(OnCreatureFocus));
                hitbox.Connect(Control.SignalName.MouseExited, Callable.From(OnCreatureUnfocus));
            }

        }

        public override void _Process(double delta)
        {
            if (_boundPlayer == null)
                return;

            var power = MagicWineFoxSpellCmd.GetPower(_boundPlayer);
            if (!ReferenceEquals(power, _power))
            {
                _power = power;
                _forceRedraw = true;
            }

            if (!ResolveCreatureNode())
            {
                Visible = false;
                return;
            }

            if (_power == null || _power.SlotCapacity <= 0)
            {
                Visible = false;
                return;
            }

            Visible = true;
            ApplyLayout();
            Refresh();
        }

        private bool ResolveCreatureNode()
        {
            if (_creatureNode == null || !GodotObject.IsInstanceValid(_creatureNode))
                return false;

            if (_visuals.Count == 0 && _visualRetryFrames < 120)
            {
                _visualRetryFrames++;
                CacheVisuals();
            }

            return true;
        }

        /// <summary>遍历一次角色子节点，缓存可能代表角色本体的可视节点。</summary>
        private void CacheVisuals()
        {
            _visuals.Clear();

            if (_creatureNode != null)
                CollectVisuals(_creatureNode, _creatureNode);
        }


        private void CollectVisuals(Node? node, Node root)
        {
            if (node == null || !GodotObject.IsInstanceValid(node))
                return;

            if (!ReferenceEquals(node, root) && node is CanvasItem canvasItem)
                _visuals.Add(canvasItem);

            foreach (var child in node.GetChildren())
                CollectVisuals(child, root);
        }

        /// <summary>
        ///     整排水平居中到角色头顶上方。
        /// <para>
        ///         本节点是角色节点的子节点，因此全部使用本地坐标：锚点偏移按角色缩放同比放大，
        ///         不做任何全局坐标换算，避免与父节点变换叠加。
        ///         </para>
        /// </summary>
        private void ApplyLayout()
        {
            if (_cachedScale == null || _visualRefreshCountdown <= 0)
            {
                _cachedScale = Math.Min(ResolveVisualScale(), 1.25f);
                _visualRefreshCountdown = VisualCacheRefreshFrames;
            }
            else
            {
                _visualRefreshCountdown--;
            }

            Position = new Vector2(
                -ResolveBarWidth() * 0.5f,
                BarOffsetFromAnchor.Y * _cachedScale.Value);
        }

        /// <summary>角色可视缩放。只取角色节点与其可视根节点，与充能球的取法一致。</summary>
        private float ResolveVisualScale()
        {
            if (_creatureNode == null || !GodotObject.IsInstanceValid(_creatureNode))
                return 1f;

            var scale = Math.Max(NodeScale(_creatureNode), NodeScale(_creatureNode.Visuals));

            return Math.Max(1f, scale);
        }

        private static float NodeScale(Node? node)
        {
            if (node == null || !GodotObject.IsInstanceValid(node))
                return 1f;

            return node switch
            {
                Node2D node2D => Math.Abs(node2D.Scale.Y),
                Control control => Math.Abs(control.Scale.Y),
                _ => 1f,
            };
        }




        private float ResolveBarWidth()
        {
            var count = Math.Max(1, _power?.SlotCapacity ?? 1);
            return SlotSpacing * count;
        }

        private void Refresh()
        {
            if (!HasContentChanged())
                return;

            Rebuild();
        }

        /// <summary>
        ///     槽位内容是否变化（容量 / 已装填数 / 每张卡的实例）。
        /// 用引用比较与复用的列表，不产生任何分配——本方法每帧都会调用，
        ///     原先的字符串签名会持续制造垃圾。
        /// </summary>
        private bool HasContentChanged()
        {
            if (_power == null)
                return _lastCapacity != -1;

            var slots = _power.Slots;
            var changed = _forceRedraw
                          || _lastCapacity != _power.SlotCapacity
                          || _lastLoaded != _power.LoadedCount
                          || _lastSlots.Count != slots.Count;

            if (!changed)
            {
                for (var i = 0; i < slots.Count; i++)
                {
                    if (ReferenceEquals(_lastSlots[i], slots[i]?.Card))
                        continue;

                    changed = true;
                    break;
                }
            }

            if (!changed)
                return false;

            _forceRedraw = false;
            _lastCapacity = _power.SlotCapacity;
            _lastLoaded = _power.LoadedCount;
            _lastSlots.Clear();

            foreach (var slot in slots)
                _lastSlots.Add(slot?.Card);

            return true;
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
                // 出现间距按 SlotSpacing、尺寸却按别的值的错位。
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

        /// <summary>
        ///     悬停槽位时显示该槽位法术的卡牌提示（与 Noita 的做法一致）。
        /// <para>
        /// 显示前刷新一次法杖内卡牌的数值——卡面显示的是Power + 法术修正符
        ///         合成后的结果，因此提示里的数字与手牌一致地受加成影响。
        /// </para>
        /// </summary>
        private static void AttachHoverTip(Control slot, CardModel card)
        {
            slot.MouseEntered += () =>
            {
                if (!GodotObject.IsInstanceValid(slot))
                    return;

                if (card.Owner != null)
                    MagicWineFoxSpellCmd.RefreshWandCardValues(MagicWineFoxSpellCmd.GetPower(card.Owner));

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
        /// 空槽与卡牌未实现 <see cref="IMagicWineFoxSpellIconProvider" />都返回 null，
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
