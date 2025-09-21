using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace Gameplay.Dungeon
{
    /// <summary>
    /// Handles spawning and resetting mobs inside dungeon levels.
    /// - Clears existing mobs from spawn locations.
    /// - Instantiates new mobs based on dungeon level data.
    /// </summary>
    public class DungeonMobController : MonoBehaviour
    {
        [SerializeField] private List<NPC> _mobs = new List<NPC>(); 
        // List of all available mob prefabs (NPCs) that can spawn in dungeon levels.

        [SerializeField] private List<MobSpawningLocation> _dungeonLevel = new List<MobSpawningLocation>(); 
        // Contains dungeon levels and their associated spawn points.

        /// <summary>
        /// Resets mobs across all dungeon levels and respawns them
        /// according to each mob’s dungeon data configuration.
        /// </summary>
        public void UpdateDungeonMobs()
        {
            // Loop through each dungeon level
            for (int i = 0; i < _dungeonLevel.Count; i++)
            {
                // Clear existing mobs from the level’s parent container
                for (int j = 0; j < _dungeonLevel[i].Parent.childCount; j++)
                {
                    Destroy(_dungeonLevel[i].Parent.GetChild(j).gameObject);
                }

                List<Transform> spawnLocationsUsed = new List<Transform>();

                // For each mob prefab in the game
                foreach (var mobToSpawn in _mobs)
                {
                    // Check the mob's allowed dungeon data
                    foreach (var mobLevel in mobToSpawn.DungeonData)
                    {
                        // Only spawn mobs that belong to this dungeon level
                        if (mobLevel.DungeonLevels != _dungeonLevel[i].DungeonLevel)
                            continue;

                        int counter = 0;

                        // Try spawning mobs at the available locations
                        for (int k = 0; k < _dungeonLevel[i].SpawnLocations.Count; k++)
                        {
                            // Prevent double-use of spawn points
                            if (spawnLocationsUsed.Contains(_dungeonLevel[i].SpawnLocations[k]))
                                continue;

                            // Limit the number of spawns per mob type in this level
                            if (counter >= mobLevel.MaxNodeAmount)
                                break;

                            // Instantiate mob at the spawn location under the level parent
                            Instantiate(
                                mobToSpawn,
                                _dungeonLevel[i].SpawnLocations[k].position,
                                Quaternion.identity,
                                _dungeonLevel[i].Parent
                            );

                            // Mark this location as used
                            spawnLocationsUsed.Add(_dungeonLevel[i].SpawnLocations[k]);
                            counter++;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Defines spawning configuration for one dungeon level.
    /// Each level has multiple spawn points and a parent object to organize them.
    /// </summary>
    [System.Serializable]
    public class MobSpawningLocation
    {
        public DungeonLevels DungeonLevel;                  // The dungeon level this data belongs to
        public List<Transform> SpawnLocations = new();      // Available spawn points for mobs
        public Transform Parent;                            // Parent transform to organize spawned mobs in hierarchy
    }
}
