using UnityEngine;
using Model;
using Core;

namespace Systems
{
    public class GameRunManager : MonoBehaviour
    {
        public static GameRunManager Instance { get; private set; }

        public RunData CurrentRun;

        [Header("Config")]
        public BossData[] BossDatabase;
        [Header("UI Refs")]
        public UI.ShopPanelUI ShopUI;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            StartNewRun();
        }

        public void StartNewRun()
        {
            Debug.Log("【Run】Starting New Run...");
            CurrentRun = new RunData();
            
            Debug.Log($"【Run】Deck Size: {CurrentRun.PlayerDeck.Count}"); 

            JokerData starter = Resources.Load<JokerData>("Data/Jokers/J_Popcorn");
            if(starter) CurrentRun.OwnedJokers.Add(new Joker(starter));

            StartRound();
        }

        public void StartRound()
        {
            int targetScore = 1000 * CurrentRun.CurrentRound;

            BossData boss = BossDatabase[Random.Range(0, BossDatabase.Length)];

            BossData roundBoss = Instantiate(boss);
            roundBoss.BaseBlindTarget = targetScore;

            Debug.Log($"【Run】Round {CurrentRun.CurrentRound} Start! Boss: {roundBoss.BossName}, Target: {targetScore}");

            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.StartBattleFromRun(roundBoss, CurrentRun);
            }
        }

        public void OnRoundVictory()
        {
            Debug.Log("【Run】Victory! Going to Shop (Skipped for now)...");
            
            CurrentRun.CurrentRound++;
            CurrentRun.Money += 10;
                
            // 2. 初始化商店数据
            ShopManager.Instance.OpenShop();
            // 3. 显示商店 UI
            if (ShopUI != null) ShopUI.Show();

            StartRound();
        }
        
        // 新增：从商店进入下一关
        public void GoToNextRound()
        {
            // 1. 隐藏商店 UI
            if (ShopUI != null) ShopUI.Hide();

            // 2. 开始新一轮战斗
            StartRound();
        }

        public void OnRoundDefeat()
        {
            Debug.Log("【Run】DEFEAT. Run Reset.");
            StartNewRun();
        }
    }
}
