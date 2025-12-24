using UnityEngine;
using DG.Tweening;

namespace Core
{
    public class GameApp : MonoBehaviour
    {
        public static GameApp Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitServices();
        }

        private void InitServices()
        {
            DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(200, 10);
        }
    }
}