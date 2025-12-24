using UnityEngine;
using System.Collections.Generic;
using Model;
using Core;

namespace Systems
{
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }

        [Header("Config")]
        public int RerollPrice = 5;
        public int ShopSize = 3;

        // 当前货架上的商品
        public List<ShopItem> CurrentItems = new List<ShopItem>();

        // 事件：通知 UI 刷新
        public System.Action OnShopRefreshed;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
        }

        /// <summary>
        /// 开启商店（通常在战斗胜利后调用）
        /// </summary>
        public void OpenShop()
        {
            Debug.Log("【Shop】Opening Shop...");
            GenerateNewItems();
        }

        /// <summary>
        /// 生成一批新货
        /// </summary>
        public void GenerateNewItems()
        {
            CurrentItems.Clear();

            // 1. 加载所有可能的 Joker (简单起见，我们直接读 Resources 文件夹)
            // 在实际项目中，你应该有一个 JokerDatabase
            JokerData[] allJokers = Resources.LoadAll<JokerData>("Data/Jokers");

            if (allJokers.Length == 0)
            {
                Debug.LogError("【Shop】没有找到任何 JokerData！请检查 Resources/Data/Jokers 路径。");
                return;
            }

            // 2. 随机挑选 3 个
            for (int i = 0; i < ShopSize; i++)
            {
                JokerData randomJoker = allJokers[Random.Range(0, allJokers.Length)];
                CurrentItems.Add(new ShopItem(randomJoker));
            }

            Debug.Log($"【Shop】Generated {CurrentItems.Count} items.");
            OnShopRefreshed?.Invoke();
        }

        /// <summary>
        /// 购买商品
        /// </summary>
        public void BuyItem(int index)
        {
            if (index < 0 || index >= CurrentItems.Count) return;

            ShopItem item = CurrentItems[index];
            RunData run = GameRunManager.Instance.CurrentRun;

            // 检查：已售出？
            if (item.IsSold) return;

            // 检查：钱够吗？
            if (run.Money < item.Price)
            {
                Debug.LogWarning("【Shop】钱不够！");
                return;
            }

            // 检查：槽位满了吗？
            if (run.OwnedJokers.Count >= 5) // 假设上限是 5
            {
                Debug.LogWarning("【Shop】Joker 槽位已满！");
                return;
            }

            // --- 交易执行 ---
            run.Money -= item.Price;       // 1. 扣钱
            item.IsSold = true;            // 2. 标记售出
            
            // 3. 给玩家添加 Joker
            JokerManager.Instance.AddJoker(item.JokerSource); // 添加到当前生效列表
            run.OwnedJokers.Add(new Joker(item.JokerSource)); // 添加到存档列表 (注意：这里简化了，实际上应该统一管理引用)

            Debug.Log($"【Shop】Bought {item.JokerSource.JokerName} for ${item.Price}");
            OnShopRefreshed?.Invoke(); // 刷新 UI
        }

        /// <summary>
        /// 刷新商店 (Reroll)
        /// </summary>
        public void RerollShop()
        {
            RunData run = GameRunManager.Instance.CurrentRun;
            if (run.Money >= RerollPrice)
            {
                run.Money -= RerollPrice;
                GenerateNewItems();
                Debug.Log("【Shop】Rerolled!");
            }
        }

        /// <summary>
        /// 离开商店，进入下一关
        /// </summary>
        public void LeaveShop()
        {
            GameRunManager.Instance.GoToNextRound();
        }
    }
}
