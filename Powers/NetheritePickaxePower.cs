using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Commands;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers;

public class NetheritePickaxePower : WineFoxPower
{
    private CardModel? _excludedCard;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => Icons(Const.Paths.NetheritePickaxePowerIcon);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new("Threshold", 2m)];

    public override bool IsInstanced => true;

    public override int DisplayAmount
    {
        get
        {
            var d = GetInternalData<Data>();
            var threshold = (int)DynamicVars["Threshold"].BaseValue;
            if (threshold <= 0) threshold = 2;
            var remainder = d.CardsPlayed % threshold;
            return threshold - remainder;
        }
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public void ExcludeCard(CardModel card)
    {
        _excludedCard = card;
        InvokeDisplayAmountChanged();
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        CombatState combatState)
    {
        if (side == Owner.Side)
        {
            var d = GetInternalData<Data>();
            d.CardsPlayed = 0;
            d.TriggerCount = 0;
            InvokeDisplayAmountChanged();
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.IsAutoPlay)
            return;

        if (cardPlay.Card?.Owner?.Creature != Owner)
            return;

        if (_excludedCard != null && cardPlay.Card == _excludedCard)
        {
            _excludedCard = null;
            return;
        }

        var d = GetInternalData<Data>();

        d.CardsPlayed++;
        InvokeDisplayAmountChanged();
        var threshold = (int)DynamicVars["Threshold"].BaseValue;
        if (threshold <= 0) threshold = 2;


        var triggers = d.CardsPlayed / threshold - d.TriggerCount;
        if (triggers > 0)
        {
            Flash();

            var ownerCreature = Owner;

            var amountPerTrigger = (decimal)Amount;
            var totalAmount = amountPerTrigger * triggers;

            await MaterialCmd.GainAllMaterials(ownerCreature, totalAmount);

            d.TriggerCount += triggers;
            InvokeDisplayAmountChanged();
        }
    }

    private class Data
    {
        public int CardsPlayed;
        public int TriggerCount;
    }
}