using UnityEngine;
using Core;

namespace Model
{
    [CreateAssetMenu(fileName = "NewJoker", menuName = "Delusion21/Joker Data")]
    public class JokerData : ScriptableObject
    {
        [Header("Basic Info")]
        public string ID;
        public string JokerName;
        [TextArea] public string Description;
        public Sprite Icon;
        public Rarity Rarity;
        public int Price;

        [Header("Logic Config")]
        public JokerTriggerType TriggerType;
        
        [Tooltip("优先级：数值越大越先触发。例如 '先变0点' 应该在 '计算倍率' 之前")]
        public int Priority = 0;

        [Header("Effect Parameters (Generic)")]

        public float EffectValue1; 
        public float EffectValue2;

        public bool IsConsumable = false; 
    }
}