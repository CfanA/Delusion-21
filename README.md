# Delusion-21

> 一款融合 **Blackjack（21点）** 与 **Roguelike Deckbuilder（牌组构筑）** 的 Unity 原型项目。  
> 目标是通过 Joker 构筑、Boss 规则对抗与局外成长，形成“高风险算分 + 组合爆发”的回合体验。

---

## 项目定位

**Delusion-21** 是我用于验证“经典扑克牌玩法 × 现代 Roguelike 系统设计”可行性的独立原型。项目重点不在美术量产，而在：

- 规则系统拆分是否清晰（战斗 / 计分 / Boss / Joker）；
- 数据驱动是否可扩展（ScriptableObject 配置 Boss 与 Joker）；
- 核心回合是否具备可重复游玩的策略深度。

---

## 核心玩法

- **21点基础规则 + 可变爆点阈值**
  - 默认按 21 点结算；
  - 不同 Boss 可修改爆点阈值（例如降低到 15），直接改变决策模型。

- **Boss 规则改写卡牌价值**
  - 例如将方块牌统一降为 1 点，或让人头牌失效；
  - 同一手牌在不同 Boss 下的最优策略不同。

- **Joker 触发链与被动效果**
  - 支持 `Passive / OnScoreResolve / OnBust` 等触发时机；
  - 通过优先级控制结算顺序，支持组合技设计。

- **Run 结构（战斗 → 商店 → 下一回合）**
  - 回合胜利可获得货币进入商店；
  - 购买 Joker 强化后继续推进，形成 Roguelike 递进体验。

---

## 系统架构

```text
GameRunManager
  ├─ 管理整局 Run（回合推进、胜负流转、货币）
  ├─ 战斗入口 -> BattleManager
  └─ 胜利后打开商店 -> ShopManager / ShopPanelUI

BattleManager
  ├─ 管理牌堆 / 手牌 / 弃牌堆
  ├─ 执行抽牌与回合状态切换
  ├─ 调用 BossRuleLibrary 改写卡牌
  ├─ 调用 JokerManager 施加被动与触发效果
  └─ 调用 ScoreCalculator 得到 ScoreContext

ScoreCalculator
  └─ 负责点数、A 牌降级、Bust/Blackjack 判定

JokerManager + JokerLogicLibrary
  ├─ 管理当前生效 Joker
  └─ 按触发时机执行具体效果逻辑

BossRuleLibrary
  └─ 集中管理 Boss 特殊规则与阈值修改
```

---

## 技术栈

- **Engine**: Unity 2022.3 LTS（URP）
- **Language**: C#
- **UI**: UGUI + TextMeshPro
- **Tween**: DOTween
- **Data**: ScriptableObject（BossData / JokerData）

---

## 如何运行

1. 使用 **Unity 2022.3.62f2c1**（或相近 2022.3 LTS 版本）打开项目。  
2. 打开场景：`Assets/_Project/Scenes/MainTable.unity`。  
3. 运行后从 `GameRunManager` 启动新局，观察战斗、算分与商店流程。

---

## 当前完成度

- [x] 回合战斗基础循环（抽牌 / 停牌 / 算分）
- [x] Boss 规则改写（点数与阈值）
- [x] Joker 系统（被动 + 触发）
- [x] 商店生成、购买与 reroll
- [ ] 更完整的内容池（更多 Boss/Joker）
- [ ] 战斗演出与反馈强化
- [ ] 存档与局外成长

---

## 👤 作者说明

该仓库当前为玩法原型阶段，后续计划补充：

- 更明确的数值曲线与难度分层；
- 更完整的 UI/UX 和视觉统一；
- Demo 视频与可执行版本（WebGL / PC Build）。
