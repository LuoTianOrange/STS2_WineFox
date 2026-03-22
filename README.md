# STS2 WineFox Mod

**版本**：0.1.0  
**游戏**：Slay the Spire 2  
**框架**：[STS2-RitsuLib](https://github.com/BAKAOLC/STS2-RitsuLib)

WineFox（酒狐）是一个自定义角色 Mod，围绕**材料合成**与**资源管理**的玩法设计。通过采集木板、圆石等材料，利用合成、挖掘等机制打出强力组合。

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
| 基础合成（BaseCraft） | 1 |

---

## 卡牌列表

| 卡牌 | 稀有度 | 费用 | 效果简介 |
|------|--------|------|---------|
| WineFoxStrike（打击） | Basic | 1 | 造成 6 点伤害 |
| WineFoxDefend（防御） | Basic | 1 | 获得 5 点格挡 |
| BasicMine（基础采掘） | Common | 1 | 获得木板和圆石各 2 层 |
| BaseCraft（基础合成） | Basic | 1 | 消耗 2 木板 + 2 圆石，从合成品中选择一张获得；升级费用变为 0 |
| FullAttack（全力一击） | Common | 2 | 消耗所有资源，每层对随机敌人造成 4 点伤害；升级后获得保留 |
| PlantTrees（种植树木） | Uncommon | 1 | 获得 5 格挡，下回合获得 4 层种植；升级后格挡变为 7 |
| MiningGems（挖掘宝石） | Rare | 2 | 消耗 1 层挖掘，获得 1 个钻石 |
| SteamEngine（蒸汽引擎） | Rare | 2 | 给予 1 层蒸汽（每回合开始时获得 1 应力）；升级费用变为 1 |
| StonePick（石镐） | Token | 0 | 获得 1 层挖掘（战斗中由基础合成生成） |
| StoneSword（石剑） | Token | 0 | 获得 4 层力量，接下来两个回合每回合 -2 层力量（战斗中由基础合成生成） |
| IronArmor（铁甲） | Token | 0 | 获得 5 层覆甲（战斗中由基础合成生成） |

---

## Power 列表

| Power | 类型 | 效果 |
|-------|------|------|
| WoodPower（木板） | Counter | 材料计数器，可用于合成 |
| StonePower（圆石） | Counter | 材料计数器，可用于合成 |
| IronPower（铁锭） | Counter | 材料计数器，可用于高级合成 |
| StressPower（应力） | Counter | 获得材料时消耗 1 层，使本次获得的材料数量翻倍 |
| DiggingPower（挖掘） | Counter | 每次出攻击牌（非采掘）额外获得等量木板和圆石 |
| PlantPower（种植） | Counter | 在下回合开始时获得等量木板，触发后移除自身 |
| Steam（蒸汽） | Counter | 每回合开始时获得 1 应力。 |

---

## 遗物列表

| 遗物 | 稀有度 | 效果 |
|------|--------|------|
| HandCrank（手摇曲柄） | Starter | 战斗开始时获得 1 层应力；获得材料时若有应力，消耗 1 层并使数量翻倍 |

---

## 项目结构

```
STS2_WineFox/
├── Cards/
│   ├── Basic/              # 基础卡（不进入奖励池）
│   ├── Common/             # 普通奖励卡
│   ├── Uncommon/           # 稀有奖励卡
│   ├── Rare/               # 稀有奖励卡
│   ├── Token/              # 战斗中临时生成的卡（autoAdd: false）
│   ├── DynamicVars/        # 自定义 DynamicVar 类
│   ├── WineFoxCard.cs      # 卡牌基类
│   └── WineFoxKeywords.cs  # 关键字 ID 常量
├── Character/
│   ├── WineFox.cs          # 角色定义（HP、初始牌组、初始遗物）
│   ├── WineFoxCardPool.cs  # 卡池（奖励池、外框颜色）
│   ├── WineFoxRelicPool.cs
│   └── WineFoxPotionPool.cs
├── Content/
│   └── Descriptors/
│       └── WineFoxContentManifest.cs  # 所有内容注册入口
├── Powers/
│   ├── WineFoxPower.cs      # Power 基类
│   ├── WineFoxActions.cs    # 材料获取/消耗工具方法
│   ├── MaterialPower.cs     # 材料 Power 基类
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
| `None` | 不叠加，特殊逻辑用 | — |
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

---

## 环境配置

| 依赖 | 路径 |
|------|------|
| Slay the Spire 2 | `G:\SteamLibrary\steamapps\common\Slay the Spire 2` |
| STS2-RitsuLib | 与本项目同级目录（`..\STS2-RitsuLib`） |
| Godot | `4.5.1 Mono` |
| .NET | `net9.0` |

构建后自动复制至 `{STS2Dir}\mods\STS2_WineFox\`。

