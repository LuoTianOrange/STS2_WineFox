using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     记录一次法术释放里参与过格挡/伤害修正的模型。
    ///     <para>
    ///         释放不是「打出卡牌」，所以这些模型收不到 <see cref="AbstractModel.AfterCardPlayed" />，
    ///         臂甲这类一次性遗物会一直认为本次战斗还没用过。释放结束时对参与者各补一次结束信号，
    ///         让它们正常进入已用状态；没有参与过修正的模型不会被通知。
    ///     </para>
    /// </summary>
    public static class SpellPlayParticipants
    {
        private static HashSet<AbstractModel>? _collecting;

        /// <summary>开始一次收集；嵌套的释放会共用同一批参与者。</summary>
        public static void Begin()
        {
            _collecting ??= [];
        }

        /// <summary>由修正钩子调用；不在一轮释放里时直接忽略。</summary>
        public static void Record(IEnumerable<AbstractModel>? modifiers)
        {
            if (_collecting == null || modifiers == null)
                return;

            foreach (var model in modifiers)
                _collecting.Add(model);
        }

        /// <summary>结束收集并对每个参与者补发一次打出结束信号。</summary>
        public static async Task FinishAsync(PlayerChoiceContext choiceContext, CardPlay? play)
        {
            var collected = _collecting;
            _collecting = null;

            if (collected == null || play == null || collected.Count == 0)
                return;

            foreach (var model in collected)
            {
                if (model is not RelicModel)
                    continue;

                try
                {
                    await model.AfterCardPlayed(choiceContext, play);
                }
                catch
                {
                }
            }
        }
    }
}
