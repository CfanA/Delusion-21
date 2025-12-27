using UnityEngine;
using TMPro;
using Systems;
using DG.Tweening; // 引入 DOTween

namespace UI
{
    public class ScoreBoardUI : MonoBehaviour
    {
        [Header("Equation UI")]
        public TMP_Text ChipsText;
        public TMP_Text MultText;
        public TMP_Text TotalScoreText;
        
        [Header("Container References for Shake")]
        public RectTransform ScoreBoardContainer; // 把整个计分板的父物体拖进去

        private long _lastTotalScore = -1;

        private void Update()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var ctx = BattleManager.Instance.CurrentScoreContext;
            if (ctx == null) return;

            // 更新文本
            ChipsText.text = ctx.BaseChips.ToString(); // 只有变化时才建议 setText，这里简化
            MultText.text = ctx.Multiplier.ToString("F1");
            
            long currentTotal = ctx.GetScore();
            TotalScoreText.text = currentTotal.ToString("N0"); // N0 格式化为 1,000

            // 检测数值变化，触发特效
            if (_lastTotalScore != -1 && currentTotal != _lastTotalScore)
            {
                PlayScoreShakeEffect();
            }
            _lastTotalScore = currentTotal;
        }

        private void PlayScoreShakeEffect()
        {
            // 杀掉之前的动画防止冲突
            ScoreBoardContainer.DOKill(true);

            // 1. 缩放弹跳
            ScoreBoardContainer.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo);

            // 2. 震动 (Strength: 强度, Vibrato: 频率)
            ScoreBoardContainer.DOShakeAnchorPos(0.3f, strength: 20f, vibrato: 20);

            // 3. 颜色闪烁 (如果 TotalScoreText 支持)
            TotalScoreText.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
        }
    }
}