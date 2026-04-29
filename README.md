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
  <div>
    <b>简体中文</b> | <a href="README_EN.md">English</a>
  </div>
</div>

WineFox（酒狐）是一个出自于Minecraft模组车万女仆的角色，现在作为杀戮尖塔2的一个自定义角色Mod，围绕**材料合成**与**资源管理**的玩法设计。通过获取木板、圆石、铁锭、钻石等材料，利用合成、应力等机制打出强力组合。

---

## 角色概览

| 属性 | 值 |
|------|----|
| 初始 HP | 80 |
| 初始金币 | 99 |
| 主题色 | `#ffaf50`（橙色） |
| 初始遗物 | 手摇曲柄（HandCrank） |

### 初始牌组

| 卡牌 | 数量 |
|------|------|
| 打击（WineFoxStrike） | 4 |
| 防御（WineFoxDefend） | 4 |
| 基础采掘（BasicMine） | 1 |
| 基础合成（BasicCraft） | 1 |

---

## 安装指南

1. 在**Releases**下载编译好的酒狐mod，里面已经包含了对应版本的**RitsuLib**前置。如果需要下载其他版本的前置请到[RitsuLib仓库](https://github.com/BAKAOLC/STS2-RitsuLib)下载。
2. 打开游戏根目录(即Slay the Spire 2文件夹)，创建mods文件夹，注意mods都是小写，然后把下载好的压缩包解压到mods文件夹内。
3. 运行游戏，并开启mod。注意：第一次使用mod时，游戏会自动将无mod存档隔离。



## 卡牌列表

### Basic（初始牌组）

| 卡牌 | 费用 | 类型 | 效果 | 升级 |
|------|------|------|------|------|
| WineFoxStrike（打击） | 1 | 攻击 | 造成 6 点伤害。 | 伤害→9 |
| WineFoxDefend（防御） | 1 | 技能 | 获得 5 点格挡。 | 格挡→8 |
| BasicMine（基础采掘） | 1 | 技能 | 获得 2 木板 和 2 圆石。 | 各+1 |
| BasicCraft（基础合成） | 1 | 技能 | **合成**。获得 5 点格挡。**保留** | 费用→0 |

### Common（普通）

| 卡牌                       | 费用 | 类型 | 效果 | 升级 |
|--------------------------|------|------|------|------|
| BackupCrafting（备用工作台）    | 0 | 技能 | **消耗**；**合成**。 | +保留 |
| BarrierWave（屏障波）         | 1 | 技能 | 对所有敌人造成 6 点不可格挡伤害，获得 5 点格挡。 | 伤害→9；格挡→7 |
| ChangeEquipment（更换装备）    | 0 | 技能 | 抽 2 张牌，选择手牌中 2 张牌洗回抽牌堆。 | 各+1 |
| FullForceCollision（全力冲撞） | 1 | 攻击 | 对所有敌人造成 12 点伤害，将一张晕眩洗入抽牌堆。 | 伤害→16 |
| GettingWood（获得木头）        | 1 | 攻击 | 获得 4 个木板。对敌人造成等同于本次获得木板数的伤害。 | 木板→6 |
| ImprovisedWeapon（临时武器）   | 1 | 攻击 | 造成 8 点伤害，消耗一张牌。 | 伤害→12 |
| IronZombie（"白色僵尸"）       | 1 | 攻击 | 造成 8 点伤害，获得 1 个铁锭。 | 伤害+3；铁锭+1 |
| LightAssault（轻装突击）       | 0 | 攻击 | 造成伤害。你每有 2 份材料，这张牌的伤害就降低 1 点。 | 基础伤害↑ |
| MagicMissile（魔法飞弹）       | 1 | 技能 | 使一名敌人失去 6 点生命，共 2 次。 | 每次→8 |
| MechanicalDrill（动力钻头）    | 2 | 技能 | 造成伤害，共数次。获得 2 个铁锭。如果花费了应力，返还能量。 | 铁锭→3 |
| PlantTrees（种植树木）         | 1 | 技能 | 获得 5 点格挡。在下回合开始时获得 4 个木板。 | 格挡→7；木板→6 |
| PowerUp（换挡，加速，起飞）        | 1 | 技能 | 获得 1 层应力。抽 1 张牌。 | 摸→2 |
| PreProcessing（预处理）       | 0 | 技能 | **先手**；**消耗**；获得 3 个木板与 3 个圆石。 | 另获得 3 格挡 |
| QuickAttack（快速攻击）        | 1 | 攻击 | **虚无**；造成伤害数次。 | 移除虚无 |
| QuickShelter（速成掩体）       | 1 | 技能 | **虚无**；获得木板和圆石。获得格挡。抽 1 张牌。 | 格挡↑ |
| Regroup（重整旗鼓）            | 1 | 技能 | **消耗**；抽 1 张牌。将弃牌堆中的 1 张牌放入你的手牌。 | 移除消耗 |
| TicTacToeGrid（井字格）       | 1 | 攻击 | 造成 8 点伤害。**合成**。 | 伤害→11 |
| Traditionalist（传统派）      | 0 | 技能 | 花费 2 木板。**合成**。抽 1 张牌。 | 摸牌→2 |
| VacantDomain（空置域）        | 3 | 技能 | 获得 14 点格挡。获得 8 圆石。 | 格挡→18；圆石→12 |
| WindCrank（摇曲柄）           | 1 | 技能 | 获得 3 层应力，结束回合。 | 应力→4 |

### Uncommon（罕见）

| 卡牌                          | 费用 | 类型 | 效果                                                           | 升级 |
|-----------------------------|------|------|--------------------------------------------------------------|------|
| Alternator（交流发电机）           | 0 | 技能 | 获得能量。消耗 1 层应力，额外获得能量。 | 额外→更多 |
| AlterPath（另辟蹊径）             | 0 | 技能 | 花费 2 木板，获得 5 点格挡，抽 2 张牌。 | 格挡→8；摸→3 |
| AnticipateAdvantage（预判先机）   | 1 | 技能 | 如果一名敌人的意图为攻击，你就获得 3 点敏捷。 | 敏捷→5 |
| BlueprintPrinting（蓝图打印）     | 1 | 技能 | **消耗**；花费木板和原石，选择手牌 1 张卡，将其数张费用-1、带有消耗的复制品加入手牌。 | 复制→3 张 |
| Bucket（水桶）                  | 1 | 技能 | **消耗**；获得 8 点格挡。下回合开始时再获得 8 点格挡。对所有单位施加 2 层虚弱。 | 格挡→11 |
| CrushingWheel（粉碎轮组）         | 3 | 攻击 | 对所有敌人造成伤害。每在本回合花费过一次材料，这张牌的费用就 -1。 | — |
| DripstoneTrap（石锥陷阱）         | 2 | 攻击 | 消耗所有圆石，每消耗一个就对随机敌人造成伤害，施加 1 层易伤。 | — |
| EmergencyRepair（紧急修复）       | 1 | 攻击 | 造成伤害，获得格挡，在下个回合开始时获得 2 应力。 | 各+2 |
| Erosion（溃蚀）                 | 1 | 技能 | 给予一名敌人 6 层消亡。所有拥有消亡的敌人失去等同于其层数的生命。 | 消亡→10 |
| FoxBite（狐咬）                 | 2 | 技能 | **保留**；给予 4 层虚弱。给予 4 层易伤。 | 各→7 |
| FullAttack（全力倾泻）            | 2 | 攻击 | 花费所有材料。每花费 1 个材料，对目标造成伤害。 | +保留 |
| HammerStrike（重锤下砸）          | 4 | 攻击 | **保留**；每次被保留时费用 -1。造成伤害，力量效果的伤害加成变为 3 倍。 | — |
| HighlyFocused（高度专注）         | 1 | 技能 | 在本回合翻倍你已有的力量。 | 费用→0 |
| IntermittentChanting（间隙咏唱）  | 1 | 法术 | 获得 1 层虚弱。每当你使敌人失去生命时，该敌人获得 1 点格挡，你获得 2 点格挡。 | 你获得→3 |
| KaleidoscopePot（森罗物语：炒锅）   | 1 | 技能 | 抽 1 张牌。选择 1 张手牌变化。 | — |
| LaunchPlatform（弹射置物台）       | 1 | 法术 | 每回合第一次消耗应力时，获得 1 费。 | 费用→0 |
| MassProduction（量产）          | 2 | 技能 | **仅多人**；当你在本回合合成时，将 1 张合成物的复制品分给你的每个队友。 | 费用→1 |
| MechanicalSaw（动力锯）          | 1 | 攻击 | 对所有敌人造成伤害。消耗 1 应力，额外造成更多伤害。 | — |
| Memory（回忆）                  | 1 | 法术 | 在你的回合开始时，将你上一回合最后打出的技能牌的一张带有消耗的复制品加入你的手牌。 | 复制品附加保留 |
| OtherworldCrossing（异界跨越）    | 1 | 法术 | 每回合开始时，选择一张手牌，将其一张虚无、消耗的复制品加入到你的手牌。 | 附加固有 |
| PetrificationSpell（石化术）     | 1 | 技能 | 获得 4 个圆石。使所有敌人失去等同于获得圆石数量的生命。 | 圆石→6 |
| PressWToThink（「W」思索）        | 1 | 技能 | **消耗**；从抽牌堆顶的 3 张牌中选择一张加入你的手牌，使其获得重放和消耗。 | 费用→0 |
| ProductionDocument（生产记录）    | 1 | 法术 | 你在战斗中临时加入的牌获得保留。回合结束时每保留 1 张牌，你就获得格挡。 | 费用→0 |
| Recuperate（休养生息）            | 1 | 技能 | **消耗**；抽 5 张牌。丢弃手中所有攻击牌。 | 同时丢弃诅咒、任务和状态牌 |
| RedemptionStrike（救赎打击）      | 1 | 攻击 | 造成伤害。治疗所有玩家生命值。 | 治愈↑ |
| RiclearPowerPlant（河电站）      | 2 | 技能 | 获得格挡，获得 2 应力。 | 格挡→16 |
| Scratch（抓挠）                 | X | 攻击 | 造成 X 点伤害，共 2X 次。 | 每次→X+2 |
| SlashBladeWood（拔刀剑:无铭刀「木偶」） | 1 | 法术 | 每当你打出一张技能牌，获得 2 层活力。 | 层数→3 |
| SnowBallOverwhelming（雪球糊脸）  | 1 | 攻击 | 造成伤害。本回合打出的下一张技能牌费用变为 0。 | 造成伤害两次 |
| StressResponse（应激）          | 0 | 技能 | **消耗**；失去 6 点生命值，获得 1 点最大生命值，获得能量并抽 2 张牌。 | 获得→3 费 |
| SweetDream（甜蜜的梦）            | 0 | 技能 | **消耗**；将所有手牌洗入抽牌堆，抽取等量的牌。 | 移除消耗 |
| WorkbenchBackpack（工作台背包）    | 2 | 技能 | 获得 1 费。合成。将这张牌的费用减少 1。 | 获得→2 费 |

### Rare（稀有）

| 卡牌                         | 费用 | 类型 | 效果                                                | 升级 |
|----------------------------|------|------|---------------------------------------------------|------|
| AllItem（全物品仓库）             | 0 | 技能 | **消耗**；获得 1 费；抽 1 张牌；获得 1 木板、1 圆石、1 铁锭、1 钻石。      | +先手 |
| AutoCrafter（自动合成器）         | 1 | 法术 | 当你的回合开始时，合成。                                  | +先手 |
| BatchCraft（批量合成）           | X | 技能 | 合成。本次合成将获得 X 张牌。                             | X+1 张 |
| CobblestoneGenerator（刷石机）  | 2 | 法术 | **虚无**；在你的回合开始时获得 2 圆石，每回合增加 2。                   | 费用→1 |
| EasyPeasy（轻而易举）            | 0 | 法术 | 每回合开始时获得 1 费，额外抽 1 张牌。每回合开始时将 1 张晕眩加入你的手牌，每回合增加 1。 | N+1 |
| ExplosionMagic（爆裂魔法）       | 2 | 技能 | 给予所有敌人 12 层烧伤。失去 3 点敏捷。                          | 烧伤→15 |
| HellGift（地狱馈赠）             | 2 | 技能 | **消耗**；将一张随机金质装备卡牌加入你的手牌。                         | 牌已升级 |
| LessHoliday（996）           | 0 | 技能 | 选择你手牌中的最多 3 张牌变化为加班加点。                            | 生成加班加点+ |
| Liberation（解放）             | 2 | 法术 | 回合结束时，触发你手牌中每张被保留的牌。                              | 费用→1 |
| MaidSupport（女仆的援护）         | 2 | 技能 | **仅多人**；为所有玩家添加 5 层覆甲，2 点荆棘。                      | 各+1 |
| Milk（牛奶）                   | 1 | 技能 | **消耗**；消除你身上的所有负面效果。                              | 移除消耗 |
| MiningGems（挖掘宝石）           | 2 | 技能 | 获得 2 个钻石。                                         | 钻石→3 |
| NetheriteChestPlate（下界合金甲） | 2 | 法术 | 获得覆甲。翻倍你从覆甲获得的格挡。                                | 覆甲↑ |
| NoMoreFalchion（时代变了）       | 2 | 攻击 | 造成伤害数次。花费 1 个铁锭，使这张牌在本场战斗中额外造成一次伤害。将这张牌返回你的手牌。  | 升级版 |
| PaybackTime（清算时间）          | 3 | 攻击 | 造成基础伤害。在本场对战中你每获得一份材料，额外造成伤害。                    | 费用→2 |
| PlanningExpert（规划专家）       | 2 | 法术 | 每当你打出一张技能牌时，该牌获得保留。                               | 费用→1 |
| ShieldAttack（盾击）           | 0 | 攻击 | 对所有敌人造成倍于你当前格挡的伤害。清除你的所有格挡。抽 1 张牌。             | — |
| SpinningHand（飞转的手）         | 4 | 攻击 | 对所有敌人造成伤害，本场战斗中每合成一次，这张牌的费用就减少 1。            | — |
| SpiritFoxForm（灵狐形态）        | 3 | 法术 | **虚无**；每当你打出一张攻击牌时，对目标施加 1 层缓慢。                   | 移除虚无 |
| SteamEngine（蒸汽引擎）          | 2 | 法术 | 每回合开始时获得 1 应力。                                    | 费用→1 |
| WirelessTerminal（无线合成终端）   | 1 | 技能 | 合成。将这张牌返回你的手牌。                                   | +保留 |

### Ancient（先古之民）

| 卡牌 | 费用 | 类型 | 效果 | 升级 |
|------|------|------|------|------|
| Forging（锻造） | 1 | 技能 | **保留**；获得格挡。合成两次。 | 格挡↑；费→0 |
| NetheritePickaxe（下界合金镐） | 1 | 法术 | 在同一个回合内每打出 2 张牌，就获得木板、圆石、铁锭和钻石各 1 个。 | +先手 |

### Token（战斗生成）

Token 牌由合成配方或特定卡牌在战斗中生成，不进入奖励池。

#### 合成配方产物

| 卡牌 | 合成配方 | 效果                                           |
|------|---------|----------------------------------------------|
| Nothing（空手打击） | 无需材料 | **消耗**；造成 1 点伤害。                             |
| WoodenSword（木剑） | 3 木板 | **消耗**；造成 6 点伤害；获得 4 层活力。                    |
| StoneSword（石剑） | 1 木板 + 2 圆石 | **消耗**；造成 9 点伤害；获得 2 层力量。                    |
| IronSword（铁剑） | 1 木板 + 2 铁锭 | **消耗**；造成 14 点伤害；下 1 张攻击牌额外打出一次。             |
| DiamondSword（钻石剑） | 1 木板 + 2 钻石 | **消耗**；造成 20 点伤害；每回合前 1 张攻击牌额外打出一次。          |
| WoodenPickaxe（木镐） | 4 木板 | **消耗**；下个回合开始时，获得 2 费，抽 1 张牌。                |
| StonePickaxe（石镐） | 1 木板 + 3 圆石 | **消耗**；每回合开始时，获得 1 个木板和 1 个圆石。               |
| IronPickaxe（铁镐） | 1 木板 + 3 铁锭 | **消耗**；接下来数次获得资源时，额外获得 3 个铁锭。 |
| DiamondPickaxe（钻石镐） | 1 木板 + 3 钻石 | **消耗**；当你合成一张牌时，将其升级。升级后：合成。 |
| WoodenArmor（木甲） | 4 木板 | **消耗**；获得格挡。 |
| StoneArmor（砖石甲） | 8 圆石 | **消耗**；获得覆甲，每回合开始时失去 1 点敏捷。 |
| IronArmor（铁甲） | 8 铁锭 | **消耗**；获得覆甲。你的覆甲层数不会降低。 |
| DiamondArmor（钻石甲） | 8 钻石 | **消耗**；获得覆甲。如果回合结束时你还有覆甲，就在本回合保留你的格挡。 |
| Shield（盾牌） | 6 木板 + 1 铁锭 | **消耗**；选择一名角色在本回合获得敏捷。此牌在本回合与下一回合不能再次打出。升级后：仅本回合不能再次打出。 |

#### 其他 Token

| 卡牌 | 来源 | 效果 |
|------|------|------|
| GoldenSword（金剑） | HellGift | 你每打出两张攻击牌就获得 1 费，所有攻击牌获得虚无。升级后：移除虚无效果。 |
| GoldenPickaxe（金镐） | HellGift | 每次你获得材料，就获得等量格挡并对随机敌人造成 2 倍等量伤害。升级后：3 倍。 |
| GoldenArmor（金甲） | HellGift | **消耗**；获得缓冲，1 层人工制品。当你失去缓冲时，回复等同于你应受到伤害的生命值。 |
| WorkWork（加班加点） | LessHoliday | **消耗**；获得 1 应力。升级后：额外抽 1 张牌。 |
| SteelChamber（钢铁枪膛） | NoMoreFalchion | 造成伤害 N 次。将这张牌返回你的手牌。 |
| CraftUpgrade（精巧背包:合成升级） | SophisticatedBackpack | 给精妙背包提供额外效果：每合成 2 次，额外进行 1 次合成。只能合成一次。 |
| FeedingUpgrade（精巧背包:喂食升级） | SophisticatedBackpack | 给精妙背包提供额外效果：每回合开始时，获得 1 再生。只能合成一次。 |
| RestockUpgrade（精巧背包:补货升级） | SophisticatedBackpack | 给精妙背包提供额外效果：每场战斗开始时，额外抽两张牌。只能合成一次。 |
| SavingsUpgrade（精巧背包:储蓄升级） | SophisticatedBackpack | 给精妙背包提供额外效果：战斗结束时，额外获得 15 金币。只能合成一次。 |
| SmeltingUpgrade（精巧背包:熔炼升级） | SophisticatedBackpack | 给精妙背包提供额外效果：每回合开始时，获得 1 个铁锭。只能合成一次。 |
| StonecutterUpgrade（精巧背包:切石机升级） | SophisticatedBackpack | 给精妙背包提供额外效果：每三个回合将你的圆石翻倍。只能合成一次。 |

---

## Power 列表

### 材料 Power（MaterialPower）

| Power | 类型 | 效果 |
|-------|------|------|
| WoodPower（木板） | Buff Counter | 一块处理过的木板，可用于合成。 |
| StonePower（圆石） | Buff Counter | 坚硬的圆石，可用于合成。 |
| IronPower（铁锭） | Buff Counter | 坚固的铁锭，可用于合成。 |
| DiamondPower（钻石） | Buff Counter | 珍贵的钻石，可用于合成。 |

### 应力相关

| Power | 类型 | 效果 |
|-------|------|------|
| StressPower（应力） | Buff Counter | 从卡牌获得材料时，同一次结算只花费 1 层，使该次获得的材料数量翻倍（能力、遗物等途径不计入）。 |

### 回合开始触发

| Power                    | 类型 | 效果 |
|--------------------------|------|------|
| DiggingPower（工具：石镐）      | Buff Counter | 每回合开始时，获得 N 个木板和 N 个圆石。 |
| PlantPower（种植）           | Buff Counter | 下回合开始时，获得 N 木板，触发后消除。 |
| SteamPower（蒸汽）           | Buff Counter | 每回合开始时，获得 N 应力。 |
| AutoCrafterPower（自动合成器）  | Buff Counter | 当你的回合开始时，合成 N 次。 |
| BrushStoneFormPower（刷石机） | Buff Counter | 在你的回合开始时获得 N 圆石，每回合层数增加（N 递增）。 |
| RepairPower（修复）          | Buff Counter | 下回合开始时，获得 N 层应力，消除。 |
| EasyPeasyPower（核电，轻而易举啊） | Buff Counter | 回合开始时，获得 N 费用，抽 N 张卡。 |
| RadiationLeakPower（坏了坏了） | Debuff Counter | 回合开始时，将 N 张晕眩加入你的手牌，每回合增加 1 层。 |

### 装备效果 Power

| Power | 类型 | 效果                                  |
|-------|------|-------------------------------------|
| WoodenSwordPower（武器:木剑） | Buff Counter | 每回合开始时获得 4 层活力。还剩 N 回合。             |
| StoneSwordPower（武器:石剑） | Debuff Counter | 每回合开始时，失去 2 点力量。还剩 N 回合。            |
| IronSwordPower（武器：铁剑） | Buff Counter | 接下来 N 张攻击牌额外打出一次。                   |
| DiamondSwordPower（武器：钻石剑） | Buff Counter | 每回合前 N 张攻击牌额外打出一次。                  |
| GoldenSwordPower（武器：金剑） | Buff Counter | 你每打出两张攻击牌就获得 1 费，所有攻击牌获得虚无。         |
| IronArmorPower（装备：铁甲） | Buff None | 你的覆甲层数不会降低。 |
| StoneArmorPower（装备：砖石甲） | Debuff Counter | 每回合开始时，失去 N 点敏捷。 |
| DiamondArmorPower（装备：钻石甲） | Buff None | 如果回合结束时你还有覆甲，就在本回合保留你的格挡。 |
| GoldenArmorPower（装备：金甲） | Buff None | 当你失去缓冲时，回复等同于你应受到伤害的生命值。 |
| NetheriteChestPlatePower（装备：下界合金甲） | Buff None | 翻倍你从覆甲获得的格挡。 |
| IronPickaxePower（工具：铁镐） | Buff Counter | 从牌中获得资源时，额外获得 2 个铁锭。还剩 N 次。 |
| GoldenPickaxePower（工具：金镐） | Buff Counter | 每次你获得材料，就获得等量格挡并对随机敌人造成 N 倍等量伤害。    |
| NetheritePickaxePower（工具：下界合金镐） | Buff Counter | 每在同一个回合内打出 2 张牌，就获得木板、石头、铁、钻石各 1 个。 |
| ShieldDexPower（盾牌敏捷） | Buff | 临时敏捷（来自盾牌）。                         |
| ShieldCooldownPower（盾牌冷却） | Debuff Counter | 无法打出盾牌。还剩 N 回合。                     |

### 其他特殊 Power

| Power | 类型 | 效果 |
|-------|------|------|
| LaunchPlatformPower（弹射置物台） | Buff None | 每回合第一次消耗应力时，获得 1 点能量。 |
| LiberationPower（解放） | Buff None | 回合结束时，触发你手牌中每张被保留的牌。 |
| MemoryPower（回忆） | Buff Counter | 在你的回合开始时，将上一回合最后打出的技能牌的一张带有消耗的复制品加入你的手牌。升级（MemoryPower+）：复制品附加保留。 |
| OtherworldCrossingPower（异界跨越） | Buff Counter | 每回合开始时，选择一张手牌，将其一张虚无、消耗的复制品加入到你的手牌。 |
| PlanningExpertPower（规划专家） | Buff None | 每当你打出一张技能牌时，该牌获得保留。 |
| SpiritFoxFormPower（灵狐形态） | Buff None | 每当你打出一张攻击牌时，对目标施加 1 层缓慢。 |
| TrackingPower（间隙咏唱） | Buff Counter | 每当你使敌人失去生命时，该敌人获得 1 点格挡，你获得 N 点格挡。 |
| HighlyFocusedPower（高度专注） | Buff Counter | 在本回合翻倍你已有的力量。 |
| AnticipateAdvantageDexPower（预判先机） | Buff Counter | 本回合获得 N 点敏捷，回合结束时消失。 |
| MassProductionPower（量产） | Buff Counter | 本回合中的合成产物将复制给其他玩家 N 张。回合结束消除。 |
| ProductionDocumentPower（生产记录） | Buff Counter | 你在战斗中临时加入的牌获得保留。回合结束时每保留 1 张牌，你就获得 N 点格挡。 |
| SnowBallOverwhelmingPower（雪球糊脸） | Buff Counter | 本回合打出的下一张技能牌费用变为 0。每出 1 张技能消耗 1 层；回合结束消除。 |
| SlashBladeWoodPower（拔刀剑:无铭刀「木偶」） | Buff Counter | 每当你打出一张技能牌，获得 N 层活力。 |
| BurningPower（烧伤） | Debuff Counter | 烧伤的生物会在自身回合开始时失去生命。烧伤层数每回合减半（向上取整）。 |

---

## 遗物列表

| 遗物 | 稀有度 | 效果 |
|------|--------|------|
| HandCrank（手摇曲柄） | Starter | 在每场战斗开始时，选择获得应力，或获得木板和圆石。 |
| Deployer（神之手） | Starter | 在每场战斗开始时，选择获得应力，或获得木板、圆石和铁锭。 |
| MaidBackpack（女仆背包） | Uncommon | 拾取时获得 2 个药水槽位并将所有空槽位填满随机药水。 |
| SophisticatedBackpack（精妙背包） | Rare | 提供额外的合成卡牌：背包升级。可通过升级卡解锁喂食（每回合回血）、熔炼（每回合获得铁锭）、合成触发、切石机（定期圆石翻倍）、储蓄（战斗结束获金币）、补货（战斗开始抽牌）等效果。 |
| TotemofUndying（不死图腾） | Rare | 当你要被杀死时，免死并回复最大生命值的 35%，获得 10 层再生和 15 点格挡。（仅一次） |

---

## 合成配方

| 产物 | 所需材料 |
|------|---------|
| 空手打击 | 无 |
| 木镐 | 4 木板 |
| 木剑 | 3 木板 |
| 木甲 | 4 木板 |
| 石镐 | 1 木板 + 3 圆石 |
| 石剑 | 1 木板 + 2 圆石 |
| 砖石甲 | 8 圆石 |
| 铁镐 | 1 木板 + 3 铁锭 |
| 铁剑 | 1 木板 + 2 铁锭 |
| 铁甲 | 8 铁锭 |
| 钻石镐 | 1 木板 + 3 钻石 |
| 钻石剑 | 1 木板 + 2 钻石 |
| 钻石甲 | 8 钻石 |
| 盾牌 | 6 木板 + 1 铁锭 |

---

## 项目结构

```
STS2_WineFox/
├── Cards/
│   ├── Basic/              # 基础卡（不进入奖励池）
│   ├── Common/             # 普通奖励卡
│   ├── Uncommon/           # 非普通奖励卡
│   ├── Rare/               # 稀有奖励卡
│   ├── Ancient/            # 古代奖励卡
│   ├── Token/              # 战斗中临时生成的卡（autoAdd: false）
│   │   ├── Craft/          # 合成配方产物（剑/镐/甲/盾等）
│   │   ├── HandCrank/      # 手摇曲柄遗物相关 Token
│   │   ├── HellGift/       # 地狱馈赠产物（金剑/金镐/金甲）
│   │   ├── LessHoliday/    # 996 产物（加班加点）
│   │   └── SophisticatedBackpack/ # 精密背包升级卡
│   ├── CraftRecipeRegistry.cs  # 合成配方注册表
│   ├── WineFoxCard.cs      # 卡牌基类
│   ├── WineFoxCardVarFactory.cs # 卡牌 DynamicVar 工厂（应力翻倍等）
│   └── WineFoxKeywords.cs  # 关键字 ID 常量
├── Character/
│   ├── WineFox.cs          # 角色定义（HP、初始牌组、初始遗物）
│   ├── WineFoxCardPool.cs  # 卡池（外框颜色、能量图标）
│   ├── WineFoxRelicPool.cs
│   └── WineFoxPotionPool.cs
├── Commands/
│   ├── CraftCmd.cs         # 合成命令（合成选择、材料消耗、记录）
│   ├── CraftDeliveryMode.cs
│   ├── CraftExecutionContext.cs
│   ├── MaterialCmd.cs      # 材料命令（获得/消耗材料）
│   ├── MaterialEventFlow.cs
│   ├── MaterialPowerRegistry.cs
│   └── StressCmd.cs
├── Content/
│   ├── WineFoxAutoRegistrationKeywords.cs  # 关键字自动注册（[RegisterOwnedCardKeyword]）
│   └── Descriptors/
│       └── WineFoxCharacterAssets.cs       # 角色资源注册
├── Enchantments/           # 附魔效果
├── Epoch/                  # 行动完成 Epoch 解锁
├── Events/                 # 事件
├── Hooks/                  # 框架钩子
├── Powers/
│   ├── WineFoxPower.cs         # Power 基类
│   ├── MaterialPower.cs        # 材料 Power 基类
│   ├── MaterialReactivePower.cs # 材料事件响应 Power 基类
│   └── ...（各 Power 类）
├── Relics/
│   ├── Backpack/               # SophisticatedBackpack 效果模块
│   ├── WineFoxRelic.cs         # 遗物基类
│   └── ...（各遗物类）
├── Screens/                # 自定义界面
├── STS2_WineFox/           # 游戏资源（Godot 导出目录）
│   ├── cards/              # 卡牌图片
│   ├── powers/             # Power 图标
│   ├── relics/             # 遗物图标
│   └── localization/
│       ├── zhs/            # 简体中文（cards / powers / relics / card_keywords）
│       └── eng/            # 英文
├── Utils/                  # 工具类
├── Const.cs                # 全局路径与常量
└── Main.cs                 # Mod 入口（程序集扫描自动注册）
```

---

## 开发指南

> 本项目使用 **STS2-RitsuLib** 的属性驱动自动注册机制。`Main.cs` 启动时会调用 `ModTypeDiscoveryHub.RegisterModAssembly` 扫描整个程序集，所有标注了 `[RegisterCard]`、`[RegisterRelic]`、`[RegisterOwnedCardKeyword]` 等特性的类型将自动完成注册，**无需**手动维护任何内容清单文件。

---

### 添加新卡牌

| 步骤 | 文件 | 操作 |
|------|------|------|
| 1 | `Cards/{Rarity}/XxxCard.cs` | 创建卡牌类，继承 `WineFoxCard`，并在类上添加 `[RegisterCard(typeof(WineFoxCardPool))]` 特性 |
| 2 | `Const.cs` | `Paths` 中添加 `CardXxx = Root + "/cards/card_xxx.png"` |
| 3 | `STS2_WineFox/cards/` | 放入卡牌图片 `card_xxx.png` |
| 4 | `localization/zhs/cards.json` | 添加 `title` 和 `description` |
| 5 | `localization/eng/cards.json` | 同上（英文） |
| 6 | `Character/WineFox.cs` | 若需加入初始牌组，在 `StartingDeckTypes` 中添加（**仅限 Basic 牌**） |

> **Token 卡**额外注意：改用 `[RegisterCard(typeof(WineFoxTokenCardPool))]` 特性，并在构造函数参数中加 `showInCardLibrary: false, autoAdd: false`，无需其他额外注册步骤。

#### 稀有度与奖励池

| 稀有度 | 可作为战斗奖励 | 用途 |
|--------|:------------:|------|
| `Basic` | ❌ | 初始牌组专用 |
| `Common` | ✅ | 普通战斗奖励 |
| `Uncommon` | ✅ | 精英/事件奖励 |
| `Rare` | ✅ | 稀有奖励 |
| `Ancient` | ✅ | 古代奖励 |
| `Token` | ❌ | 战斗中临时生成 |

> ⚠️ 奖励池中至少需要一张 Common 或更高稀有度的非黑名单卡，否则战斗结算时会崩溃。  
> ⚠️ 已在初始牌组（`StartingDeckTypes`）中的卡会被奖励系统永久列入黑名单，无法再次获得。

---

### 添加新 Power

| 步骤 | 文件 | 操作 |
|------|------|------|
| 1 | `Powers/XxxPower.cs` | 创建 Power 类，继承 `WineFoxPower`（或 `MaterialPower` / `MaterialReactivePower`） |
| 2 | `Const.cs` | `Paths` 中添加 `XxxPowerIcon = Root + "/powers/xxx.png"` |
| 3 | `STS2_WineFox/powers/` | 放入图标图片 |
| 4 | `localization/zhs/powers.json` | 添加 `title`、`description`、`smartDescription` |
| 5 | `localization/eng/powers.json` | 同上（英文） |

> Power 类通过框架约定（命名 / 图标路径属性）自动被发现，无需手动注册。

#### Power 本地化字段说明

| 字段 | 支持变量 | 显示时机 |
|------|:-------:|---------|
| `description` | ❌ 纯文本，用硬编码数字 | 卡牌库、非战斗状态 |
| `smartDescription` | ✅ `{Amount}` | 战斗中 Power 图标悬浮提示 |

#### PowerStackType 说明

| 值 | 含义 | 示例 |
|----|------|------|
| `None` | 不叠加，特殊逻辑用 | DiamondArmorPower |
| `Counter` | 可叠加计数 | 木板、力量、挖掘 |
| `Single` | 仅有/无，不计层数 | 脆弱、缴械 |

---

### 添加新关键字（可选）

| 步骤 | 文件 | 操作 |
|------|------|------|
| 1 | `Cards/WineFoxKeywords.cs` | 添加 `public const string XxxKey = "STS2_WINEFOX-XXX"` |
| 2 | `Content/WineFoxAutoRegistrationKeywords.cs` | 在类内部新增一个带 `[RegisterOwnedCardKeyword(WineFoxKeywords.XxxKey, LocKeyPrefix = "STS2_WINEFOX-XXX")]` 特性的私有 `sealed class` |
| 3 | `localization/zhs/card_keywords.json` | 添加 `title` 和 `description` |
| 4 | `localization/eng/card_keywords.json` | 同上（英文） |

> ⚠️ 关键字描述是**纯静态文本**，不支持 `{VarName:diff()}` 等动态变量。  
> 若关键字需要显示图标，在 `[RegisterOwnedCardKeyword]` 中补充 `IconPath = Const.Paths.XxxIcon`。

---

### 本地化说明

#### 卡牌描述动态变量

| 格式 | 效果 |
|------|------|
| `{VarName}` | 显示变量当前值（静态） |
| `{VarName:diff()}` | 显示当前值，升级或受加成时高亮显示差异（推荐） |

| 位置 | 支持动态变量 |
|------|:-----------:|
| `cards.json` 卡牌描述（绑定 `CanonicalVars`） | ✅ |
| `card_keywords.json` 关键字悬浮描述 | ❌ |
| `powers.json` → `description` | ❌ |
| `powers.json` → `smartDescription` | ✅（仅 `{Amount}`） |

#### 常用 DynamicVar 类型

| 类 | 描述变量 | 用途 |
|----|---------|------|
| `DamageVar(value, ValueProp.Move)` | `{Damage:diff()}` | 攻击伤害 |
| `BlockVar(value, ValueProp.Move)` | `{Block:diff()}` | 格挡 |
| `PowerVar<T>("Name", value)` | `{Name:diff()}` | Power 层数 |
| `new("Name", value)` | `{Name:diff()}` | 自定义数值 |
| `CardsVar(value)` | `{Cards:diff()}` | 摸牌数 |
| `EnergyVar(value)` | `{Energy:diff()}` | 费用/能量 |

---

## 环境配置

| 依赖 | 路径                                                  |
|------|-----------------------------------------------------|
| Slay the Spire 2 | `G:\SteamLibrary\steamapps\common\Slay the Spire 2` |
| STS2-RitsuLib | 与本项目同级目录（`..\STS2-RitsuLib`）                        |
| Godot | `4.5.1 Mono`                                        |
| .NET | `net10.0`                                           |

构建后自动复制至 `{STS2Dir}\mods\STS2_WineFox\`。



## 作者

- 程序员：OLC，灯火橘
- 数值策划，卡牌设计：IAmNotFood，明风OuO，晓月漓风，末影酱安德Channel
- 画师：穰雀，Linger铃



## 鸣谢

- 感谢[酒石酸菌](https://github.com/TartaricAcid/WineFoxModel)创作了可爱的酒狐
