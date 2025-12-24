using System;
using UnityEngine; 

namespace Model
{
    using Core;

    [Serializable]
    public class Card
    {
        public readonly string UID;
        public readonly CardSuit BaseSuit;
        public readonly CardRank BaseRank;
        
        public CardSuit CurrentSuit; 
        public int CurrentPoints;
        public bool IsFaceCard;
        
        // --- 特殊标记 ---
        public bool IsSealed;
        public bool IsDebuffed;

        public Card(CardSuit suit, CardRank rank)
        {
            UID = System.Guid.NewGuid().ToString();
            BaseSuit = suit;
            BaseRank = rank;
            
            ResetToDefault();
        }

        public void ResetToDefault()
        {
            CurrentSuit = BaseSuit;
            IsFaceCard = (BaseRank == CardRank.Jack || BaseRank == CardRank.Queen || BaseRank == CardRank.King);

            if (IsFaceCard) CurrentPoints = 10;
            else if (BaseRank == CardRank.Ace) CurrentPoints = 11; 
            else CurrentPoints = (int)BaseRank;
            
            IsSealed = false;
            IsDebuffed = false;
        }

        public override string ToString()
        {
            return $"{BaseSuit} {BaseRank} (Pts: {CurrentPoints})";
        }
    }
}