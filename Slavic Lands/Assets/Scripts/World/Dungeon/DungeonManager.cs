using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Dungeon
{
    /// <summary>
    /// Enum representing dungeon levels available in the game.
    /// Used for indexing levels and referencing them in UI or logic.
    /// </summary>
    public enum DungeonLevels
    {
        Level1 = 0,
        Level2 = 1,
        Level3 = 2
    }

    /// <summary>
    /// Manages all dungeon-related systems including:
    /// - Dungeon entrances
    /// - Dungeon resources
    /// - Dungeon mobs
    /// Acts as a singleton for global access.
    /// </summary>
    public class DungeonManager : MonoBehaviour
    {
        // List of all dungeon entrance controllers (one for each level)
        [field: SerializeField] public List<DungeonEntranceController> Levels = new List<DungeonEntranceController>();

        private DungeonResourcesController _dungeonResourcesController; // Handles spawning/updating dungeon resources
        private DungeonMobController _dungeonMobController;             // Handles spawning/updating dungeon mobs

        // Singleton reference
        public static DungeonManager Instance { get; private set; }

        private void Awake()
        {
            // Ensure singleton instance
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            // Initialize and update dungeon systems
            _dungeonResourcesController = GetComponent<DungeonResourcesController>();
            _dungeonResourcesController.UpdateDungeonResources();

            _dungeonMobController = GetComponent<DungeonMobController>();
            _dungeonMobController.UpdateDungeonMobs();
        }

        /// <summary>
        /// Gets the world position of the specified dungeon level.
        /// </summary>
        public Vector2 GetLevelPoint(int level)
        {
            return Levels[level].gameObject.transform.position;
        }

        /// <summary>
        /// Returns the entrance controller for the specified level.
        /// </summary>
        public DungeonEntranceController GetLevel(int level)
        {
            return Levels[level];
        }

        /// <summary>
        /// Called when entering a dungeon.
        /// If entering the first level, resets dungeon resources and mobs.
        /// </summary>
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
