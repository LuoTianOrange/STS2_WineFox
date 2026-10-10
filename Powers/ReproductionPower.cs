using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class ReproductionPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ReproductionPowerIcon);

        private MagicWineFoxSpellSlotSnapshot? _boundSpell;

        /// <summary>
        ///     是否处于「等待认领」状态：打出复现时法杖里还没有法术，
        ///     于是认领之后再装填进法杖的第一个法术。
        /// </summary>
        private bool _awaitingSpell;

        /// <summary>当前绑定的法术</summary>
        public CardModel? BoundSpell => _boundSpell?.Card;

        /// <summary>
        ///     由复现在打出时写入要重新装填的法术。
        ///     传入 <c>null</c>（打出时法杖为空）则转入等待状态，
        ///     等之后装填的第一个法术由 <see cref="TryBind" /> 认领。
        /// </summary>
        public void Bind(MagicWineFoxSpellSlotSnapshot? snapshot)
        {
            _boundSpell = snapshot;
            _awaitingSpell = snapshot == null;
        }

        /// <summary>
        ///     装填时调用：若正在等待认领，则绑定这张刚装填的法术。
        ///     与 <see cref="EchoesSequencePower.TryBind" /> 同一套语义。
        /// </summary>
        public bool TryBind(MagicWineFoxSpellSlotSnapshot snapshot)
        {
            if (!_awaitingSpell || !IsBindable(snapshot.Card))
                return false;

            _boundSpell = snapshot;
            _awaitingSpell = false;
            return true;
        }

        /// <summary>
        ///     复现只认法术本体：法术修正符是依附于法术的修饰，不该被复现当成"那个法术"。
        ///     判据与 <see cref="EchoesSequencePower" /> 保持一致。
        /// </summary>
        public static bool IsBindable(CardModel? card)
        {
            return card is MagicWineFoxSpellCard { IsLoadable: true }
                   and not IMagicWineFoxSpellModifierCard;
        }

        /// <summary>
        ///     把绑定法术的名字注入描述。
        /// </summary>
        internal void AddBoundSpellArg(LocString description)
        {
            if (_boundSpell?.Card is not { } spell)
            {
                description.Add("BoundSpellClause", string.Empty);
                return;
            }

            var clause = new LocString("powers", $"{Id.Entry}.boundClause");
            clause.Add("SpellName", new LocString("cards", $"{spell.Id.Entry}.title").GetFormattedText());

            description.Add("BoundSpellClause", clause.GetFormattedText());
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext,
            Player player)
        {
            if (player.Creature != Owner)
                return;

            var snapshot = _boundSpell;
            _boundSpell = null;
            _awaitingSpell = false;

            if (snapshot != null)
            {
                var slots = await MagicWineFoxSpellCmd.EnsurePower(player);
                if (slots != null)
                {
                    Flash();

                    // 槽满时 TryLoad 会静默失败，退回抽牌堆而不是让这张法术凭空蒸发。
                    if (!slots.TryLoad(snapshot))
                        await CardPileCmd.Add(snapshot.Card, PileType.Draw, CardPilePosition.Random);
                    else
                        MagicWineFoxSpellCmd.RefreshWandCardValues(slots);
                }
            }

            await PowerCmd.Remove(this);
        }
    }
}
