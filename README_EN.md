<div align="center">
<img style="width: 256px; height: auto; border-radius: 12px;" src="STS2_WineFox/mod_image.png" alt="WineFox Mod"/>
<h1>STS2 WineFox Mod</h1>
<div style="display: flex;flex-direction: row;justify-content: center;">
<img src="https://img.shields.io/badge/-C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#"/> 
<img src="https://img.shields.io/badge/-.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET"/> 
<img src="https://img.shields.io/badge/-Godot-478CBF?style=for-the-badge&logo=godotengine&logoColor=white" alt="Godot"/> 
<img src="https://img.shields.io/badge/-Slay%20the%20Spire%202-8B0000?style=for-the-badge&logoColor=white" alt="Slay the Spire 2"/> 
<a href="https://github.com/BAKAOLC/STS2-RitsuLib"><img src="https://img.shields.io/badge/-STS2--RitsuLib-5538DD?style=for-the-badge&logo=github&logoColor=white" alt="STS2-RitsuLib"/></a> 
<img src="https://img.shields.io/badge/version-1.1.14-ffaf50?style=for-the-badge" alt="Version"/>
</div>
<div><a href="README.md">简体中文</a> | <b>English</b></div>
</div>

WineFox is a character originating from the Minecraft mod *Touhou Gensokyo Maid*, now brought to Slay the Spire 2 as a custom character mod. Her playstyle revolves around **crafting** and **resource management** — gather Planks, Cobblestone, Iron Ingots, and Diamonds, then leverage Craft, Stress, and powerful combos to dominate combat.

---

## Character Overview

| Attribute      | Value              |
|----------------|--------------------|
| Starting HP    | 80                 |
| Starting Gold  | 99                 |
| Color          | `#ffaf50` (Orange) |
| Starting Relic | Hand Crank         |

### Starting Deck

| Card                   | Count |
|------------------------|-------|
| Strike (WineFoxStrike) | 4     |
| Defend (WineFoxDefend) | 4     |
| Basic Mining           | 1     |
| Basic Crafting         | 1     |

---

## Card List

### Basic

| Card           | Cost | Type   | Effect                              | Upgraded   |
|----------------|------|--------|-------------------------------------|------------|
| Strike         | 1    | Attack | Deal 6 damage.                      | Damage → 9 |
| Defend         | 1    | Skill  | Gain 5 Block.                       | Block → 8  |
| Basic Mining   | 1    | Skill  | Gain 2 Plank and 2 Cobblestone.     | +1 each    |
| Basic Crafting | 1    | Skill  | **Craft**. Gain 5 Block. **Retain** | Cost → 0   |

### Common

| Card                        | Cost | Type   | Effect                                                                         | Upgraded               |
|-----------------------------|------|--------|--------------------------------------------------------------------------------|------------------------|
| Backup Workbench            | 0    | Skill  | **Exhaust**; **Craft**.                                                        | +Retain                |
| Barrier Wave                | 1    | Skill  | Cause ALL enemies to lose 6 HP. Gain 5 Block.                                  | HP → 9; Block → 7      |
| Change Equipment            | 0    | Skill  | Draw 2 cards, then choose 2 cards in your hand to shuffle into your draw pile. | +1 each                |
| Full-Force Ram              | 1    | Attack | Deal 12 damage to all enemies. Shuffle a Dazed into your draw pile.            | Damage → 16            |
| Getting Wood                | 1    | Attack | Gain 4 Plank. Deal damage equal to the Plank gained.                           | Plank → 6              |
| Improvised Weapon           | 1    | Attack | Deal 8 damage. Exhaust a card.                                                 | Damage → 12            |
| "White Zombie"              | 1    | Attack | Deal 8 damage. Gain 1 Iron Ingot.                                              | Damage+3; Ingot+1      |
| Light Assault               | 0    | Attack | Deal damage. This card deals 1 less damage for every 2 Materials you have.     | Base damage ↑          |
| Magic Missile               | 1    | Skill  | Cause an enemy to lose 6 HP twice.                                             | HP → 8                 |
| Mechanical Drill            | 2    | Skill  | Deal damage N times. Gain 2 Iron Ingot. If Stress was spent, refund Energy.    | Ingot → 3              |
| Plant Trees                 | 1    | Skill  | Gain 5 Block. At the start of next turn, gain 4 Plank.                         | Block → 7; Plank → 6   |
| Shift, Accelerate, Take Off | 1    | Skill  | Gain 1 Stress. Draw 1 card.                                                    | Draw → 2               |
| Preprocessing               | 0    | Skill  | **Innate**; **Exhaust**; Gain 3 Plank and 3 Cobblestone.                       | Also gain 3 Block      |
| Quick Attack                | 1    | Attack | **Ethereal**; Deal damage N times.                                             | Removes Ethereal       |
| Quick Shelter               | 1    | Skill  | **Ethereal**; Gain Plank and Cobblestone. Gain Block. Draw 1 card.             | Block ↑                |
| Regroup                     | 1    | Skill  | **Exhaust**; Draw 1 card. Put 1 card from your discard pile into your hand.    | Removes Exhaust        |
| Tic-Tac-Toe Grid            | 1    | Attack | Deal 8 damage. **Craft**.                                                      | Damage → 11            |
| Traditionalist              | 0    | Skill  | Spend 2 Plank. **Craft**. Draw 1 card.                                         | Draw → 2               |
| Vacant Domain               | 3    | Skill  | Gain 14 Block. Gain 8 Cobblestone.                                             | Block → 18; Stone → 12 |
| Wind Crank                  | 1    | Skill  | Gain 3 Stress. End your turn.                                                  | Stress → 4             |

