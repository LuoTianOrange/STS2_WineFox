# STS2 WineFox Mod

**版本**：0.1.0  
**游戏**：Slay the Spire 2  
**框架**：[STS2-RitsuLib](https://github.com/BAKAOLC/STS2-RitsuLib)

WineFox（酒狐）是一个自定义角色 Mod，围绕**材料合成**与**资源管理**的玩法设计。通过采集木板、圆石、铁锭、钻石等材料，利用合成、挖掘、应力等机制打出强力组合。

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

## 卡牌列表

### Basic（基础牌）

| 卡牌 | 费用 | 类型 | 效果 | 升级 |
|------|------|------|------|------|
| WineFoxStrike（打击） | 1 | 攻击 | 造成 6 点伤害。 | 伤害→9 |
| WineFoxDefend（防御） | 1 | 技能 | 获得 5 点格挡。 | 格挡→8 |
| BasicMine（基础采掘） | 1 | 技能 | 获得 2 木板 和 2 圆石。 | 各+1 |
| BasicCraft（基础合成） | 1 | 技能 | **合成**。获得 5 点格挡。**保留** | 费用→0 |

### Common（普通）

| 卡牌 | 费用 | 类型 | 效果 | 升级 |
|------|------|------|------|------|
| BackupCrafting（备用工作台） | 0 | 技能 | **消耗**；**合成**。 | +保留 |
| ChangeEquipment（更换装备） | 0 | 技能 | 抽 2 张牌，选择手牌中 2 张牌洗回抽牌堆。 | 各+1 |
| IronZombie（"白色僵尸"） | 1 | 攻击 | 造成 8 点伤害，获得 1 个铁锭。 | 伤害+3；铁锭+1 |
| LightAssault（轻装突击） | 0 | 攻击 | 造成 13 点伤害。你每有 2 份材料，这张牌的伤害就降低 1 点。 | 基础伤害→16 |
| MechanicalDrill（动力钻头） | 2 | 技能 | 获得 2 个铁锭。如果花费了应力，返还 2 点能量。 | 铁锭→3 |
| PlantTrees（种植树木） | 1 | 技能 | 获得 5 点格挡。在下回合开始时获得 4 个木板。 | 格挡→7；木板→6 |
| PreProcessing（预处理） | 0 | 技能 | **先手**；**消耗**；获得 3 木板 与 3 圆石。 | 另获得 3 格挡 |
| QuickShelter（速成掩体） | 1 | 技能 | **虚空**；花费 1 个木板 1 个圆石，获得 10 点格挡，抽一张牌。 | 格挡→13 |
| Regroup（重整旗鼓） | 1 | 技能 | **消耗**；抽 1 张牌。将弃牌堆中的 1 张牌放入你的手牌。 | 不再消耗 |
| TicTacToeGrid（井字格） | 1 | 攻击 | 造成 8 点伤害。**合成**。 | 伤害→11 |
| Traditionalist（传统派） | 0 | 技能 | 花费 2 木板。**合成**。抽 1 张牌。 | 摸牌→2 |
| VacantDomain（空置域） | 3 | 技能 | 获得 14 点格挡。获得 8 圆石。 | 格挡→18；圆石→12 |

### Uncommon（非普通）

| 卡牌 | 费用 | 类型 | 效果 | 升级 |
|------|------|------|------|------|
| Alternator（交流发电机） | 0 | 技能 | 花费 1 应力，获得 2 费。 | 获得→3 费 |
| AlterPath（另辟蹊径） | 0 | 技能 | 花费 2 木板，获得 5 点格挡，抽 2 张牌。 | 格挡→8；摸→3 |
| BlueprintPrinting（蓝图打印） | 1 | 技能 | **消耗**；花费 5 个木板和 5 个圆石，选择手牌 1 张卡，将其 2 张带保留、消耗、费用-1 的复制品加入手牌。 | 复制→3 张 |
| Bucket（水桶） | 1 | 技能 | **消耗**；获得 8 点格挡。下回合开始时再获得 8 点格挡。对所有单位施加 2 层虚弱。 | 格挡→11 |
| CrushingWheel（粉碎轮组） | 3 | 攻击 | 对所有敌人造成 21 点伤害。每在本回合花费过一次材料，这张牌的费用就 -1。 | — |
| DripstoneTrap（石锥陷阱） | 2 | 攻击 | 消耗所有圆石，每消耗一个就对随机敌人造成 2 点伤害，施加 1 层易伤。 | — |
| EmergencyRepair（紧急修复） | 1 | 攻击 | 造成 6 伤害，获得 5 格挡，在下个回合开始时获得 2 应力。 | 各+2 |
| FoxBite（狐咬） | 2 | 技能 | **保留**；给予 4 层虚弱。给予 4 层易伤。 | 各→7 |
| FullAttack（全力倾泻） | 2 | 攻击 | 花费所有材料。每花费 1 个材料，对目标造成 4 点伤害。 | +保留 |
| MassProduction（量产） | 2 | 技能 | **仅多人**；当你在本回合合成时，将 1 张合成物的复制品分给你的每个队友。 | 费用→1 |
| MechanicalSaw（动力锯） | 1 | 攻击 | 对所有敌人造成 8 点伤害。消耗 1 应力，额外造成 7 点伤害。 | — |
| PowerUp（换挡，加速，起飞） | 1 | 技能 | 获得 1 层应力。抽 1 张牌。 | 摸→2 |
| PressWToThink（「W」思索） | 1 | 技能 | **消耗**；查看抽牌堆顶端 3 张牌，选择其中 1 张加入手牌，并附加重放和消耗。 | 费用→0 |
| ProductionDocument（生产记录） | 1 | 法术 | 你在战斗中临时加入的牌获得保留。 | 费用→0 |
| RecordPlayer（唱片机） | 1 | 法术 | 消耗 1 钻石，获得 2 层仪式。 | 仪式→3 |
| RiclearPowerPlant（河电站） | 2 | 技能 | 获得 13 点格挡，获得 2 应力。 | 格挡→16 |
| SlashBladeWood（拔刀剑:无铭刀「木偶」） | 1 | 法术 | 每当你在回合中抽 1 张牌，获得 2 层活力。 | 层数→3 |
| SnowBallOverwhelming（雪球糊脸） | 1 | 攻击 | 造成 3 点伤害。本回合打出的下一张技能牌费用变为 0。 | 攻击→×2 |
| StressResponse（应激） | 0 | 技能 | **消耗**；失去 6 点生命值，获得 1 点最大生命值，2 点能量并抽 2 张牌。 | 获得→3 费 |
| WorkbenchBackpack（工作台背包） | 2 | 技能 | 获得 1 费。**合成**。将这张牌的费用减少 1。 | 获得→2 费 |

### Rare（稀有）

| 卡牌 | 费用 | 类型 | 效果 | 升级 |
|------|------|------|------|------|
| AllItem（全物品仓库） | 0 | 技能 | **消耗**；获得 1 费；抽 1 张牌；获得 1 木板、1 圆石、1 铁锭、1 钻石。 | +先手 |
| AutoCrafter（自动合成器） | 1 | 法术 | 当你的回合开始时，**合成**。 | +先手 |
| CobblestoneGenerator（刷石形态） | 2 | 法术 | **虚空**；在你的回合开始时获得 2 圆石，每回合增加 2。 | 费用→1 |
| EasyPeasy（轻而易举） | 0 | 法术 | 每回合开始时获得 1 费，额外抽 1 张牌。每回合开始时将 1 张晕眩加入你的手牌，每回合增加 1。 | N+1 |
| EquivalentExchange（等价交换） | 0 | 技能 | **消耗**；**先手**；选择任意手牌消耗，根据消耗卡牌的等级获得资源。 | 消耗已升级牌时获得资源翻倍 |
| HellGift（地狱馈赠） | 2 | 技能 | **消耗**；将一张随机金质装备卡牌加入你的手牌。 | 牌已升级 |
| LessHoliday（996） | 0 | 技能 | 选择你手牌中的最多 3 张牌变化为加班加点。 | 生成加班加点+ |
| MaidSupport（女仆的援护） | 2 | 技能 | **仅多人**；为所有玩家添加 5 层覆甲，2 点荆棘。 | 各+1 |
| Milk（牛奶） | 1 | 技能 | **消耗**；消除你身上的所有负面效果。 | 不再消耗 |
| MiningGems（挖掘宝石） | 2 | 技能 | 获得 2 个钻石。 | 钻石→3 |
| NoMoreFalchion（时代变了） | 2 | 技能 | **消耗**；消耗所有铁锭，将一张攻击次数等同于消耗铁锭数的【钢铁枪膛】加入你的手牌。 | 生成【钢铁枪膛+】 |
| ShieldAttack（盾击） | 0 | 攻击 | 对所有敌人造成 2 倍于你当前格挡的伤害。清除你的所有格挡。抽 1 张牌。 | — |
| SpinningHand（飞转的手） | 4 | 攻击 | 对所有敌人造成 26 点伤害，本场战斗中每合成一次，这张牌的费用就减少 1。 | — |
| SteamEngine（蒸汽引擎） | 2 | 法术 | 每回合开始时获得 1 应力。 | 费用→1 |
| WirelessTerminal（无线终端） | 1 | 技能 | **合成**。将这张牌返回你的手牌。 | +保留 |

### Ancient（古代）

| 卡牌 | 费用 | 类型 | 效果 | 升级 |
|------|------|------|------|------|
| Forging（锻造） | 1 | 技能 | **保留**；获得 10 格挡。每种材料各获得 2 个。**合成**两次。 | 格挡→13；费→0 |
| NetheritePickaxe（下界合金镐） | 1 | 法术 | 在同一个回合内每打出 2 张牌，就获得木板、圆石、铁锭和钻石各 1 个。 | +先手 |

### Token（战斗生成）

Token 牌由合成配方或特定卡牌在战斗中生成，不进入奖励池。

#### 合成配方产物

| 卡牌 | 合成配方 | 效果 |
|------|---------|------|
| Nothing（空手打击） | 无需材料 | **消耗**；造成 1 点伤害。 |
| WoodenSword（木剑） | 3 木板 | **消耗**；造成 6 点伤害；获得 4 层活力。 |
| StoneSword（石剑） | 1 木板 + 2 圆石 | **消耗**；造成 9 点伤害；获得 2 层力量。 |
| IronSword（铁剑） | 1 木板 + 2 铁锭 | **消耗**；造成 14 点伤害；下 1 张攻击牌额外打出一次。 |
| DiamondSword（钻石剑） | 1 木板 + 2 钻石 | **消耗**；造成 20 点伤害；每回合前 1 张攻击牌额外打出一次。 |
| WoodenPickaxe（木镐） | 4 木板 | **消耗**；下个回合开始时，获得 2 费，抽 1 张牌。 |
| StonePickaxe（石镐） | 1 木板 + 3 圆石 | **消耗**；每回合开始时，获得 1 个木板和 1 个圆石。 |
| IronPickaxe（铁镐） | 1 木板 + 3 铁锭 | **消耗**；接下来 2 次获得资源时，额外获得 3 个铁锭。 |
| DiamondPickaxe（钻石镐） | 1 木板 + 3 钻石 | **消耗**；你合成的每张牌都将升级。 |
| WoodenArmor（木甲） | 4 木板 | **消耗**；获得 7 点格挡。 |
| StoneArmor（砖石甲） | 8 圆石 | **消耗**；获得 7 层覆甲，每回合开始时失去 1 点敏捷。 |
| IronArmor（铁甲） | 8 铁锭 | **消耗**；获得 7 层覆甲。 |
| DiamondArmor（钻石甲） | 8 钻石 | **消耗**；获得 7 层覆甲。如果回合结束时你还有覆甲，就在本回合保留你的格挡。 |
| Shield（盾牌） | 6 木板 + 1 铁锭 | **消耗**；选择一名角色在本回合获得 10 点敏捷。本回合与下一回合无法再打出这张牌。 |

#### 其他 Token

| 卡牌 | 来源 | 效果 |
|------|------|------|
| GoldenSword（金剑） | HellGift | 你每打出两张攻击牌就获得 1 费，所有攻击牌获得虚无。 |
| GoldenPickaxe（金镐） | HellGift | 每次你获得材料，就获得等量格挡并对随机敌人造成 2 倍等量伤害。 |
| WorkWork（加班加点） | LessHoliday | **消耗**；获得 1 应力。（升级后额外抽 1 张牌） |
| SteelChamber（钢铁枪膛） | NoMoreFalchion | 造成 4 点伤害 N 次。将这张牌返回你的手牌。 |

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

| Power | 类型 | 效果 |
|-------|------|------|
| DiggingPower（工具：石镐） | Buff Counter | 每回合开始时，获得 N 个木板和 N 个圆石。 |
| PlantPower（种植） | Buff Counter | 下回合开始时，获得 N 木板，触发后消除。 |
| SteamPower（蒸汽） | Buff Counter | 每回合开始时，获得 N 应力。 |
| AutoCrafterPower（自动合成器） | Buff Counter | 当你的回合开始时，合成 N 次。 |
| BrushStoneFormPower（刷石形态） | Buff Counter | 在你的回合开始时获得 N 圆石，每回合层数增加（N 递增）。 |
| RepairPower（修复） | Buff Counter | 下回合开始时，获得 N 层应力，消除。 |
| EasyPeasyPower（核电，轻而易举啊） | Buff Counter | 回合开始时，获得 N 费用，抽 N 张卡。 |
| RadiationLeakPower（坏了坏了） | Debuff Counter | 回合开始时，将 N 张晕眩加入你的手牌，每回合增加 1 层。 |

### 装备效果 Power

| Power | 类型 | 效果 |
|-------|------|------|
| WoodenSwordPower（武器:木剑） | Buff Counter | 每回合开始时获得 4 层活力。还剩 N 回合。 |
| StoneSwordPower（武器:石剑） | Debuff Counter | 每回合开始时，失去 2 点力量。还剩 N 回合。 |
| IronSwordPower（武器：铁剑） | Buff Counter | 接下来 N 张攻击牌额外打出一次。 |
| DiamondSwordPower（武器：钻石剑） | Buff Counter | 每回合前 N 张攻击牌额外打出一次。 |
| GoldenSwordPower（武器：金剑） | Buff Counter | 你每打出两张攻击牌就获得 1 费，所有攻击牌获得虚无。 |
| StoneArmorPower（装备：砖石甲） | Debuff Counter | 每回合开始时，失去 N 点敏捷。 |
| DiamondArmorPower（装备：钻石甲） | Buff None | 如果回合结束时你还有覆甲，就在本回合保留你的格挡。 |
| IronPickaxePower（工具：铁镐） | Buff Counter | 从牌中获得资源时，额外获得 3 个铁锭。还剩 N 次。 |
| GoldenPickaxePower（工具：金镐） | Buff Counter | 每次你获得材料，就获得等量格挡并对随机敌人造成 N 倍等量伤害。 |
| NetheritePickaxePower（工具：下界合金镐） | Buff Counter | 每在同一个回合内打出 2 张牌，就获得木板、石头、铁、钻石各 1 个。 |
| ShieldDexPower（盾牌敏捷） | Buff | 临时敏捷（来自盾牌）。 |
| ShieldCooldownPower（盾牌冷却） | Debuff Counter | 无法打出盾牌。还剩 N 回合。 |

### 其他特殊 Power

| Power | 类型 | 效果 |
|-------|------|------|
| MassProductionPower（量产） | Buff Counter | 本回合中的合成产物将复制给其他玩家 N 张。回合结束消除。 |
| ProductionDocumentPower（生产记录） | Buff Counter | 你在战斗中临时加入的牌获得保留。回合结束每张保留牌+2 格挡。 |
| SnowBallOverwhelmingPower（雪球糊脸） | Buff Counter | 本回合打出的下一张技能牌费用变为 0。每出 1 张技能消耗 1 层；回合结束消除。 |
| SlashBladeWoodPower（拔刀剑:无铭刀「木偶」） | Buff Counter | 每当你在回合中抽 1 张牌，获得 N 层活力。 |
| BurningPower（烧伤） | Debuff Counter | 在你的回合开始时失去 N 点生命，然后将烧伤层数减半（向上取整）。 |

---

## 遗物列表

| 遗物                 | 稀有度 | 效果 |
|--------------------|--------|------|
| HandCrank（手摇曲柄）    | Starter | 战斗开始时获得 1 应力 |
| Deployer（神之手）      | Starter | 战斗开始时获得 3 应力 |
| MaidBackpack（女仆背包） | Uncommon | 获取时增加 2 个药水槽并填满随机药水 |

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
│   ├── CraftRecipeRegistry.cs  # 合成配方注册表
│   ├── WineFoxCard.cs      # 卡牌基类
│   └── WineFoxKeywords.cs  # 关键字 ID 常量
├── Character/
│   ├── WineFox.cs          # 角色定义（HP、初始牌组、初始遗物）
│   ├── WineFoxCardPool.cs  # 卡池（奖励池、外框颜色）
│   ├── WineFoxRelicPool.cs
│   └── WineFoxPotionPool.cs
├── Commands/
│   ├── CraftCmd.cs         # 合成命令（合成选择、材料消耗、记录）
│   └── MaterialCmd.cs      # 材料命令（获得/消耗材料）
├── Content/
│   └── Descriptors/
│       └── WineFoxContentManifest.cs  # 所有内容注册入口
├── Powers/
│   ├── WineFoxPower.cs      # Power 基类
│   ├── MaterialPower.cs     # 材料 Power 基类
│   ├── MaterialReactivePower.cs  # 材料事件响应 Power 基类
│   └── ...（各 Power 类）
├── Relics/
│   ├── WineFoxRelic.cs      # 遗物基类
│   └── ...（各遗物类）
├── STS2_WineFox/            # 游戏资源（Godot 导出目录）
│   ├── cards/               # 卡牌图片
│   ├── powers/              # Power 图标
│   ├── relics/              # 遗物图标
│   └── localization/
│       ├── zhs/             # 简体中文（cards / powers / relics / card_keywords）
│       └── eng/             # 英文
├── Const.cs                 # 全局路径与常量
└── Main.cs                  # Mod 入口
```

---

## 开发指南

### 添加新卡牌

| 步骤 | 文件 | 操作 |
|------|------|------|
| 1 | `Cards/{Rarity}/XxxCard.cs` | 创建卡牌类，继承 `WineFoxCard` |
| 2 | `Const.cs` | `Paths` 中添加 `CardXxx = Root + "/cards/card_xxx.png"` |
| 3 | `STS2_WineFox/cards/` | 放入卡牌图片 `card_xxx.png` |
| 4 | `Content/Descriptors/WineFoxContentManifest.cs` | 添加 `new CardRegistrationEntry<WineFoxCardPool, XxxCard>()` |
| 5 | `Character/WineFoxCardPool.cs` | `CardTypes` 中添加 `typeof(XxxCard)` |
| 6 | `localization/zhs/cards.json` | 添加 `title` 和 `description` |
| 7 | `localization/eng/cards.json` | 同上（英文） |
| 8 | `Character/WineFox.cs` | 若需加入初始牌组，在 `StartingDeckTypes` 中添加（**仅限 Basic 牌**） |

> **Token 卡**额外注意：构造函数中加 `showInCardLibrary: false, autoAdd: false`，但仍需在 `ContentManifest` 中注册。

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
| 1 | `Powers/XxxPower.cs` | 创建 Power 类，继承 `WineFoxPower` |
| 2 | `Const.cs` | `Paths` 中添加 `XxxPowerIcon = Root + "/powers/xxx.png"` |
| 3 | `STS2_WineFox/powers/` | 放入图标图片 |
| 4 | `Content/Descriptors/WineFoxContentManifest.cs` | 添加 `new PowerRegistrationEntry<XxxPower>()` |
| 5 | `localization/zhs/powers.json` | 添加 `title`、`description`、`smartDescription` |
| 6 | `localization/eng/powers.json` | 同上（英文） |

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
| 1 | `Cards/WineFoxKeywords.cs` | 添加 `public const string Xxx = "STS2_WINEFOX-XXX"` |
| 2 | `Content/Descriptors/WineFoxContentManifest.cs` | `KeywordEntries` 中添加 `KeywordRegistrationEntry.Card(WineFoxKeywords.Xxx, "STS2_WINEFOX-XXX")` |
| 3 | `localization/zhs/card_keywords.json` | 添加 `title` 和 `description` |
| 4 | `localization/eng/card_keywords.json` | 同上（英文） |

> ⚠️ 关键字描述是**纯静态文本**，不支持 `{VarName:diff()}` 等动态变量。

---

## 本地化说明

### 卡牌描述动态变量

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

### 常用 DynamicVar 类型

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

| 依赖 | 路径 |
|------|------|
| Slay the Spire 2 | `G:\SteamLibrary\steamapps\common\Slay the Spire 2` |
| STS2-RitsuLib | 与本项目同级目录（`..\STS2-RitsuLib`） |
| Godot | `4.5.1 Mono` |
| .NET | `net9.0` |

构建后自动复制至 `{STS2Dir}\mods\STS2_WineFox\`。
