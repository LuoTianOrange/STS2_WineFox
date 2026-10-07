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

        private const float BarGapAboveVisual = 12f;

        private static Texture2D? _outline;

        private MagicWineFoxSpellSlotPower? _power;
        private Player? _boundPlayer;
        private NCreature? _creatureNode;

        /// <summary>
        ///     角色可视节点的缓存：只在角色节点更换时遍历一次树（缓存为空时有限次重试，
        ///     因为角色贴图可能比节点晚若干帧才创建），避免每帧递归子节点。
        /// </summary>
        private readonly List<CanvasItem> _visuals = [];

        private int _visualRetryFrames;

        /// <summary>上一次重绘时的槽位内容，用于**无分配**地判断是否需要重绘。</summary>
        private readonly List<CardModel?> _lastSlots = [];

        private int _lastCapacity = -1;
        private int _lastLoaded = -1;
        private bool _forceRedraw = true;

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

            // 用「当前战斗」的实时玩家替换绑定时的缓存对象。
            // 存档 / 读档会重建 Player、Creature、Power 实例；若继续复用旧对象，
            // 预览条会一直读旧 Power 的槽位列表，表现为「读档后槽位保留了之前打出的法术」。
            var live = ResolveLivePlayer();
            if (live != null && !ReferenceEquals(live, _boundPlayer))
            {
                _boundPlayer = live;
                _forceRedraw = true;
            }

            // Power 实例更换（读档 / 换战斗）同样必须强制重绘：
            // 只比较容量与槽内卡牌时，两个不同实例可能恰好相同，此时会跳过重绘、
            // UI 停留在读档前的画面。
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

        /// <summary>
        ///     从**当前战斗**取本地玩家。
        ///     <para>
        ///         存档 / 读档会重建 Player 与 Power 实例，而本节点是跨场景复用的附件节点，
        ///         绑定时的 <c>_boundPlayer</c> 会变成指向旧对象的悬空引用。
        ///         这里每帧用实时对象覆盖它，从而不会显示存档前的槽位内容。
        ///     </para>
        /// </summary>
        private static Player? ResolveLivePlayer()
        {
            var state = CombatManager.Instance?.DebugOnlyGetState();
            return state == null ? null : LocalContext.GetMe(state);
        }

        /// <summary>绑定要显示其槽位的玩家；传 null 表示解绑并隐藏。</summary>
        public void Bind(Player? player)
        {
            _boundPlayer = player;
            _power = player == null ? null : MagicWineFoxSpellCmd.GetPower(player);
            _creatureNode = null;
            _visuals.Clear();
            _forceRedraw = true;

            if (player == null)
                Visible = false;
        }

        public void Unbind()
        {
            Bind(null);
        }

        /// <summary>
        ///     记录玩家角色节点，仅用于读取其屏幕坐标。
        ///     <para>
        ///         刻意**不**把本节点挂到角色节点下：那会与 RitsuLib 的节点附件系统冲突——
        ///         附件系统仍认为本节点归属于 NCombatUi，随后会尝试重新挂载并报
        ///         「child already belongs to NCreature」。因此这里保持 NCombatUi 子节点身份，
        ///         改为每帧按角色的全局坐标定位，视觉上同样跟随角色。
        ///     </para>
        /// </summary>
        private bool ResolveCreatureNode()
        {
            var creatureNode = _boundPlayer?.Creature?.GetCreatureNode();
            if (creatureNode == null || !GodotObject.IsInstanceValid(creatureNode))
                return false;

            if (!ReferenceEquals(creatureNode, _creatureNode))
            {
                _creatureNode = creatureNode;
                _visualRetryFrames = 0;
                CacheVisuals();
            }
            else if (_visuals.Count == 0 && _visualRetryFrames < 120)
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

        /// <summary><see cref="CanvasItem" /> 本身没有 <c>GlobalPosition</c>，按具体类型取。</summary>
        private static Vector2 VisualPosition(CanvasItem visual)
        {
            return visual switch
            {
                Control control => control.GlobalPosition,
                Node2D node => node.GlobalPosition,
                _ => Vector2.Zero,
            };
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

        /// <summary>整排水平居中到角色头顶上方（屏幕坐标）。</summary>
        private void ApplyLayout()
        {
            var anchor = ResolveAnchor();
            var scale = Math.Max(1f, ResolveCreatureScale());
            var scaledY = anchor.Y + BarOffsetFromAnchor.Y * scale;
            var hitboxY = ResolveHitboxTop() - BarGapAboveVisual;
            var visualY = ResolveVisualTop() - BarGapAboveVisual;

            Position = new Vector2(
                anchor.X - ResolveBarWidth() * 0.5f,
                Math.Min(Math.Min(scaledY, hitboxY), visualY));
        }

        /// <summary>
        ///     角色命中框顶边——游戏自身的 <c>NCreature.GetTopOfHitbox</c>，
        ///     随角色缩放与移动变化，是最可靠的「头顶」来源。
        /// </summary>
        private float ResolveHitboxTop()
        {
            if (_creatureNode == null || !GodotObject.IsInstanceValid(_creatureNode))
                return float.MaxValue;

            return _creatureNode.GetTopOfHitbox().Y;
        }

        /// <summary>
        ///     角色当前的放大倍数。取角色节点自身与其可视子节点中的最大值——
        ///     【大蘑菇】这类效果会把角色放大（1.5 倍），基准偏移必须同比放大才不会挡住。
        /// </summary>
        private float ResolveCreatureScale()
        {
            var scale = Math.Max(1f, NodeScale(_creatureNode));

            // 角色是 Spine 动画：承载缩放的往往是 Visuals / Body（不是贴图子节点），
            // 这两个必须显式读，否则【大蘑菇】那类放大读不到（会一直当成 1 倍）。
            if (_creatureNode != null)
            {
                scale = Math.Max(scale, NodeScale(_creatureNode.Visuals));
                scale = Math.Max(scale, NodeScale(_creatureNode.Body));
            }

            foreach (var visual in _visuals)
                scale = Math.Max(scale, NodeScale(visual));

            return scale;
        }

        private static float NodeScale(Node? node)
        {
            if (node == null || !GodotObject.IsInstanceValid(node))
                return 1f;

            return node switch
            {
                Node2D node2D => Math.Abs(node2D.GlobalScale.Y),
                Control control => Math.Abs(control.Scale.Y),
                _ => 1f,
            };
        }

        /// <summary>
        ///     角色可视范围的最上沿（y 越小越靠上）。按贴图实际尺寸与缩放计算，
        ///     因此角色被放大（如【大蘑菇】放大 1.5 倍）、缩小或移动时都能跟随。
        /// </summary>
        private float ResolveVisualTop()
        {
            var top = float.MaxValue;

            foreach (var visual in _visuals)
            {
                if (!GodotObject.IsInstanceValid(visual) || !visual.Visible)
                    continue;

                if (visual is Control control)
                {
                    var rect = control.GetGlobalRect();
                    if (rect.Size.Y > 1f)
                        top = Math.Min(top, rect.Position.Y);

                    continue;
                }

                float? textureHeight = visual switch
                {
                    Sprite2D sprite when sprite.Texture != null
                        => sprite.Texture.GetHeight(),
                    AnimatedSprite2D animated when animated.SpriteFrames != null
                        => animated.SpriteFrames.GetFrameTexture(animated.Animation, animated.Frame)?.GetHeight(),
                    _ => null,
                };

                if (textureHeight is not { } height)
                    continue;

                var centered = visual switch
                {
                    Sprite2D sprite => sprite.Centered,
                    AnimatedSprite2D animated => animated.Centered,
                    _ => false,
                };

                var half = centered ? height * 0.5f : 0f;
                var scale = visual is Node2D node2D ? Math.Abs(node2D.GlobalScale.Y) : 1f;
                top = Math.Min(top, VisualPosition(visual).Y - half * scale);
            }

            return top == float.MaxValue ? _creatureNode?.GlobalPosition.Y ?? 0f : top;
        }

        /// <summary>锚点取血条的屏幕坐标；取不到则用角色节点自身。</summary>
        private Vector2 ResolveAnchor()
        {
            var creatureNode = _creatureNode;
            if (creatureNode == null || !GodotObject.IsInstanceValid(creatureNode))
                return Vector2.Zero;

            var stateDisplay = creatureNode.GetNodeOrNull<Control>("%HealthBar")
                               ?? creatureNode.GetNodeOrNull<Control>("HealthBar");

            return stateDisplay?.GlobalPosition ?? creatureNode.GlobalPosition;
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
        ///     用引用比较与复用的列表，**不产生任何分配**——本方法每帧都会调用，
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

        /// <summary>
        ///     悬停槽位时显示该槽位法术的卡牌提示（与 Noita 的做法一致）。
        ///     <para>
        ///         显示前刷新一次法杖内卡牌的数值——卡面显示的是「Power + 法术修正符」
        ///         合成后的结果，因此提示里的数字与手牌一致地受加成影响。
        ///     </para>
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
