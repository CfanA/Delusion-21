namespace Core
{
    public enum CardSuit
    {
        Spades,
        Hearts,
        Clubs,
        Diamonds,
        None
    }

    public enum CardRank
    {
        Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9,
        Ten = 10, Jack = 11, Queen = 12, King = 13, Ace = 14
    }
    
    public enum JokerTriggerType
    {
        Passive,
        OnDraw,
        OnPlay,
        OnDiscard,
        OnScoreResolve,
        OnBust,
        OnBossStart
    }
 
    public enum Rarity
    {
        Common, Uncommon, Rare, Legendary
    }
}