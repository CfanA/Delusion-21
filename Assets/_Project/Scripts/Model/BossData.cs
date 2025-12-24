using UnityEngine;

namespace Model
{
    [CreateAssetMenu(fileName = "NewBoss", menuName = "Delusion21/Boss Data")]
    public class BossData : ScriptableObject
    {
        [Header("Boss Identity")]
        public string ID;
        public string BossName;
        [TextArea] public string Description;
        public Sprite Portrait;

        [Header("Level Constraints")]
        public int BaseBlindTarget;
        public int HandsAllowed = 4;
        public int DiscardsAllowed = 3;

        [Header("Rule Modification")]
        public string RuleModifierID; 
    }
}