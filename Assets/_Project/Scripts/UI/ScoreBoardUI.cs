using UnityEngine;
using TMPro;
using Systems;

namespace UI
{
    public class ScoreBoardUI : MonoBehaviour
    {
        public TMP_Text ChipsText;
        public TMP_Text MultText;
        public TMP_Text TotalScoreText; 
        public TMP_Text TargetText;

        private void Update()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var ctx = BattleManager.Instance.CurrentScoreContext;
            var boss = BattleManager.Instance.CurrentBoss;

            if (ctx != null)
            {
                ChipsText.text = $"{ctx.BaseChips + ctx.TotalPoints}";
                MultText.text = $"x{ctx.Multiplier:F1}";
                TotalScoreText.text = $"{ctx.GetScore()}";

                TotalScoreText.color = ctx.IsBusted ? Color.red : Color.white;
            }

            if (boss != null)
            {
                TargetText.text = $"Target: {boss.BaseBlindTarget}";
            }
        }
    }
}