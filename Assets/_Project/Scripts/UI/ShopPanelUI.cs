using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Systems;

namespace UI
{
    public class ShopPanelUI : MonoBehaviour
    {
        [Header("Layout")]
        public Transform ItemsContainer; 
        public GameObject ItemPrefab;    
        public GameObject ShopRoot;      

        [Header("Header Info")]
        public TMP_Text MoneyText;
        public TMP_Text RoundText; // 如果有

        [Header("Footer Controls")]
        public TMP_Text RerollPriceText;
        public Button RerollButton;
        public Button NextRoundButton;

        private void Start()
        {
            if (ShopManager.Instance != null)
                ShopManager.Instance.OnShopRefreshed += RefreshDisplay;
                
            RerollButton.onClick.AddListener(() => ShopManager.Instance.RerollShop());
            NextRoundButton.onClick.AddListener(() => ShopManager.Instance.LeaveShop());

            // 初始隐藏
            // ShopRoot.SetActive(false); 
        }

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
            // 清理
            foreach (Transform child in ItemsContainer) Destroy(child.gameObject);

            // 生成商品
            var items = ShopManager.Instance.CurrentItems;
            for (int i = 0; i < items.Count; i++)
            {
                GameObject go = Instantiate(ItemPrefab, ItemsContainer);
                go.GetComponent<ShopItemUI>().Initialize(items[i], i);
            }

            // 更新文本
            if (GameRunManager.Instance != null)
            {
                MoneyText.text = $"{GameRunManager.Instance.CurrentRun.Money}";
            }
            
            int cost = ShopManager.Instance.RerollPrice;
            RerollPriceText.text = $"${cost}";
            
            // 检查 Reroll 钱够不够
            int currentMoney = GameRunManager.Instance.CurrentRun.Money;
            RerollButton.interactable = currentMoney >= cost;
        }
    }
}