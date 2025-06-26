using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Dungeon
{
    
    public enum DungeonLevels
    {
        Level1 = 0,
        Level2 = 1,
        Level3 = 2
    }
    
    public class DungeonManager : MonoBehaviour
    {
        [field:SerializeField] public List<DungeonEntranceController> Levels = new List<DungeonEntranceController>();

        private DungeonResourcesController _dungeonResourcesController;
        private DungeonMobController _dungeonMobController;
        
        public static DungeonManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            _dungeonResourcesController = GetComponent<DungeonResourcesController>();
            _dungeonResourcesController.UpdateDungeonResources();
            _dungeonMobController = GetComponent<DungeonMobController>();
            _dungeonMobController.UpdateDungeonMobs();
        }

        public Vector2 GetLevelPoint(int level)
        {
            return Levels[level].gameObject.transform.position;
        }

        public DungeonEntranceController GetLevel(int level)
        {
            return Levels[level];
        }

        public void EnterDungeon(DungeonEntranceController level)
        {
            if (Levels.IndexOf(level) == 0)
            {
                _dungeonResourcesController.UpdateDungeonResources();
                _dungeonMobController.UpdateDungeonMobs();
            }
        }
    }
}