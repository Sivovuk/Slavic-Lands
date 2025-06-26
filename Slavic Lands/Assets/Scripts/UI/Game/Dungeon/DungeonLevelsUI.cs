using System;
using System.Collections.Generic;
using Gameplay.Dungeon;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.Dungeon
{
    public class DungeonLevelsUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private List<Button> _levels = new List<Button>();
        private DungeonEntranceController _currentEntranceController;

        public static DungeonLevelsUI Instance;

        private void Start()
        {
            UpdateLevelsUI();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public void Open(DungeonEntranceController entranceController)
        {
            _panel.SetActive(true);
            _currentEntranceController = entranceController;
        }

        public void UpdateLevelsUI()
        {
            for (int i = 0; i < DungeonManager.Instance.Levels.Count; i++)
            {
                if (DungeonManager.Instance.Levels[i].IsLevelActivated)
                    _levels[i].interactable = true;
                else
                    _levels[i].interactable = false;
            }
        }

        public void SelectLevel(int level)
        {
            _currentEntranceController.EnterDungeon(level);
            _panel.SetActive(false);
        }
    }
}
