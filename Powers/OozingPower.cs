using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class OozingPower : PotionDeathMarkPower
    {
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.OozingPowerIcon);

        protected override async Task OnMarkedOwnerDied(PlayerChoiceContext choiceContext, float deathAnimLength)
        {
            var spawnPosition = GetMarkedOwnerPosition();
            var slimes = new List<Creature>
            {
                await CreatureCmd.Add<LeafSlimeM>(CombatState),
                await CreatureCmd.Add<TwigSlimeM>(CombatState)
            };

            SpaceSpawnedSlimes(slimes, spawnPosition);
            AddCardReward(CardRarity.Rare);
        }

        private Vector2? GetMarkedOwnerPosition()
        {
            return NCombatRoom.Instance?.RemovingCreatureNodes
                .FirstOrDefault(node => node.Entity == Owner)
                ?.Position;
        }

        private static void SpaceSpawnedSlimes(IReadOnlyList<Creature> slimes, Vector2? spawnPosition)
        {
            if (slimes.Count < 2 || NCombatRoom.Instance == null)
                return;

            var offsets = new[] { -90f, 90f };
            for (var i = 0; i < slimes.Count; i++)
            {
                var node = NCombatRoom.Instance.GetCreatureNode(slimes[i]);
                if (node == null)
                    continue;

                if (spawnPosition.HasValue)
                    node.Position = spawnPosition.Value;

                node.Position += new Vector2(offsets[i], i % 2 == 0 ? 0f : -20f);
            }
        }
    }
}
