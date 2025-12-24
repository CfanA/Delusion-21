using System.Collections.Generic;
using Model;

namespace Systems
{
    public class ScoreContext
    {
        public List<Card> Cards;
        public int BustThreshold;

        public int BaseChips;
        public float Multiplier;
        public int TotalPoints;

        public bool IsBusted;
        public bool IsBlackjack;

        public ScoreContext(List<Card> cards, int threshold = 21)
        {
            Cards = cards;
            BustThreshold = threshold;
            BaseChips = 0;
            Multiplier = 1.0f;
            TotalPoints = 0;
            IsBusted = false;
            IsBlackjack = false;
        }

        public long GetScore()
        {
            return (long)((BaseChips + TotalPoints) * Multiplier);
        }
    }
}