### Uncommon

| Card                                        | Cost | Type   | Effect                                                                                                                     | Upgraded                                    |
|---------------------------------------------|------|--------|----------------------------------------------------------------------------------------------------------------------------|---------------------------------------------|
| Alternator                                  | 0    | Skill  | Gain Energy. Consume 1 Stress to gain additional Energy.                                                                   | More bonus Energy                           |
| Another Way                                 | 0    | Skill  | Spend 2 Plank. Gain 5 Block and draw 2 cards.                                                                              | Block → 8; Draw → 3                         |
| Anticipate Advantage                        | 1    | Skill  | If any enemy intends to attack, gain 3 Dexterity this turn.                                                                | Dexterity → 5                               |
| Blueprint Printing                          | 1    | Skill  | **Exhaust**; Spend Plank and Cobblestone. Choose 1 card in your hand; add copies that cost 1 less and have Exhaust.        | 3 copies                                    |
| Bucket                                      | 1    | Skill  | **Exhaust**; Gain 8 Block. At the start of next turn, gain 8 Block. Apply 2 Weak to all units.                             | Block → 11                                  |
| Crushing Wheel Array                        | 3    | Attack | Deal damage to all enemies. This card costs 1 less for each time you spent Materials this turn.                            | —                                           |
| Dripstone Trap                              | 2    | Attack | Spend all Cobblestone. For each spent, deal damage to a random enemy and apply 1 Vulnerable.                               | —                                           |
| Emergency Repair                            | 1    | Attack | Deal damage, gain Block, and at the start of next turn gain 2 Stress.                                                      | +2 each                                     |
| Erosion                                     | 1    | Skill  | Apply 6 Disintegration to an enemy. ALL enemies with Disintegration lose HP equal to their stacks.                         | Stacks → 10                                 |
| Fox Bite                                    | 2    | Skill  | **Retain**; Apply 4 Weak. Apply 4 Vulnerable.                                                                              | Each → 7                                    |
| Full Barrage                                | 2    | Attack | Spend all Materials. Deal damage for each Material spent.                                                                  | +Retain                                     |
| Hammer Slam                                 | 4    | Attack | **Retain**; Each turn this card is Retained, its cost is reduced by 1. Deal damage; Strength bonuses are multiplied by 3x. | —                                           |
| Highly Focused                              | 1    | Skill  | This turn, double your current Strength.                                                                                   | Cost → 0                                    |
| Intermittent Chanting                       | 1    | Power  | Gain 1 Weak. Whenever you cause an enemy to lose HP, that enemy gains 1 Block and you gain 2 Block.                        | You gain → 3                                |
| Kaleidoscope Tales: Wok                     | 1    | Skill  | Draw 1 card. Choose 1 card in your hand to change.                                                                         | —                                           |
| Launch Platform                             | 1    | Power  | The first time you consume Stress each turn, gain 1 Energy.                                                                | Cost → 0                                    |
| Mass Production                             | 2    | Skill  | **Multiplayer only**; When you Craft this turn, give each teammate 1 copy of the crafted card.                             | Cost → 1                                    |
| Mechanical Saw                              | 1    | Attack | Deal damage to all enemies. Spend 1 Stress to deal additional damage.                                                      | —                                           |
| Memory                                      | 1    | Power  | At the start of your turn, add an Exhaust copy of the last Skill you played last turn to your hand.                        | Copy also gains Retain                      |
| Otherworld Crossing                         | 1    | Power  | At the start of each turn, choose a card in your hand; add an Ethereal Exhaust copy to your hand.                          | +Innate                                     |
| Petrification Spell                         | 1    | Skill  | Gain 4 Cobblestone. Cause all enemies to lose HP equal to the Cobblestone gained.                                          | Cobblestone → 6                             |
| "W" to Think                                | 1    | Skill  | **Exhaust**; Look at the top 3 cards of your draw pile. Choose 1 to add to your hand; it gains Replay and Exhaust.         | Cost → 0                                    |
| Production Log                              | 1    | Power  | Cards you temporarily add during combat gain Retain. At end of turn, gain Block for each Retained card.                    | Cost → 0                                    |
| Recuperate                                  | 1    | Skill  | **Exhaust**; Draw 5 cards. Discard all Attack cards in your hand.                                                          | Also discard Curse, Quest, and Status cards |
| Redemption Strike                           | 1    | Attack | Deal damage. Heal all characters for HP.                                                                                   | Heal ↑                                      |
| River Power Plant                           | 2    | Skill  | Gain Block and 2 Stress.                                                                                                   | Block → 16                                  |
| Scratch                                     | X    | Attack | Deal X damage 2X times.                                                                                                    | Each hit → X+2                              |
| Slash Blade: Nameless Blade "Wooden Puppet" | 1    | Power  | Whenever you play a Skill card, gain 2 Vigor.                                                                              | Vigor → 3                                   |
| Snowball to the Face                        | 1    | Attack | Deal damage. The next Skill you play this turn costs 0.                                                                    | Deal damage twice                           |
| Stress Response                             | 0    | Skill  | **Exhaust**; Lose 6 HP. Gain 1 Max HP, Energy, and draw 2 cards.                                                           | Gain → 3 Energy                             |
| Sweet Dream                                 | 0    | Skill  | **Exhaust**; Shuffle all cards in your hand into your draw pile, then draw that many cards.                                | Removes Exhaust                             |
| Workbench Backpack                          | 2    | Skill  | Gain 1 Energy. Craft. Reduce this card's cost by 1.                                                                        | Gain → 2 Energy                             |

