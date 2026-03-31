using STS2_WineFox.Character;
using STS2RitsuLib.Content;

namespace STS2_WineFox.Epoch
{
    /// <summary>
    ///     Stable ids and localization keys for WineFox timeline content. Story column title uses
    ///     <c>epochs.STORY_{StoryId.ToUpperInvariant()}</c> (see
    ///     <see cref="MegaCrit.Sts2.Core.Timeline.EpochModel.StoryTitle" />)
    ///     → e.g. <c>STORY_STS2_WINEFOX</c>. Each epoch uses <c>epochs.{Id}.*</c> for
    ///     <c>title</c>, <c>description</c>, <c>unlockInfo</c>, <c>unlock</c>. Note:
    ///     <see cref="STS2RitsuLib.Timeline.Scaffolding.CardUnlockEpochTemplate" /> and
    ///     <see cref="STS2RitsuLib.Timeline.Scaffolding.RelicUnlockEpochTemplate" /> override
    ///     <see cref="MegaCrit.Sts2.Core.Timeline.EpochModel.UnlockText" /> with generated text from
    ///     unlocked models, so <c>epochs.{Id}.unlockText</c> in JSON is only shown for epochs that use
    ///     the base implementation (here: <see cref="WineFoxCharacterEpoch" />).
    /// </summary>
    internal static class WineFoxTimelineKeys
    {
        /// <summary>
        ///     <see cref="STS2RitsuLib.Timeline.Scaffolding.ModStoryTemplate.StoryKey" /> and every epoch’s
        ///     <see cref="MegaCrit.Sts2.Core.Timeline.EpochModel.StoryId" /> — keep identical so the story column groups epochs.
        /// </summary>
        internal const string TimelineStoryId = "STS2_WineFox";

        internal const string CharacterEpochId = "WINEFOX_CHARACTER_EPOCH";

        internal const string CardEpochId = "WINEFOX_CARD_EPOCH";

        internal const string VictoryEpochId = "WINEFOX_VICTORY_EPOCH";

        internal const string EliteMilestoneEpochId = "WINEFOX_ELITE_MILESTONE_EPOCH";

        internal const string BossMilestoneEpochId = "WINEFOX_BOSS_MILESTONE_EPOCH";

        internal const string AscensionOneEpochId = "WINEFOX_ASCENSION_ONE_EPOCH";

        /// <summary>
        ///     Normalized public entry for <see cref="WineFox" /> from <see cref="ModContentRegistry.GetFixedPublicEntry" />
        ///     (already upper-case segments). Must match runtime <c>character.Id.Entry</c> for
        ///     <c>ObtainCharUnlockEpoch</c>. Localize act epochs under <c>epochs.{Id}.*</c> using
        ///     <see cref="ActCompletionEpochId" /> (0–2). Ids end with <c>…_WINE_FOX2_EPOCH</c> etc. because
        ///     <see cref="ModContentRegistry" /> splits <c>WineFox</c> → <c>WINE_FOX</c> (not <c>WINEFOX</c>).
        /// </summary>
        internal static string ModCharacterEntryUpper =>
            ModContentRegistry.GetFixedPublicEntry(Const.ModId, typeof(WineFox));

        /// <summary>
        ///     Vanilla act index passed to <c>ObtainCharUnlockEpoch</c>: 0 = Act 1 cleared → …2_EPOCH, 1 → …3_EPOCH, 2 → …4_EPOCH.
        /// </summary>
        internal static string ActCompletionEpochId(int obtainCharUnlockActIndex)
        {
            return ModCharacterEntryUpper + (obtainCharUnlockActIndex + 2) + "_EPOCH";
        }
    }
}
