using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Keywords;

namespace STS2_WineFox.Cards
{
    public static class WineFoxKeywords
    {
        public const string Stress = "stress";
        public const string Digging = "digging";
        public const string Wood = "wood";
        public const string Stone = "stone";
        public const string Iron = "iron";
        public const string Plant = "plant";
        public const string Steam = "steam";
        public const string Strength = "strength";
        public const string Plating = "plating";
        public const string Diamond = "diamond";
        public const string Material =  "material";

        extension(CardModel card)
        {
            public bool IsStress()
            {
                return card.HasModKeyword(Stress);
            }

            public bool IsDigging()
            {
                return card.HasModKeyword(Digging);
            }

            public bool IsWood()
            {
                return card.HasModKeyword(Wood);
            }

            public bool IsStone()
            {
                return card.HasModKeyword(Stone);
            }

            public bool IsIron()
            {
                return card.HasModKeyword(Iron);
            }

            public bool IsPlant()
            {
                return card.HasModKeyword(Plant);
            }

            public bool IsSteam()
            {
                return card.HasModKeyword(Steam);
            }

            public bool IsDiamond()
            {
                return card.HasModKeyword(Diamond);
            }
        }
    }
}
