using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class InfestedPower : PotionDeathMarkPower
    {
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.InfestedPowerIcon);

        protected override async Task OnMarkedOwnerDied(PlayerChoiceContext choiceContext, float deathAnimLength)
        {
            var spawnPosition = GetMarkedOwnerPosition();
            List<Creature> wrigglers = [];
            for (var i = 1; i <= 2; i++)
            {
                var wriggler = (Wriggler)ModelDb.Monster<Wriggler>().ToMutable();
                wriggler.StartStunned = true;
                var creature = await CreatureCmd.Add(wriggler, CombatState);
                creature.SlotName = $"wriggler{i}";
                wrigglers.Add(creature);
            }

            SpaceSpawnedWrigglers(wrigglers, spawnPosition);
            AddCardReward(CardRarity.Uncommon);
        }

        private Vector2? GetMarkedOwnerPosition()
        {
            return NCombatRoom.Instance?.RemovingCreatureNodes
                .FirstOrDefault(node => node.Entity == Owner)
                ?.Position;
        }

        private static void SpaceSpawnedWrigglers(IReadOnlyList<Creature> wrigglers, Vector2? spawnPosition)
        {
            if (wrigglers.Count < 2 || NCombatRoom.Instance == null)
                return;

            var offsets = new[]
            {
                new Vector2(-150f, -45f),
                new Vector2(150f, -65f)
            };
            for (var i = 0; i < wrigglers.Count; i++)
            {
                var node = NCombatRoom.Instance.GetCreatureNode(wrigglers[i]);
                if (node == null)
                    continue;

                if (spawnPosition.HasValue)
                    node.Position = spawnPosition.Value;

                node.Position += offsets[i];
            }
        }
    }
}
