using UnityEngine;
using System.Collections.Generic;
using Model;
using Core;

namespace Systems
{
    public class DebugController : MonoBehaviour
    {
        [Header("Settings")]
        public bool IsDebugMode = true;

        private Rect _windowRect = new Rect(20, 20, 350, 600);
        private bool _showGUI = false;

        private void Update()
        {
            if (!IsDebugMode) return;

            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                _showGUI = !_showGUI;
            }
        }

        private void OnGUI()
        {
            if (!_showGUI) return;
            _windowRect = GUI.Window(0, _windowRect, DrawWindowContent, "🔧 GOD MODE (开发者面板)");
        }

        private void DrawWindowContent(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            GUILayout.BeginVertical();

            GUILayout.Label("<b>[ 数据监控 ]</b>");
            if (BattleManager.Instance != null)
            {
                GUILayout.Label($"Deck Count: {BattleManager.Instance.Deck.Count}");
                GUILayout.Label($"Hand Count: {BattleManager.Instance.Hand.Count}");
                
                var ctx = BattleManager.Instance.CurrentScoreContext;
                if (ctx != null)
                {
                    string color = ctx.IsBusted ? "red" : "white";
                    GUILayout.Label($"Score: <color={color}>{ctx.GetScore()}</color>");
                    GUILayout.Label($"Formula: ({ctx.BaseChips} + {ctx.TotalPoints}) x {ctx.Multiplier}");
                }
            }

            GUILayout.Space(10);
            
            GUILayout.Label("<b>[ 资源修改 ]</b>");
            if (GUILayout.Button("💰 Add $100 Money"))
            {
                if (GameRunManager.Instance != null)
                    GameRunManager.Instance.CurrentRun.Money += 100;
            }

            if (GUILayout.Button("🃏 Get 'Popcorn' Joker"))
            {
                JokerData joker = Resources.Load<JokerData>("Data/Jokers/J_Popcorn");
                if (joker) JokerManager.Instance.AddJoker(joker);
            }

            if (GUILayout.Button("🃏 Get 'The Void' Joker"))
            {
                JokerData joker = Resources.Load<JokerData>("Data/Jokers/J_Void");
                if (joker) JokerManager.Instance.AddJoker(joker);
            }

            GUILayout.Space(10);

            GUILayout.Label("<b>[ 牌局操控 ]</b>");
            
            if (GUILayout.Button("✨ Draw Ace of Spades (Cheat)"))
            {
                Card cheatCard = new Card(CardSuit.Spades, CardRank.Ace);
                BattleManager.Instance.Debug_CheatDraw(cheatCard);
            }

            if (GUILayout.Button("💣 Force BUST (Draw 3 Kings)"))
            {
                BattleManager.Instance.Debug_CheatDraw(new Card(CardSuit.Hearts, CardRank.King));
                BattleManager.Instance.Debug_CheatDraw(new Card(CardSuit.Diamonds, CardRank.King));
                BattleManager.Instance.Debug_CheatDraw(new Card(CardSuit.Clubs, CardRank.King));
            }

            if (GUILayout.Button("🔥 Set Hand to Royal Flush"))
            {
                BattleManager.Instance.Debug_SetHandRoyalFlush();
            }

            GUILayout.EndVertical();
        }
    }
}
