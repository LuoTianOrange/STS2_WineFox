using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Content;
using STS2RitsuLib.Keywords;

namespace STS2_WineFox.Cards
{
    public static class WineFoxKeywords
    {
        public const string StressKey = "stress";
        public const string DiggingKey = "digging";
        public const string WoodKey = "wood";
        public const string StoneKey = "stone";
        public const string IronKey = "iron";
        public const string PlantKey = "plant";
        public const string SteamKey = "steam";
        public const string PlatingKey = "plating";
        public const string DiamondKey = "diamond";
        public const string MaterialKey = "material";
        public const string RadiationLeakKey = "radiation_leak";
        public const string EasyPeasyKey = "easypeasy";
        public const string CraftKey = "craft";
        public const string ExchangeKey = "exchange";
        public const string MagicKey = "magic";

        public static readonly string Stress = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, StressKey);
        public static readonly string Digging = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, DiggingKey);
        public static readonly string Wood = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, WoodKey);
        public static readonly string Stone = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, StoneKey);
        public static readonly string Iron = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, IronKey);
        public static readonly string Plant = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, PlantKey);
        public static readonly string Steam = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, SteamKey);
        public static readonly string Plating = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, PlatingKey);
        public static readonly string Diamond = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, DiamondKey);
        public static readonly string Material = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, MaterialKey);

        public static readonly string RadiationLeak =
            ModContentRegistry.GetQualifiedKeywordId(Const.ModId, RadiationLeakKey);

        public static readonly string EasyPeasy = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, EasyPeasyKey);
        public static readonly string Craft = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, CraftKey);
        public static readonly string Exchange = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, ExchangeKey);
        public static readonly string Magic = ModContentRegistry.GetQualifiedKeywordId(Const.ModId, MagicKey);

        public static readonly CardKeyword StressKeyword = Stress.GetModCardKeyword();
        public static readonly CardKeyword DiggingKeyword = Digging.GetModCardKeyword();
        public static readonly CardKeyword WoodKeyword = Wood.GetModCardKeyword();
        public static readonly CardKeyword StoneKeyword = Stone.GetModCardKeyword();
        public static readonly CardKeyword IronKeyword = Iron.GetModCardKeyword();
        public static readonly CardKeyword PlantKeyword = Plant.GetModCardKeyword();
        public static readonly CardKeyword SteamKeyword = Steam.GetModCardKeyword();
        public static readonly CardKeyword PlatingKeyword = Plating.GetModCardKeyword();
        public static readonly CardKeyword DiamondKeyword = Diamond.GetModCardKeyword();
        public static readonly CardKeyword MaterialKeyword = Material.GetModCardKeyword();
        public static readonly CardKeyword RadiationLeakKeyword = RadiationLeak.GetModCardKeyword();
        public static readonly CardKeyword EasyPeasyKeyword = EasyPeasy.GetModCardKeyword();
        public static readonly CardKeyword CraftKeyword = Craft.GetModCardKeyword();
        public static readonly CardKeyword ExchangeKeyword = Exchange.GetModCardKeyword();
        public static readonly CardKeyword MagicKeyword = Magic.GetModCardKeyword();

        extension(CardModel card)
        {
            public bool IsStress()
            {
                return card.HasModKeyword(StressKeyword);
            }

            public bool IsDigging()
            {
                return card.HasModKeyword(DiggingKeyword);
            }

            public bool IsWood()
            {
                return card.HasModKeyword(WoodKeyword);
            }

            public bool IsStone()
            {
                return card.HasModKeyword(StoneKeyword);
            }

            public bool IsIron()
            {
                return card.HasModKeyword(IronKeyword);
            }

            public bool IsPlant()
            {
                return card.HasModKeyword(PlantKeyword);
            }

            public bool IsSteam()
            {
                return card.HasModKeyword(SteamKeyword);
            }

            public bool IsPlating()
            {
                return card.HasModKeyword(PlatingKeyword);
            }

            public bool IsDiamond()
            {
                return card.HasModKeyword(DiamondKeyword);
            }

            public bool IsMaterial()
            {
                return card.HasModKeyword(MaterialKeyword);
            }

            public bool IsRadiationLeak()
            {
                return card.HasModKeyword(RadiationLeakKeyword);
            }

            public bool IsEasyPeasy()
            {
                return card.HasModKeyword(EasyPeasyKeyword);
            }

            public bool IsCraft()
            {
                return card.HasModKeyword(CraftKeyword);
            }

            public bool IsExchange()
            {
                return card.HasModKeyword(ExchangeKeyword);
            }

            public bool IsMagic()
            {
                return card.HasModKeyword(MagicKeyword);
            }
        }
    }
}