### Rare

| Card                 | Cost | Type   | Effect                                                                                                                                          | Upgraded                 |
|----------------------|------|--------|-------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------|
| All-Item Warehouse   | 0    | Skill  | **Exhaust**; Gain 1 Energy; Draw 1 card; Gain 1 Plank, 1 Cobblestone, 1 Iron Ingot, 1 Diamond.                                                  | +Innate                  |
| Auto Crafter         | 1    | Power  | At the start of your turn, Craft.                                                                                                               | +Innate                  |
| Batch Craft          | X    | Skill  | Craft. Gain X copies of the crafted card.                                                                                                       | X+1 copies               |
| Cobblestone Form     | 2    | Power  | **Ethereal**; At the start of your turn, gain 2 Cobblestone; this amount increases by 2 each turn.                                              | Cost → 1                 |
| Easy Peasy           | 0    | Power  | At the start of each turn, gain 1 Energy and draw 1 extra card. At the start of each turn, add 1 Dazed to your hand; it gains 1 more each turn. | N+1                      |
| Explosion Magic      | 2    | Skill  | Apply 12 Burning to ALL enemies. Lose 3 Dexterity.                                                                                              | Burning → 15             |
| Hell's Gift          | 2    | Skill  | **Exhaust**; Add a random golden equipment card to your hand.                                                                                   | Card is Upgraded         |
| 996                  | 0    | Skill  | Choose up to 3 cards in your hand to change into Work Overtime.                                                                                 | Generates Work Overtime+ |
| Liberation           | 2    | Power  | At the end of your turn, trigger every Retained card in your hand.                                                                              | Cost → 1                 |
| Maid's Support       | 2    | Skill  | **Multiplayer only**; Give all players 5 Plating and 2 Thorns.                                                                                  | +1 each                  |
| Milk                 | 1    | Skill  | **Exhaust**; Remove all debuffs from yourself.                                                                                                  | Removes Exhaust          |
| Mining Gems          | 2    | Skill  | Gain 2 Diamond.                                                                                                                                 | Diamond → 3              |
| Netherite Chestplate | 2    | Power  | Gain Plating. When you gain Block, if you have Plating, double the Block you gain.                                                              | Plating ↑                |
| Times Have Changed   | 2    | Attack | Deal damage N times. Spend 1 Iron Ingot to make this card deal 1 additional hit this combat. Return this card to your hand.                     | Upgraded version         |
| Payback Time         | 3    | Attack | Deal base damage. For each Material you have gained this combat, deal additional damage.                                                        | Cost → 2                 |
| Planning Expert      | 2    | Power  | Whenever you play a Skill card, that card gains Retain.                                                                                         | Cost → 1                 |
| Shield Bash          | 0    | Attack | Deal damage to all enemies equal to N times your current Block. Clear all your Block. Draw 1 card.                                              | —                        |
| Spinning Hand        | 4    | Attack | Deal damage to all enemies. This card costs 1 less for each time you Craft this combat.                                                         | —                        |
| Spirit Fox Form      | 3    | Power  | **Ethereal**; Whenever you play an Attack card, apply 1 Slow to the target.                                                                     | Removes Ethereal         |
| Steam Engine         | 2    | Power  | At the start of each turn, gain 1 Stress.                                                                                                       | Cost → 1                 |
| Wireless Terminal    | 1    | Skill  | Craft. Return this card to your hand.                                                                                                           | +Retain                  |

