using System.Collections.Generic;
using UnityEngine;
using Model;
using Core;
using System.Linq;

namespace Systems
{
    public class JokerManager : MonoBehaviour
    {
        public static JokerManager Instance { get; private set; }

        public List<Joker> ActiveJokers = new List<Joker>();
        public int MaxJokerSlots = 5;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
        }

        public void AddJoker(JokerData data)
        {
            if (ActiveJokers.Count >= MaxJokerSlots)
            {
                Debug.LogWarning("Joker slots full!");
                return;
            }
            ActiveJokers.Add(new Joker(data));
            Debug.Log($"Added Joker: {data.JokerName}");
        }

        public void TriggerJokers(JokerTriggerType type, object context)
        {
            var sortedJokers = ActiveJokers.OrderByDescending(j => j.Data.Priority).ToList();

            foreach (var joker in sortedJokers)
            {
                JokerLogicLibrary.Execute(joker, type, context);
            }
        }

        public void ApplyPassiveEffects(List<Card> hand)
        {
            foreach (var joker in ActiveJokers)
            {
                if (joker.Data.TriggerType == JokerTriggerType.Passive)
                {
                    if (joker.Data.ID == "J_Faceless") 
                    {
                        foreach (var c in hand)
                        {
                             if (c.IsFaceCard) c.CurrentPoints = 0;
                        }
                    }
                }
            }
        }
    }
}
