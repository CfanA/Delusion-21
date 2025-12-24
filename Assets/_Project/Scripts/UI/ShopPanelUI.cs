using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Systems;
using System.Collections.Generic;

namespace UI
{
    public class ShopPanelUI : MonoBehaviour
    {
        [Header("Layout")]
        public Transform ItemsContainer; // 放置商品的父节点
        public GameObject ItemPrefab;    // 商品预制体
        public GameObject ShopRoot;      // 整个商店界面的根节点 (用于显示/隐藏)

        [Header("Controls")]
        public TMP_Text MoneyText;
        public TMP_Text RerollPriceText;
        public Button RerollButton;
        public Button NextRoundButton;

        private void Start()
        {
            // 订阅事件
            if (ShopManager.Instance != null)
                ShopManager.Instance.OnShopRefreshed += RefreshDisplay;
                
            // 绑定按钮
            RerollButton.onClick.AddListener(() => ShopManager.Instance.RerollShop());
            NextRoundButton.onClick.AddListener(() => ShopManager.Instance.LeaveShop());

            // 初始状态：隐藏商店
            ShopRoot.SetActive(false);
        }

        // 被外部调用以显示商店
        public void Show()
        {
            ShopRoot.SetActive(true);
            RefreshDisplay();
        }

        public void Hide()
        {
            ShopRoot.SetActive(false);
        }

        private void RefreshDisplay()
        {
            // 1. 清理旧商品
            foreach (Transform child in ItemsContainer) Destroy(child.gameObject);

            // 2. 生成新商品
            var items = ShopManager.Instance.CurrentItems;
            for (int i = 0; i < items.Count; i++)
            {
                GameObject go = Instantiate(ItemPrefab, ItemsContainer);
                go.GetComponent<ShopItemUI>().Initialize(items[i], i);
            }

            // 3. 更新文本
            if (GameRunManager.Instance != null)
            {
                MoneyText.text = $"Money: ${GameRunManager.Instance.CurrentRun.Money}";
            }
            RerollPriceText.text = $"Reroll (${ShopManager.Instance.RerollPrice})";
        }
    }
}