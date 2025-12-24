using System.Collections.Generic;
namespace Model
{
    public class RunData
    {
        public int CurrentRound;
        public int Money;
        public List<Joker> OwnedJokers;
        public List<Card> PlayerDeck;

        public RunData()
        {
            CurrentRound = 1;
            Money = 10;
            OwnedJokers = new List<Joker>();
            PlayerDeck = new List<Card>();

            InitializeStandardDeck();
        }

        private void InitializeStandardDeck()
        {
            foreach (Core.CardSuit suit in System.Enum.GetValues(typeof(Core.CardSuit)))
            {
                if (suit == Core.CardSuit.None) continue;
                foreach (Core.CardRank rank in System.Enum.GetValues(typeof(Core.CardRank)))
                {
                    PlayerDeck.Add(new Card(suit, rank));
                }
            }
        }
    }
}