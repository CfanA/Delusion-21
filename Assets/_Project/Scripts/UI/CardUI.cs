using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 引入事件系统接口
using TMPro;
using Model;
using System;
using Core; // 引入 Action

namespace UI
{
    // 实现 IPointer 接口以检测鼠标
    public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI References")]
        public Image Background;
        public Image SuitIcon; 
        public TMP_Text RankText;
        public TMP_Text ValueText;
        public GameObject DebuffOverlay;

        [Header("Assets")]
        public Sprite SpadeSprite;
        public Sprite HeartSprite;
        public Sprite ClubSprite;
        public Sprite DiamondSprite;

        // --- 新增状态控制 ---
        // 只读属性，外部只能读取，不能修改
        public bool IsHovered { get; private set; } = false;
        
        // 当悬停状态改变时触发的事件，通知 HandVisualizer
        public event Action<CardUI> OnHoverStateChanged;

        private Card _targetCard;

        public void Initialize(Card card)
        {
            _targetCard = card;
            RenderStaticInfo();
            RefreshDynamicInfo();
        }

        // 当鼠标进入时
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsHovered) return;
            IsHovered = true;
            // 通知监听者：“我被悬停了，请重新排版！”
            OnHoverStateChanged?.Invoke(this); 
        }

        // 当鼠标离开时
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsHovered) return;
            IsHovered = false;
            // 通知监听者：“我恢复正常了，请复原！”
            OnHoverStateChanged?.Invoke(this);
        }

        // --- 下面是原有的显示逻辑，保持不变 ---

        private void RenderStaticInfo()
        {
            if (_targetCard == null) return;

            switch (_targetCard.BaseSuit)
            {
                case CardSuit.Spades: SuitIcon.sprite = SpadeSprite; Background.color = new Color(0.8f, 0.8f, 0.9f); break;
                case CardSuit.Hearts: SuitIcon.sprite = HeartSprite; Background.color = new Color(1f, 0.8f, 0.8f); break;
                case CardSuit.Clubs: SuitIcon.sprite = ClubSprite; Background.color = new Color(0.8f, 0.9f, 0.8f); break;
                case CardSuit.Diamonds: SuitIcon.sprite = DiamondSprite; Background.color = new Color(1f, 0.9f, 0.8f); break;
            }

            RankText.text = GetRankString(_targetCard.BaseRank);

            bool isRed = _targetCard.BaseSuit == CardSuit.Hearts || _targetCard.BaseSuit == CardSuit.Diamonds;
            RankText.color = isRed ? Color.red : Color.black;
        }

        public void RefreshDynamicInfo()
        {
            if (_targetCard == null) return;

            ValueText.text = _targetCard.CurrentPoints.ToString();

            if (_targetCard.IsDebuffed)
            {
                DebuffOverlay.SetActive(true);
                ValueText.color = Color.gray;
            }
            else
            {
                DebuffOverlay.SetActive(false);
                ValueText.color = Color.white; 
            }
        }

        private string GetRankString(CardRank rank)
        {
            switch (rank)
            {
                case CardRank.Jack: return "J";
                case CardRank.Queen: return "Q";
                case CardRank.King: return "K";
                case CardRank.Ace: return "A";
                default: return ((int)rank).ToString();
            }
        }
    }
}