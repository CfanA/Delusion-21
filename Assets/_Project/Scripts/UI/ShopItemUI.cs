using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Model;
using Systems;

namespace UI
{
    public class ShopItemUI : MonoBehaviour
    {
        [Header("UI Refs")]
        public Image Icon;
        public TMP_Text NameText;
        public TMP_Text DescText;
        public TMP_Text PriceText;
        public Button BuyButton;
        public GameObject SoldOverlay; // 售罄遮罩

        private int _myIndex;
        private ShopItem _itemData;

        public void Initialize(ShopItem item, int index)
        {
            _itemData = item;
            _myIndex = index;

            // 显示基础信息
            if (item.JokerSource.Icon != null) Icon.sprite = item.JokerSource.Icon;
            NameText.text = item.JokerSource.JokerName;
            DescText.text = item.JokerSource.Description;
            PriceText.text = $"${item.Price}";

            // 状态控制
            if (item.IsSold)
            {
                SoldOverlay.SetActive(true);
                BuyButton.interactable = false;
            }
            else
            {
                SoldOverlay.SetActive(false);
                BuyButton.interactable = true;
            }

            // 绑定点击事件 (移除旧的监听以防重复)
            BuyButton.onClick.RemoveAllListeners();
            BuyButton.onClick.AddListener(OnBuyClicked);
        }

        private void OnBuyClicked()
        {
            ShopManager.Instance.BuyItem(_myIndex);
        }
    }
}