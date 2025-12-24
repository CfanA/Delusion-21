using System.Collections.Generic;
using UnityEngine;
using Model;
using Core;

namespace Systems
{
    public static class ScoreCalculator
    {
        public static ScoreContext Calculate(List<Card> hand, int threshold = 21)
        {
            ScoreContext ctx = new ScoreContext(hand, threshold);

            int aceCount = 0;
            int rawSum = 0;

            foreach (var card in hand)
            {
                int val = card.CurrentPoints;
                if (card.BaseRank == CardRank.Ace && val == 11)
                {
                    aceCount++;
                }

                rawSum += val;
                ctx.BaseChips += val; 
            }

            while (rawSum > threshold && aceCount > 0)
            {
                rawSum -= 10; 
                aceCount--;
                ctx.BaseChips -= 10;
            }

            ctx.TotalPoints = rawSum;

            if (ctx.TotalPoints > threshold)
            {
                ctx.IsBusted = true;
            }
            else if (ctx.TotalPoints == 21 && hand.Count == 2)
            {
                ctx.IsBlackjack = true;
            }

            return ctx;
        }
    }
}