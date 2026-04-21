using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Ancient
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class NetheritePickaxe() : WineFoxCard(
        1, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNetheritePickaxe);

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone, WineFoxKeywords.Iron, WineFoxKeywords.Diamond];
        
        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var creature = Owner?.Creature;
            if (creature == null)
                return;

            var appliedPower = await PowerCmd.Apply<NetheritePickaxePower>(creature, 1m, creature, this);
            if (appliedPower != null) appliedPower.ExcludeCard(this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}