### Ancient

| Card              | Cost | Type  | Effect                                                                                                 | Upgraded          |
|-------------------|------|-------|--------------------------------------------------------------------------------------------------------|-------------------|
| Forging           | 1    | Skill | **Retain**; Gain Block. Craft twice.                                                                   | Block ↑; Cost → 0 |
| Netherite Pickaxe | 1    | Power | Each time you play 2 cards in the same turn, gain 1 Plank, 1 Cobblestone, 1 Iron Ingot, and 1 Diamond. | +Innate           |

### Token (Combat-Generated)

Token cards are generated during combat by crafting recipes or specific cards. They do not appear in reward pools.

#### Craft Recipe Products

| Card               | Recipe                  | Effect                                                                                                                                     |
|--------------------|-------------------------|--------------------------------------------------------------------------------------------------------------------------------------------|
| Bare-Handed Strike | No materials            | **Exhaust**; Deal 1 damage.                                                                                                                |
| Wooden Sword       | 3 Plank                 | **Exhaust**; Deal damage; Gain Vigor.                                                                                                      |
| Stone Sword        | 1 Plank + 2 Cobblestone | **Exhaust**; Deal damage; Gain Strength.                                                                                                   |
| Iron Sword         | 1 Plank + 2 Iron Ingot  | **Exhaust**; Deal damage; The next N Attack cards are played an additional time.                                                           |
| Diamond Sword      | 1 Plank + 2 Diamond     | **Exhaust**; Deal damage; Each turn, the first N Attack cards are played an additional time.                                               |
| Wooden Pickaxe     | 4 Plank                 | **Exhaust**; At the start of next turn, gain Energy and draw cards.                                                                        |
| Stone Pickaxe      | 1 Plank + 3 Cobblestone | **Exhaust**; At the start of each turn, gain 1 Plank and 1 Cobblestone.                                                                    |
| Iron Pickaxe       | 1 Plank + 3 Iron Ingot  | **Exhaust**; For the next N times you gain resources, gain 3 additional Iron Ingot.                                                        |
| Diamond Pickaxe    | 1 Plank + 3 Diamond     | **Exhaust**; Every card you craft is Upgraded. Upgraded: also Crafts.                                                                      |
| Wooden Armor       | 4 Plank                 | **Exhaust**; Gain Block.                                                                                                                   |
| Stone Brick Armor  | 8 Cobblestone           | **Exhaust**; Gain Plating. At the start of each turn, lose 1 Dexterity.                                                                    |
| Iron Armor         | 8 Iron Ingot            | **Exhaust**; Gain Plating. At the start of each turn, gain Plating.                                                                        |
| Diamond Armor      | 8 Diamond               | **Exhaust**; Gain Plating. If you still have Plating at end of turn, retain your Block for this turn.                                      |
| Shield             | 6 Plank + 1 Iron Ingot  | **Exhaust**; Choose a character to gain Dexterity this turn. Cannot play this card again this turn or next turn. Upgraded: only this turn. |

#### Other Tokens

| Card                                  | Source                 | Effect                                                                                                                              |
|---------------------------------------|------------------------|-------------------------------------------------------------------------------------------------------------------------------------|
| Golden Sword                          | Hell's Gift            | Every 2 Attack cards you play, gain 1 Energy. All Attack cards gain Ethereal. Upgraded: removes Ethereal.                           |
| Golden Pickaxe                        | Hell's Gift            | Whenever you gain Materials, gain that much Block and deal 2x that amount as damage to a random enemy. Upgraded: 3x.                |
| Golden Armor                          | Hell's Gift            | **Exhaust**; Gain Buffer and 1 Artifact. When you lose Buffer, heal HP equal to the damage you would have taken.                    |
| Work Overtime                         | 996                    | **Exhaust**; Gain 1 Stress. Upgraded: also draw 1 card.                                                                             |
| Steel Chamber                         | Times Have Changed     | Deal damage N times. Return this card to your hand.                                                                                 |
| Refined Backpack: Crafting Upgrade    | Sophisticated Backpack | Grants your Sophisticated Backpack an extra effect: every 2 Crafts, you Craft 1 additional time. Can only be crafted once.          |
| Refined Backpack: Feeding Upgrade     | Sophisticated Backpack | Grants your Sophisticated Backpack an extra effect: at the start of each turn, gain 1 Regeneration. Can only be crafted once.       |
| Refined Backpack: Restock Upgrade     | Sophisticated Backpack | Grants your Sophisticated Backpack an extra effect: at the start of each combat, draw 2 additional cards. Can only be crafted once. |
| Refined Backpack: Savings Upgrade     | Sophisticated Backpack | Grants your Sophisticated Backpack an extra effect: at the end of combat, gain 15 additional Gold. Can only be crafted once.        |
| Refined Backpack: Smelting Upgrade    | Sophisticated Backpack | Grants your Sophisticated Backpack an extra effect: at the start of each turn, gain 1 Iron Ingot. Can only be crafted once.         |
| Refined Backpack: Stonecutter Upgrade | Sophisticated Backpack | Grants your Sophisticated Backpack an extra effect: every 3 turns, your Cobblestone is doubled. Can only be crafted once.           |

