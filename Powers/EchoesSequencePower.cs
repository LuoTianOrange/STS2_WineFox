using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     序列回响 —— 参考 Noita 的「始终释放」。
    ///     <para>
    ///         施加时：**法术槽 +1**，并进入「等待绑定下一张法术卡」状态。
    ///     </para>
    ///     <para>
    ///         下一张打出的可装填法术会被**消耗**（进入消耗堆），同时照常装填，
    ///         并被绑定为本能力始终释放的法术。此后每次法杖清空（释放），
    ///         该法术都会自动装回**第一个槽位**——因此它永远排在序列最前，吃不到任何修正符。
    ///     </para>
    /// </summary>
    [RegisterPower]
    public class EchoesSequencePower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.EchoesSequencePowerIcon);

        private CardModel? _echoSpell;
        private bool _awaitingSpell;

        /// <summary>被绑定的那张法术卡的**实例**，用于结算后把它搬进消耗堆。</summary>
        private CardModel? _boundCardInstance;

        /// <summary>是否仍在等待绑定下一张法术卡。</summary>
        public bool IsAwaitingSpell => _awaitingSpell;

        /// <summary>已绑定的「始终释放」法术模板。</summary>
        public CardModel? EchoSpell => _echoSpell;

        /// <summary>
        ///     填充 <c>{BoundSpellClause}</c> 占位符——**未绑定时填空字符串**，
        ///     因此卡面不会出现「当前绑定：尚未绑定」这种赘述。
        ///     <para>
        ///         由 <c>EchoesSequenceSmartDescriptionPatch</c> 调用——<c>PowerModel.SmartDescription</c>
        ///         不是 virtual，无法直接覆写，只能像 RitsuLib 那样用 Harmony 后置补丁注入。
        ///     </para>
        /// </summary>
        internal void AddBoundSpellArg(LocString description)
        {
            if (_echoSpell == null)
            {
                description.Add("BoundSpellClause", string.Empty);
                return;
            }

            var clause = new LocString("powers", $"{Id.Entry}.boundClause");
            clause.Add("SpellName", new LocString("cards", $"{_echoSpell.Id.Entry}.title").GetFormattedText());

            description.Add("BoundSpellClause", clause.GetFormattedText());
        }

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            _awaitingSpell = true;

            if (Owner?.Player is not { } player)
                return;

            // 法术槽 +1（增量由卡面的 SlotBonus 经 Amount 传入，保证与文案同源）
            var bonus = Math.Max(1, (int)Amount);
            var slots = await MagicWineFoxSpellCmd.EnsurePower(player);
            slots?.EnsureCapacity(slots.SlotCapacity + bonus);
        }

        /// <summary>
        ///     绑定的那张法术卡结算后，显式搬进消耗堆。
        ///     <para>
        ///         **不能**靠 <c>BeforeCardPlayed</c> 里 <c>AddKeyword(Exhaust)</c> 实现：
        ///         卡牌的结算堆在打出前就已确定，那时再加关键字无效，卡还是会进弃牌堆。
        ///         项目里的做法是显式搬迁（见 <c>ImprovisedWeapon</c> / <c>InequivalentExchange</c>）。
        ///     </para>
        /// </summary>
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (_boundCardInstance == null || !ReferenceEquals(cardPlay.Card, _boundCardInstance))
                return;

            _boundCardInstance = null;
            await CardPileCmd.Add(cardPlay.Card, PileType.Exhaust);
        }

        /// <summary>
        ///     装填入口调用：把这张法术绑定为「始终释放」。
        ///     <para>
        ///         **不拦截装填**——这张牌照常进入法杖；绑定只决定「法杖清空后自动装回哪张」。
        ///         返回 true 表示本次绑定成功（只绑定第一张）。
        ///     </para>
        /// </summary>
        public bool TryBind(CardModel card)
        {
            if (!_awaitingSpell || !IsBindable(card))
                return false;

            _echoSpell = card.CreateClone();
            _boundCardInstance = card;
            _awaitingSpell = false;
            return true;
        }

        private static bool IsBindable(CardModel? card)
        {
            return card is MagicWineFoxSpellCard { IsLoadable: true };
        }
    }
}
