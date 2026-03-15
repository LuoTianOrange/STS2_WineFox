using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Cards;

public static class WineFoxKeywords
{
    [CustomEnum]
    public static CardKeyword Stress; //应力
    [CustomEnum] 
    public static CardKeyword Digging; //挖掘
    [CustomEnum]
    public static CardKeyword Wood; //木板
    [CustomEnum]
    public static CardKeyword Stone; //圆石
    [CustomEnum] 
    public static CardKeyword Iron; //铁锭
    
    public static bool IsStress(this CardModel card)
    {
        return card.Keywords.Contains(Stress);
    }
    
    public static bool IsDigging(this CardModel card)
    {
        return card.Keywords.Contains(Digging);
    }
    
    public static bool IsWood(this CardModel card)
    {
        return card.Keywords.Contains(Wood);
    }
    
    public static bool IsStone(this CardModel card)
    {
        return card.Keywords.Contains(Stone);
    }
    
    public static bool IsIron(this CardModel card)
    {
        return card.Keywords.Contains(Iron);
    }
}