---

## Power List

### Material Powers (MaterialPower)

| Power       | Type         | Effect                                          |
|-------------|--------------|-------------------------------------------------|
| Plank       | Buff Counter | A piece of processed Plank, usable in crafting. |
| Cobblestone | Buff Counter | Hard Cobblestone, usable in crafting.           |
| Iron Ingot  | Buff Counter | Sturdy Iron Ingot, usable in crafting.          |
| Diamond     | Buff Counter | Precious Diamond, usable in crafting.           |

### Stress

| Power  | Type         | Effect                                                                                                                                                                  |
|--------|--------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Stress | Buff Counter | When you gain materials from cards, only 1 stack is spent for that resolution, doubling the materials gained (powers, relics, and other non-card sources do not count). |

### Turn-Start Triggers

| Power                                      | Type           | Effect                                                                |
|--------------------------------------------|----------------|-----------------------------------------------------------------------|
| Tool: Stone Pickaxe (DiggingPower)         | Buff Counter   | At the start of each turn, gain N Plank and N Cobblestone.            |
| Planting (PlantPower)                      | Buff Counter   | At the start of next turn, gain N Plank, then expires.                |
| Steam (SteamPower)                         | Buff Counter   | At the start of each turn, gain N Stress.                             |
| Auto Crafter (AutoCrafterPower)            | Buff Counter   | At the start of your turn, Craft N times.                             |
| Brush Stone Form (BrushStoneFormPower)     | Buff Counter   | At the start of your turn, gain N Cobblestone; N increases each turn. |
| Repair (RepairPower)                       | Buff Counter   | At the start of next turn, gain N Stress, then expires.               |
| Nuclear Power? Easy Peasy (EasyPeasyPower) | Buff Counter   | At turn start, gain N Energy and draw N cards.                        |
| This Is Bad (RadiationLeakPower)           | Debuff Counter | At turn start, add N Dazed to your hand; gains 1 more each turn.      |

### Equipment Powers

| Power                             | Type           | Effect                                                                                                      |
|-----------------------------------|----------------|-------------------------------------------------------------------------------------------------------------|
| Weapon: Wooden Sword              | Buff Counter   | At the start of each turn, gain 4 Vigor. N turns remaining.                                                 |
| Weapon: Stone Sword               | Debuff Counter | At the start of each turn, lose 2 Strength. N turns remaining.                                              |
| Weapon: Iron Sword                | Buff Counter   | The next N Attack cards are played an additional time.                                                      |
| Weapon: Diamond Sword             | Buff Counter   | Each turn, the first N Attack cards are played an additional time.                                          |
| Weapon: Golden Sword              | Buff Counter   | Every 2 Attack cards you play, gain 1 Energy. All Attack cards gain Ethereal.                               |
| Equipment: Iron Armor             | Buff None      | At the start of your turn, gain N Plating.                                                                  |
| Equipment: Stone Armor            | Debuff Counter | At the start of each turn, lose N Dexterity.                                                                |
| Equipment: Diamond Armor          | Buff None      | If you still have Plating at the end of the turn, retain your Block for this turn.                          |
| Equipment: Golden Armor           | Buff None      | When you lose Buffer, heal HP equal to the damage you would have taken.                                     |
| Equipment: Netherite Chestplate   | Buff None      | When you gain Block, if you have Plating, double the Block you gain.                                        |
| Tool: Iron Pickaxe                | Buff Counter   | When you gain resources from cards, gain 2 additional Iron Ingot. N uses remaining.                         |
| Tool: Golden Pickaxe              | Buff Counter   | Whenever you gain Materials, gain that much Block and deal N times that amount as damage to a random enemy. |
| Tool: Netherite Pickaxe           | Buff Counter   | Each time you play 2 cards in the same turn, gain 1 Plank, Cobblestone, Iron Ingot, and Diamond.            |
| Shield Dexterity (ShieldDexPower) | Buff           | Temporary Dexterity (from Shield).                                                                          |
| Shield Cooldown                   | Debuff Counter | Cannot play Shield. N turns remaining.                                                                      |

