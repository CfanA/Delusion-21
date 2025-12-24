using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Model;

namespace Systems
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }
        public ScoreContext CurrentScoreContext;
        private RunData _currentRunSource;

        [Header("Debug Info")]
        public BattleState CurrentState;
        public BossData CurrentBoss;

        public List<Card> Deck = new List<Card>();
        public List<Card> Hand = new List<Card>();
        public List<Card> DiscardPile = new List<Card>();

        public System.Action<BattleState> OnStateChanged;
        public System.Action<Card> OnCardDrawn;
        public System.Action OnHandUpdate;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
        }

        private void Start()
        {
            //BossData boss = Resources.Load<BossData>("Data/Bosses/B_Supervisor");
            //StartCoroutine(SetupBattleRoutine(boss));
        }
        
        public void StartBattleFromRun(BossData boss, RunData runData)
        {
            _currentRunSource = runData;
            StartCoroutine(SetupBattleRoutine(boss));
        }

        public IEnumerator SetupBattleRoutine(BossData bossData)
        {
            ChangeState(BattleState.Setup);
            CurrentBoss = bossData;

            JokerManager.Instance.ActiveJokers.Clear();
            foreach (var joker in _currentRunSource.OwnedJokers)
            {
                JokerManager.Instance.ActiveJokers.Add(joker);
            }
            Debug.Log($"Loaded {_currentRunSource.OwnedJokers.Count} Jokers from Run.");

            Deck.Clear();
            Hand.Clear();
            DiscardPile.Clear();

            foreach(var card in _currentRunSource.PlayerDeck)
            {
                card.ResetToDefault();
                Deck.Add(card); 
            }
            
            ShuffleDeck();

            yield return new WaitForSeconds(0.5f);

            DrawCard(); 
            yield return new WaitForSeconds(0.2f);
            DrawCard();

            yield return new WaitForSeconds(0.5f);
            ChangeState(BattleState.PlayerTurn);

            JokerData popcorn = Resources.Load<JokerData>("Data/Jokers/J_Popcorn");
            if(popcorn) JokerManager.Instance.AddJoker(popcorn);
        }

        private void ChangeState(BattleState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
            Debug.Log($"【Battle】State Changed to: <color=yellow>{newState}</color>");
        }

        public void PlayerAction_Hit()
        {
            if (CurrentState != BattleState.PlayerTurn) return;

            Debug.Log("【Input】Player chose HIT.");
            DrawCard();

        }

        public void PlayerAction_Stand()
        {
            if (CurrentState != BattleState.PlayerTurn) return;

            Debug.Log("【Input】Player chose STAND.");
            StartCoroutine(ResolveRoutine());
        }


        private void InitializeDeck()
        {
            Deck.Clear();
            Hand.Clear();
            DiscardPile.Clear();

            foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
            {
                if (suit == CardSuit.None) continue;
                foreach (CardRank rank in System.Enum.GetValues(typeof(CardRank)))
                {
                    Deck.Add(new Card(suit, rank));
                }
            }
        }

        private void ShuffleDeck()
        {
            for (int i = 0; i < Deck.Count; i++)
            {
                Card temp = Deck[i];
                int randomIndex = Random.Range(i, Deck.Count);
                Deck[i] = Deck[randomIndex];
                Deck[randomIndex] = temp;
            }
            Debug.Log($"【Battle】Deck Shuffled. Count: {Deck.Count}");
        }

        private void DrawCard()
        {
            if (Deck.Count == 0)
            {
                Debug.LogWarning("【Battle】Deck is empty! Reshuffling Discard Pile...");
                return;
            }

            Card card = Deck[0];
            Deck.RemoveAt(0);
            Hand.Add(card);
            RecalculateGameState();
    
            OnCardDrawn?.Invoke(card);
            OnHandUpdate?.Invoke();
            
            Debug.Log($"【Battle】Player drew: {card}");
        }

        private void RecalculateGameState()
        {
            foreach (var card in Hand) card.ResetToDefault();

            if (CurrentBoss != null)
            {
                BossRuleLibrary.ApplyCardRules(CurrentBoss, Hand);
            }

            if (JokerManager.Instance != null)
            {
                JokerManager.Instance.ApplyPassiveEffects(Hand);
            }

            int currentThreshold = BossRuleLibrary.GetModifiedThreshold(CurrentBoss, 21);

            CurrentScoreContext = ScoreCalculator.Calculate(Hand, currentThreshold);

            JokerManager.Instance.TriggerJokers(JokerTriggerType.OnScoreResolve, CurrentScoreContext);

            if (CurrentScoreContext.IsBusted)
            {
                JokerManager.Instance.TriggerJokers(JokerTriggerType.OnBust, CurrentScoreContext);
            }

            Debug.Log($"【Score】Points: {CurrentScoreContext.TotalPoints} | " +
                      $"Mult: {CurrentScoreContext.Multiplier} | " +
                      $"Total: {CurrentScoreContext.GetScore()} | " +
                      $"Busted: {CurrentScoreContext.IsBusted}");
            
            if (JokerManager.Instance != null)
            {
                JokerManager.Instance.TriggerJokers(JokerTriggerType.OnScoreResolve, CurrentScoreContext);
                if (CurrentScoreContext.IsBusted)
                {
                    JokerManager.Instance.TriggerJokers(JokerTriggerType.OnBust, CurrentScoreContext);
                }
            }

            Debug.Log($"【Score】Threshold: {currentThreshold} | Points: {CurrentScoreContext.TotalPoints} | " +
                      $"Total: {CurrentScoreContext.GetScore()}");
            
        }

        private IEnumerator ResolveRoutine()
        {
            RecalculateGameState();
            long finalScore = CurrentScoreContext.GetScore();

            yield return new WaitForSeconds(1.0f);

            if (finalScore >= CurrentBoss.BaseBlindTarget)
            {
                Debug.Log("<color=green>ROUND WON!</color>");
                GameRunManager.Instance.OnRoundVictory();
            }
            else
            {
                Debug.Log("<color=red>ROUND LOST!</color>");
                GameRunManager.Instance.OnRoundDefeat();
            }
        }

        public void Debug_CheatDraw(Card card)
        {
            Hand.Add(card);
            RecalculateGameState();
        
            // 记得只 Invoke 一次！(刚才的教训)
            OnCardDrawn?.Invoke(card);
            OnHandUpdate?.Invoke();
        
            Debug.Log($"<color=magenta>[CHEAT]</color> Force drawn: {card}");
        }

        public void Debug_SetHandRoyalFlush()
        {
            Hand.Clear();

            Hand.Add(new Card(CardSuit.Spades, CardRank.Ten));
            Hand.Add(new Card(CardSuit.Spades, CardRank.Jack));
            Hand.Add(new Card(CardSuit.Spades, CardRank.Queen));
            Hand.Add(new Card(CardSuit.Spades, CardRank.King));
            Hand.Add(new Card(CardSuit.Spades, CardRank.Ace));

            RecalculateGameState();

            OnStateChanged?.Invoke(BattleState.PlayerTurn); 

            foreach(var c in Hand) OnCardDrawn?.Invoke(c);
        
            Debug.Log("<color=magenta>[CHEAT]</color> HAND SET TO ROYAL FLUSH!");
        }

    }
}
