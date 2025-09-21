using System;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// Singleton manager responsible for central game-level events and player initialization.
    /// Provides an event-based way to notify systems when the player is ready.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Singleton instance for global access
        public static GameManager Instance { get; private set; }

        // Event triggered when the player has finished initialization
        public Action OnPlayerInit;

        private void Awake()
        {
            // Basic singleton setup
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject); // Prevent duplicates
        }

        /// <summary>
        /// Invoked externally when the player is initialized.
        /// Notifies all subscribed systems via OnPlayerInit event.
        /// </summary>
        public void PlayerInit()
        {
            Debug.Log("Player init");
            OnPlayerInit?.Invoke();
        }
    }
}