### Other Special Powers

| Power                                       | Type           | Effect                                                                                                                                          |
|---------------------------------------------|----------------|-------------------------------------------------------------------------------------------------------------------------------------------------|
| Launch Platform                             | Buff None      | The first time you consume Stress each turn, gain 1 Energy.                                                                                     |
| Liberation                                  | Buff None      | At the end of your turn, trigger every Retained card in your hand.                                                                              |
| Memory                                      | Buff Counter   | At the start of your turn, add an Exhaust copy of the last Skill you played last turn to your hand. Upgraded (Memory+): copy also gains Retain. |
| Otherworld Crossing                         | Buff Counter   | At the start of each turn, choose a card in your hand; add an Ethereal Exhaust copy to your hand.                                               |
| Planning Expert                             | Buff None      | Whenever you play a Skill card, that card gains Retain.                                                                                         |
| Spirit Fox Form                             | Buff None      | Whenever you play an Attack card, apply 1 Slow to the target.                                                                                   |
| Tracking                                    | Buff Counter   | Whenever you cause an enemy to lose HP, that enemy gains 1 Block and you gain N Block.                                                          |
| Highly Focused                              | Buff Counter   | This turn, double your current Strength.                                                                                                        |
| Anticipate: Dexterity                       | Buff Counter   | Gain N Dexterity this turn. Expires at end of turn.                                                                                             |
| Mass Production                             | Buff Counter   | This turn, craft outputs are copied to other players (N cards each). Expires at end of turn.                                                    |
| Production Log                              | Buff Counter   | Cards you temporarily add during combat gain Retain. At end of turn, gain N Block per Retained card.                                            |
| Snowball to the Face                        | Buff Counter   | The next Skill you play this turn costs 0. Expires at end of turn.                                                                              |
| Slash Blade: Nameless Blade "Wooden Puppet" | Buff Counter   | Whenever you play a Skill card, gain N Vigor.                                                                                                   |
| Burning                                     | Debuff Counter | Burning creatures lose HP at the start of their turn. Burning stacks are halved each turn (rounded up).                                         |

---

## Relic List

| Relic                  | Rarity   | Effect                                                                                                                                                                                                                                                                                              |
|------------------------|----------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Hand Crank             | Starter  | At the start of each combat, choose to gain Stress, or gain Plank and Cobblestone.                                                                                                                                                                                                                  |
| Deployer               | Starter  | At the start of each combat, choose to gain Stress, or gain Plank, Cobblestone, and Iron Ingot.                                                                                                                                                                                                     |
| Maid's Backpack        | Uncommon | When picked up, gain 2 Potion Slots and fill every empty slot with a random potion.                                                                                                                                                                                                                 |
| Sophisticated Backpack | Rare     | Provides extra craftable cards: backpack upgrades. Unlock feeding (Regen per turn), smelting (Iron Ingot per turn), crafting bonus (extra Crafts), stonecutter (Cobblestone doubles every N turns), savings (bonus Gold on combat end), and restock (draw cards on combat start) via upgrade cards. |
| Totem of Undying       | Rare     | When you would die, prevent death, heal 35% of your Max HP, gain 10 Regeneration and 15 Block. (Once per run)                                                                                                                                                                                       |

---

## Craft Recipes

| Product            | Materials Required      |
|--------------------|-------------------------|
| Bare-Handed Strike | None                    |
| Wooden Pickaxe     | 4 Plank                 |
| Wooden Sword       | 3 Plank                 |
| Wooden Armor       | 4 Plank                 |
| Stone Pickaxe      | 1 Plank + 3 Cobblestone |
| Stone Sword        | 1 Plank + 2 Cobblestone |
| Stone Brick Armor  | 8 Cobblestone           |
| Iron Pickaxe       | 1 Plank + 3 Iron Ingot  |
| Iron Sword         | 1 Plank + 2 Iron Ingot  |
| Iron Armor         | 8 Iron Ingot            |
| Diamond Pickaxe    | 1 Plank + 3 Diamond     |
| Diamond Sword      | 1 Plank + 2 Diamond     |
| Diamond Armor      | 8 Diamond               |
| Shield             | 6 Plank + 1 Iron Ingot  |

---

## Project Structure

