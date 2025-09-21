using System;
using Gameplay.Player;
using UI.Game.Dungeon;
using UnityEngine;

namespace Gameplay.Dungeon
{
    /// <summary>
    /// Controls the dungeon entrance logic.
    /// - Detects when the player enters or exits the dungeon entrance trigger.
    /// - Opens the dungeon level selection UI when the player interacts.
    /// - Moves the player into the chosen dungeon level.
    /// </summary>
    public class DungeonEntranceController : MonoBehaviour
    {
        [SerializeField] private Transform _positionToSend;   // Destination point inside dungeon
        private GameObject _target;                          // Reference to the player

        private PlayerInputSystem _playerInputSystem;
        private DungeonManager _dungeonManager;

        private bool _isPlayerIn;                            // Whether the player is in the entrance trigger
        [field: SerializeField] public bool IsLevelActivated { get; private set; } // Whether this dungeon level is already active

        private void Awake()
        {
            _dungeonManager = GetComponentInParent<DungeonManager>();
            _playerInputSystem = GetComponent<PlayerInputSystem>();
        }

        private void OnEnable()
        {
            // Subscribe to interaction input
            _playerInputSystem.OnInteractionClick += OpenDungeonLevels;
        }

        private void OnDisable()
        {
            // Unsubscribe to prevent memory leaks
            _playerInputSystem.OnInteractionClick -= OpenDungeonLevels;
        }

        /// <summary>
        /// Opens dungeon level selection UI when the player interacts with entrance.
        /// </summary>
        private void OpenDungeonLevels()
        {
            if (!_isPlayerIn || _positionToSend == null) return;

            // Activate dungeon entrance
            _positionToSend.GetComponent<DungeonEntranceController>().SetLevelActive();

            // Update and open dungeon UI
            DungeonLevelsUI.Instance.UpdateLevelsUI();
            DungeonLevelsUI.Instance.Open(this);
        }

        /// <summary>
        /// Moves the player into the dungeon at the chosen level index.
        /// Updates dungeon state in DungeonManager.
        /// </summary>
        public void EnterDungeon(int levelIndex)
        {
            if (_target == null || _positionToSend == null) return;

            // Get spawn point for the chosen dungeon level
            Vector2 newPoint = _dungeonManager.GetLevelPoint(levelIndex);

            // Move player to that point
            _target.transform.position = new Vector2(newPoint.x, newPoint.y);

            // Mark level as active and register dungeon entry
            _dungeonManager.GetLevel(levelIndex).SetLevelActive();
            _dungeonManager.EnterDungeon(this);
        }

        /// <summary>
        /// Marks this entrance as active (used when dungeon becomes available).
        /// </summary>
        public void SetLevelActive()
        {
            IsLevelActivated = true;
        }

        // --- PLAYER DETECTION ---

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerIn = true;
                _target = other.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerIn = false;
                _target = null;
            }
        }
    }
}
