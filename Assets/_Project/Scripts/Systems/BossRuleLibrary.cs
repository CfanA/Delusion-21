using System.Collections.Generic;
using UnityEngine;
using Model;
using Core;

namespace Systems
{
    public static class BossRuleLibrary
    {
        public static void ApplyCardRules(BossData boss, List<Card> hand)
        {
             Debug.Log($"正在应用 Boss 规则: {boss?.RuleModifierID}");
            
            if (boss == null || string.IsNullOrEmpty(boss.RuleModifierID)) return;

            switch (boss.RuleModifierID)
            {
                case "DIAMONDS_ONE":
                    int count = 0;
                    foreach (var card in hand)
                    {
                         Debug.Log($"检查卡牌: {card.BaseSuit}");
                        
                        if (card.BaseSuit == CardSuit.Diamonds)
                        {
                            card.CurrentPoints = 1;
                            card.IsDebuffed = true;
                            count++;
                        }
                    }
                    if (count > 0) Debug.Log($"⚡ 生效了！Boss 将 {count} 张方块牌变成了 1 点！");
                    
                    break;

                case "NO_FACE_CARDS":
                    foreach (var card in hand)
                    {
                        if (card.IsFaceCard)
                        {
                            card.CurrentPoints = 0;
                            card.IsDebuffed = true;
                        }
                    }
                    break;
            }
        }

        public static int GetModifiedThreshold(BossData boss, int defaultThreshold = 21)
        {
            if (boss == null || string.IsNullOrEmpty(boss.RuleModifierID)) return defaultThreshold;

            switch (boss.RuleModifierID)
            {
                case "LOWER_CEILING": 
                    return 15;
                
                default:
                    return defaultThreshold;
            }
        }
    }
}