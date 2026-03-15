using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using STS2_WineFox.Character;

namespace STS2_WineFox.Cards;

[Pool(typeof(WineFoxCardPool))]
public abstract class WineFoxCard(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true) : CustomCardModel(baseCost, type, rarity, target, showInCardLibrary, autoAdd)
{
    
}