```
STS2_WineFox/
├── Cards/
│   ├── Basic/              # Basic cards (not in reward pool)
│   ├── Common/             # Common reward cards
│   ├── Uncommon/           # Uncommon reward cards
│   ├── Rare/               # Rare reward cards
│   ├── Ancient/            # Ancient reward cards
│   ├── Token/              # Cards generated during combat (autoAdd: false)
│   │   ├── Craft/          # Craft recipe products (swords, pickaxes, armor, shield, etc.)
│   │   ├── HandCrank/      # Hand Crank relic token cards
│   │   ├── HellGift/       # Hell's Gift products (Golden Sword/Pickaxe/Armor)
│   │   ├── LessHoliday/    # 996 product (Work Overtime)
│   │   └── SophisticatedBackpack/ # Sophisticated Backpack upgrade cards
│   ├── CraftRecipeRegistry.cs  # Craft recipe registry
│   ├── WineFoxCard.cs      # Card base class
│   ├── WineFoxCardVarFactory.cs # Card DynamicVar factory (Stress doubling, etc.)
│   └── WineFoxKeywords.cs  # Keyword ID constants
├── Character/
│   ├── WineFox.cs          # Character definition (HP, starting deck, starting relic)
│   ├── WineFoxCardPool.cs  # Card pool (frame color, energy icon)
│   ├── WineFoxRelicPool.cs
│   └── WineFoxPotionPool.cs
├── Commands/
│   ├── CraftCmd.cs         # Craft command (craft selection, material spending, logging)
│   ├── CraftDeliveryMode.cs
│   ├── CraftExecutionContext.cs
│   ├── MaterialCmd.cs      # Material command (gain/spend materials)
│   ├── MaterialEventFlow.cs
│   ├── MaterialPowerRegistry.cs
│   └── StressCmd.cs
├── Content/
│   ├── WineFoxAutoRegistrationKeywords.cs  # Keyword auto-registration ([RegisterOwnedCardKeyword])
│   └── Descriptors/
│       └── WineFoxCharacterAssets.cs       # Character asset registration
├── Enchantments/           # Enchantment effects
├── Epoch/                  # Act-completion Epoch unlocks
├── Events/                 # Events
├── Hooks/                  # Framework hooks
├── Powers/
│   ├── WineFoxPower.cs         # Power base class
│   ├── MaterialPower.cs        # Material Power base class
│   ├── MaterialReactivePower.cs # Material event-reactive Power base class
│   └── ...（individual Power classes）
├── Relics/
│   ├── Backpack/               # SophisticatedBackpack effect modules
│   ├── WineFoxRelic.cs         # Relic base class
│   └── ...（individual relic classes）
├── Screens/                # Custom screens
├── STS2_WineFox/           # Game assets (Godot export directory)
│   ├── cards/              # Card images
│   ├── powers/             # Power icons
│   ├── relics/             # Relic icons
│   └── localization/
│       ├── zhs/            # Simplified Chinese (cards / powers / relics / card_keywords)
│       └── eng/            # English
├── Utils/                  # Utility classes
├── Const.cs                # Global paths and constants
└── Main.cs                 # Mod entry point (assembly scan auto-registration)
```

---

## Development Guide

> This project uses **STS2-RitsuLib**'s attribute-driven auto-registration. On startup, `Main.cs` calls
`ModTypeDiscoveryHub.RegisterModAssembly` to scan the entire assembly. All types annotated with `[RegisterCard]`,
`[RegisterRelic]`, `[RegisterOwnedCardKeyword]`, etc. are registered automatically — **no** manual content manifest file
> is required.

---

### Adding a New Card

| Step | File                          | Action                                                                                      |
|------|-------------------------------|---------------------------------------------------------------------------------------------|
| 1    | `Cards/{Rarity}/XxxCard.cs`   | Create a card class inheriting `WineFoxCard`, add `[RegisterCard(typeof(WineFoxCardPool))]` |
| 2    | `Const.cs`                    | Add `CardXxx = Root + "/cards/card_xxx.png"` to `Paths`                                     |
| 3    | `STS2_WineFox/cards/`         | Place the card image `card_xxx.png`                                                         |
| 4    | `localization/zhs/cards.json` | Add `title` and `description`                                                               |
| 5    | `localization/eng/cards.json` | Same (English)                                                                              |
| 6    | `Character/WineFox.cs`        | To include in the starting deck, add to `StartingDeckTypes` (**Basic cards only**)          |

> **Token cards**: use `[RegisterCard(typeof(WineFoxTokenCardPool))]` and add `showInCardLibrary: false, autoAdd: false`
> to the constructor. No other registration steps needed.

#### Rarity and Reward Pool

| Rarity     | Appears as Reward | Usage                   |
|------------|:-----------------:|-------------------------|
| `Basic`    |         ❌         | Starting deck only      |
| `Common`   |         ✅         | Normal combat reward    |
| `Uncommon` |         ✅         | Elite / event reward    |
| `Rare`     |         ✅         | Rare reward             |
| `Ancient`  |         ✅         | Ancient reward          |
| `Token`    |         ❌         | Generated during combat |

> ⚠️ The reward pool must contain at least one non-blacklisted card of Common rarity or higher, otherwise a crash will
> occur during combat resolution.  
> ⚠️ Cards already in `StartingDeckTypes` are permanently blacklisted by the reward system and cannot be obtained again.

