using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Basic;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Cards.DynamicVars
{
    public sealed class MineMaterialVar<TPower>(string name, decimal baseValue)
        : DynamicVar(name, baseValue)
        where TPower : PowerModel
    {
        public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
            bool runGlobalHooks)
        {
            PreviewValue = card is BasicMine mine
                ? CalculateAmount(mine, runGlobalHooks)
                : BaseValue;
        }

        protected override decimal GetBaseValueForIConvertible()
        {
            return _owner is not BasicMine mine
                ? base.GetBaseValueForIConvertible()
                : CalculateAmount(mine, CombatManager.Instance.IsInProgress);
        }

        private decimal CalculateAmount(BasicMine card, bool runGlobalHooks)
        {
            if (card.Owner?.Creature == null)
                return BaseValue;

            if (!runGlobalHooks || card.CombatState == null)
                return BaseValue;
            
            var hasStress = card.Owner.Creature.Powers
                .OfType<StressPower>()
                .Any(p => p.Amount > 0);

            return hasStress ? BaseValue * 2 : BaseValue;
        }
    }
}