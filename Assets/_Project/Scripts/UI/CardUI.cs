using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Model;
using Core;

namespace UI
{
    public class CardUI : MonoBehaviour
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

        private Card _targetCard;

        public void Initialize(Card card)
        {
            _targetCard = card;
            RenderStaticInfo();
            RefreshDynamicInfo();
        }

        private void RenderStaticInfo()
        {
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