---

### Adding a New Power

| Step | File                           | Action                                                                                        |
|------|--------------------------------|-----------------------------------------------------------------------------------------------|
| 1    | `Powers/XxxPower.cs`           | Create a Power class inheriting `WineFoxPower` (or `MaterialPower` / `MaterialReactivePower`) |
| 2    | `Const.cs`                     | Add `XxxPowerIcon = Root + "/powers/xxx.png"` to `Paths`                                      |
| 3    | `STS2_WineFox/powers/`         | Place the icon image                                                                          |
| 4    | `localization/zhs/powers.json` | Add `title`, `description`, and `smartDescription`                                            |
| 5    | `localization/eng/powers.json` | Same (English)                                                                                |

> Power classes are discovered automatically by the framework (via naming conventions / icon path properties). No manual
> registration required.

#### Power Localization Fields

| Field              | Supports Variables                  | When Displayed               |
|--------------------|-------------------------------------|------------------------------|
| `description`      | ❌ Plain text, use hardcoded numbers | Card library, out-of-combat  |
| `smartDescription` | ✅ `{Amount}`                        | In-combat Power icon tooltip |

#### PowerStackType Reference

| Value     | Meaning                   | Example                  |
|-----------|---------------------------|--------------------------|
| `None`    | No stacking; custom logic | DiamondArmorPower        |
| `Counter` | Stackable counter         | Plank, Strength, Digging |
| `Single`  | Present/absent, no count  | Weak, Disarm             |

---

### Adding a New Keyword (Optional)

| Step | File                                         | Action                                                                                                                    |
|------|----------------------------------------------|---------------------------------------------------------------------------------------------------------------------------|
| 1    | `Cards/WineFoxKeywords.cs`                   | Add `public const string XxxKey = "STS2_WINEFOX-XXX"`                                                                     |
| 2    | `Content/WineFoxAutoRegistrationKeywords.cs` | Add a private `sealed class` with `[RegisterOwnedCardKeyword(WineFoxKeywords.XxxKey, LocKeyPrefix = "STS2_WINEFOX-XXX")]` |
| 3    | `localization/zhs/card_keywords.json`        | Add `title` and `description`                                                                                             |
| 4    | `localization/eng/card_keywords.json`        | Same (English)                                                                                                            |

> ⚠️ Keyword descriptions are **static text** and do not support dynamic variables like `{VarName:diff()}`.  
> To show an icon on the keyword, add `IconPath = Const.Paths.XxxIcon` to `[RegisterOwnedCardKeyword]`.

---

## Localization Notes

### Card Description Dynamic Variables

| Format             | Effect                                                                             |
|--------------------|------------------------------------------------------------------------------------|
| `{VarName}`        | Shows the variable's current value (static)                                        |
| `{VarName:diff()}` | Shows the current value; highlights the diff when upgraded or buffed (recommended) |

| Location                                            | Supports Dynamic Variables |
|-----------------------------------------------------|:--------------------------:|
| `cards.json` description (bound to `CanonicalVars`) |             ✅              |
| `card_keywords.json` keyword tooltip                |             ❌              |
| `powers.json` → `description`                       |             ❌              |
| `powers.json` → `smartDescription`                  |    ✅ (`{Amount}` only)     |

### Common DynamicVar Types

| Class                              | Description Variable | Usage         |
|------------------------------------|----------------------|---------------|
| `DamageVar(value, ValueProp.Move)` | `{Damage:diff()}`    | Attack damage |
| `BlockVar(value, ValueProp.Move)`  | `{Block:diff()}`     | Block         |
| `PowerVar<T>("Name", value)`       | `{Name:diff()}`      | Power stacks  |
| `new("Name", value)`               | `{Name:diff()}`      | Custom value  |
| `CardsVar(value)`                  | `{Cards:diff()}`     | Cards drawn   |
| `EnergyVar(value)`                 | `{Energy:diff()}`    | Cost / Energy |

---

## Environment Setup

| Dependency       | Path                                                       |
|------------------|------------------------------------------------------------|
| Slay the Spire 2 | `G:\SteamLibrary\steamapps\common\Slay the Spire 2`        |
| STS2-RitsuLib    | Same parent directory as this project (`..\STS2-RitsuLib`) |
| Godot            | `4.5.1 Mono`                                               |
| .NET             | `net10.0`                                                  |

After building, the mod is automatically copied to `{STS2Dir}\mods\STS2_WineFox\`.

## Authors

- Programmer: OLC, 灯火橘

- Numerical Planner, Card Design: IAmNotFood, 明风OuO, 晓月漓风, 末影酱安德Channel

- Artist: 穰雀, Linger铃

## Acknowledgements

- 酒石酸菌 created the cute Wine Fox