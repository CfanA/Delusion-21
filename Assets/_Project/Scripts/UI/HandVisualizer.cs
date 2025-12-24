using UnityEngine;
using System.Collections.Generic;
using Systems;
using Model;

namespace UI
{
    public class HandVisualizer : MonoBehaviour
    {
        public GameObject CardPrefab;
        public Transform HandContainer;

        private List<CardUI> _spawnedCards = new List<CardUI>();

        private void Start()
        {
            BattleManager.Instance.OnCardDrawn += HandleCardDrawn;
            BattleManager.Instance.OnHandUpdate += RefreshAllCards;
            BattleManager.Instance.OnStateChanged += HandleStateChange;
        }

        private void OnDestroy()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnCardDrawn -= HandleCardDrawn;
                BattleManager.Instance.OnHandUpdate -= RefreshAllCards;
                BattleManager.Instance.OnStateChanged -= HandleStateChange;
            }
        }

        private void HandleCardDrawn(Card cardData)
        {
            Debug.Log($"🧬 DNA检测: 物体[{gameObject.name}] - 脚本ID[{this.GetInstanceID()}] 正在生成卡牌 {cardData.BaseSuit} {cardData.BaseRank}");
            // ====================

            GameObject go = Instantiate(CardPrefab, HandContainer);
            CardUI ui = go.GetComponent<CardUI>();
            ui.Initialize(cardData);
            _spawnedCards.Add(ui);
        }

        private void RefreshAllCards()
        {
            foreach (var cardUI in _spawnedCards)
            {
                cardUI.RefreshDynamicInfo();
            }
        }

        private void HandleStateChange(Core.BattleState state)
        {
            if (state == Core.BattleState.Setup)
            {
                ClearHand();
            }
        }

        private void ClearHand()
        {
            foreach (var cardUI in _spawnedCards)
            {
                Destroy(cardUI.gameObject);
            }
            _spawnedCards.Clear();
        }
    }
}