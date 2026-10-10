using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.VFX;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     汇聚之光：本场战斗中每释放过一次本法术，伤害提高。
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class GatheringLight() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(6m, ValueProp.Move),
            new("BonusPerCast", 3m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGatheringLight);

        public override string SpellIconPath => Const.Paths.SpellIconGatheringLight;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;
        
        private static readonly Dictionary<Player, int> CastCounts = [];

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner || owner.Creature?.CombatState is not { } combatState)
                return;

            var targets = ResolveSpellTargets(context);
            if (targets.Count == 0)
                return;

            var bonus = DynamicVars["BonusPerCast"].BaseValue;

            var attack = DamageCmd.Attack(context.DamageWithModifiers(DynamicVars.Damage.BaseValue))
                .FromCard(context.SourceCard, null);

            // 绿色光束特效
            if (owner.Creature is { } caster && targets[0] is { } beamTarget)
                GatheringLightVfx.Play(caster, beamTarget);

            if (context.TargetsAllEnemies)
                await attack.TargetingAllOpponents(combatState).Execute(context.ChoiceContext);
            else
                await attack.Targeting(targets[0]).Execute(context.ChoiceContext);

            // 同名卡一起成长。
            // 注意 AllCards 是**所有牌堆的并集**（手牌原卡 + 法杖克隆都在内），
            // 逐个加会变成一次释放 +2 次；因此按「本场已释放次数」统一重算基础值，
            // 无论场上有几份副本，数值都一致且每次释放只 +1 次加成。
            var castCount = CastCounts.TryGetValue(owner, out var done) ? done : 0;
            CastCounts[owner] = castCount + 1;
            var newBase = DynamicVars.Damage.BaseValue + bonus;

            foreach (var card in owner.PlayerCombatState.AllCards.OfType<GatheringLight>())
                card.DynamicVars.Damage.BaseValue = newBase;
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["BonusPerCast"].UpgradeValueBy(1m);
        }
    }
}
