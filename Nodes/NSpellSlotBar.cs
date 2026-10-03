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

        /// <summary>绑定要显示其槽位的玩家；传 null 表示解绑并隐藏。</summary>
        public void Bind(Player? player)
        {
            _boundPlayer = player;
            _power = player == null ? null : MagicWineFoxSpellCmd.GetPower(player);
            _creatureNode = null;
            _signature = string.Empty;

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

            _creatureNode = creatureNode;
            return true;
        }

        /// <summary>整排水平居中到角色头顶上方（屏幕坐标）。</summary>
        private void ApplyLayout()
        {
            var anchor = ResolveAnchor();
            Position = new Vector2(
                anchor.X - ResolveBarWidth() * 0.5f,
                anchor.Y + BarOffsetFromAnchor.Y);
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
