using System;
using Gameplay.Player;
using UI.Game.Dungeon;
using UnityEngine;

namespace Gameplay.Dungeon
{
    public class DungeonEntranceController : MonoBehaviour
    {
        [SerializeField] private Transform _positionToSend;
        private GameObject _target;
        
        private PlayerInputSystem _playerInputSystem;
        private DungeonManager _dungeonManager;

        private bool _isPlayerIn;
        [field:SerializeField] public bool IsLevelActivated { get; private set; }

        private void Awake()
        {
            _dungeonManager = GetComponentInParent<DungeonManager>();
            _playerInputSystem = GetComponent<PlayerInputSystem>();
        }

        private void OnEnable()
        {
            _playerInputSystem.OnInteractionClick += OpenDungeonLevels;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnInteractionClick -= OpenDungeonLevels;
        }

        private void OpenDungeonLevels()
        {
            if (!_isPlayerIn || _positionToSend == null) return;
            _positionToSend.GetComponent<DungeonEntranceController>().SetLevelActive();
            DungeonLevelsUI.Instance.UpdateLevelsUI();
            DungeonLevelsUI.Instance.Open(this);
        }

        public void EnterDungeon(int levelIndex)
        {
            if (_target == null || _positionToSend == null) return;

            Vector2 newPoint = _dungeonManager.GetLevelPoint(levelIndex);
            _target.transform.position = new Vector2(newPoint.x, newPoint.y);
            _dungeonManager.GetLevel(levelIndex).SetLevelActive();
            _dungeonManager.EnterDungeon(this);
        }

        public void SetLevelActive()
        {
            IsLevelActivated = true;
        }

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