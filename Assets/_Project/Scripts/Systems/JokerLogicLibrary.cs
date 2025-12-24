using UnityEngine;
using Model;
using Core;
using System.Linq;

namespace Systems
{
    public static class JokerLogicLibrary
    {
        public static void Execute(Joker joker, JokerTriggerType trigger, object contextData)
        {
            if (joker.Data.TriggerType != trigger) return;

            switch (joker.Data.ID)
            {
                case "J_Popcorn": 
                    ApplyPopcorn(joker, contextData as ScoreContext);
                    break;
                    
                case "J_Faceless":
                    ApplyFaceless(joker, contextData as ScoreContext);
                    break;
                    
                case "J_Void":
                    ApplyVoid(joker, contextData as ScoreContext);
                    break;
            }
        }

        private static void ApplyPopcorn(Joker joker, ScoreContext ctx)
        {
            if (ctx == null) return;

            Debug.Log($"<color=cyan>[Joker] Popcorn Maniac Triggered!</color>");

            ctx.IsBusted = false;

            int overflow = ctx.TotalPoints - ctx.BustThreshold;
            if (overflow > 0)
            {
                int bonusChips = overflow * 10;
                Debug.Log($"[POP] Before: Chips={ctx.BaseChips}, Points={ctx.TotalPoints}");
                ctx.BaseChips += bonusChips;
                Debug.Log($"[POP] After: Chips={ctx.BaseChips}, Points={ctx.TotalPoints}");
                
                Debug.Log($" -> Popcorn converted {overflow} overflow to {bonusChips} Chips!");
            }
        }

        private static void ApplyFaceless(Joker joker, ScoreContext ctx)
        {
            if (ctx == null) return;
            
            int faceCardCount = 0;

            foreach(var card in ctx.Cards)
            {
                if (card.IsFaceCard)
                {
                    ctx.TotalPoints -= card.CurrentPoints;
                    ctx.BaseChips -= card.CurrentPoints;
                    
                    faceCardCount++;
                }
            }

            if (faceCardCount > 0)
            {
                float addedMult = faceCardCount * 0.5f;
                ctx.Multiplier += addedMult;
                Debug.Log($"<color=cyan>[Joker] Faceless Triggered!</color> Removed points from {faceCardCount} cards, Mult +{addedMult}");
            }
        }

        private static void ApplyVoid(Joker joker, ScoreContext ctx)
        {
            if (ctx == null || ctx.Cards.Count == 0) return;

            Card firstCard = ctx.Cards[0];

            ctx.TotalPoints -= firstCard.CurrentPoints;
            ctx.BaseChips -= firstCard.CurrentPoints;

            float bonusMult = firstCard.CurrentPoints;
            ctx.Multiplier += bonusMult;

            Debug.Log($"<color=cyan>[Joker] The Void Triggered!</color> Swallowed {firstCard} -> Mult +{bonusMult}");
        }
    }
}
