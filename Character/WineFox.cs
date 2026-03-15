using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Basic;
using STS2_WineFox.Relics;

namespace STS2_WineFox.Character;

public class WineFox : PlaceholderCharacterModel
{
    public static readonly Color Color = new("ffb6c1");

    public override Color NameColor => Color;
    public override string PlaceholderID => "theironclad";
    public override int StartingHp => 80;
    public override int StartingGold => 99;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override CardPoolModel CardPool => ModelDb.CardPool<WineFoxCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<WineFoxRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<WineFoxPotionPool>();
    
    //初始牌组
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<WineFoxStrike>(),
        ModelDb.Card<WineFoxStrike>(),
        ModelDb.Card<WineFoxStrike>(),
        ModelDb.Card<WineFoxStrike>(),
        ModelDb.Card<WineFoxDefend>(),
        ModelDb.Card<WineFoxDefend>(),
        ModelDb.Card<WineFoxDefend>(),
        ModelDb.Card<WineFoxDefend>(),
        ModelDb.Card<BasicMine>()
    ];
    
    //初始遗物
    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<HandCrank>()];
    
    //Visuals
    // public override string CustomVisualPath => "res://scenes/oddmelt/oddmelt.tscn";
    // public override string CustomIconTexturePath => "res://images/oddmelt/character_icon_oddmelt.png";
    // public override string CustomCharacterSelectIconPath => "res://images/oddmelt/char_select_oddmelt.png";
    // public override string CustomCharacterSelectLockedIconPath => "res://images/oddmelt/char_select_oddmelt_locked.png";
    // public override string CustomMapMarkerPath => "res://images/oddmelt/map_marker_oddmelt.png";
}