using System;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Action OnPlayerInit;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void PlayerInit()
        {
            Debug.Log("Player init");
            OnPlayerInit?.Invoke();
        }
    }
}