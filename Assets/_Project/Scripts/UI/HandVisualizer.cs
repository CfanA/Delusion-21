using UnityEngine;
using System.Collections.Generic;
using Systems;
using Model;
using DG.Tweening; // 引入 DOTween

namespace UI
{
    public class HandVisualizer : MonoBehaviour
    {
        public GameObject CardPrefab;
        public Transform HandContainer;

        [Header("Fan Settings")]
        public float FanSpacing = 100f;   // 牌与牌的间距
        public float FanRadius = 1500f;   // 扇形半径（越大越平）
        public float FanAngle = 30f;      // 总张角

        private List<CardUI> _spawnedCards = new List<CardUI>();

        private void Start()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnCardDrawn += HandleCardDrawn;
                BattleManager.Instance.OnHandUpdate += RefreshHandVisuals; // 改名为刷新视觉
                BattleManager.Instance.OnStateChanged += HandleStateChange;
            }
        }

        private void OnDestroy()
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.OnCardDrawn -= HandleCardDrawn;
                BattleManager.Instance.OnHandUpdate -= RefreshHandVisuals;
                BattleManager.Instance.OnStateChanged -= HandleStateChange;
            }
        }

        private void HandleCardDrawn(Card cardData)
        {
            GameObject go = Instantiate(CardPrefab, HandContainer);
            CardUI ui = go.GetComponent<CardUI>();
            ui.Initialize(cardData);
            _spawnedCards.Add(ui);

            // 生成时播放一个小动画
            go.transform.localScale = Vector3.zero;
            go.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

            RefreshHandVisuals();
        }

        // 核心：计算扇形布局
        private void RefreshHandVisuals()
        {
            int count = _spawnedCards.Count;
            if (count == 0) return;

            float startAngle = -FanAngle / 2f;
            float angleStep = FanAngle / (count > 1 ? count - 1 : 1);
            if (count == 1) startAngle = 0; // 一张牌居中

            for (int i = 0; i < count; i++)
            {
                CardUI card = _spawnedCards[i];
                Transform t = card.transform;

                // 1. 计算角度
                float angle = (count > 1) ? startAngle + i * angleStep : 0;
                
                // 2. 计算位置 (基于圆弧)
                // 简单的水平分布 + 垂直偏移(模拟弧度)
                float xPos = (i - (count - 1) / 2f) * FanSpacing;
                float yPos = Mathf.Abs(i - (count - 1) / 2f) * -10f; // 中间高，两边低

                // 3. 应用 DOTween 动画移动到目标位置
                t.DOLocalMove(new Vector3(xPos, yPos, 0), 0.3f).SetEase(Ease.OutQuad);
                t.DOLocalRotate(new Vector3(0, 0, -angle), 0.3f).SetEase(Ease.OutQuad);
                
                // 确保层级正确 (右边的盖住左边的)
                t.SetSiblingIndex(i);
                
                // 刷新卡牌数据
                card.RefreshDynamicInfo();
            }
        }

        private void HandleStateChange(Core.BattleState state)
        {
            if (state == Core.BattleState.Setup) ClearHand();
        }

        private void ClearHand()
        {
            foreach (var cardUI in _spawnedCards) Destroy(cardUI.gameObject);
            _spawnedCards.Clear();
        }
    }
}