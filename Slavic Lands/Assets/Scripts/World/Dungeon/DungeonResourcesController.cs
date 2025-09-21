using System.Collections.Generic;
using Gameplay.Resources;
using UnityEngine;

namespace Gameplay.Dungeon
{
    /// <summary>
    /// Handles spawning and resetting resource nodes inside dungeon levels.
    /// - Clears previously spawned resources.
    /// - Instantiates new resources based on their ResourceSO dungeon configuration.
    /// </summary>
    public class DungeonResourcesController : MonoBehaviour
    {
        [SerializeField] private List<Resource> _resources = new List<Resource>();
        // List of all resource prefabs (wood, stone, ores, etc.) available in the dungeon.

        [SerializeField] private List<ResourcesSpawningLocation> _dungeonLevel = new List<ResourcesSpawningLocation>();
        // Contains dungeon levels and their associated resource spawn points.

        /// <summary>
        /// Resets and spawns dungeon resources for each dungeon level.
        /// </summary>
        public void UpdateDungeonResources()
        {
            // Iterate over each dungeon level
            for (int i = 0; i < _dungeonLevel.Count; i++)
            {
                // Clear existing resources from this level
                for (int j = 0; j < _dungeonLevel[i].Parent.childCount; j++)
                {
                    Destroy(_dungeonLevel[i].Parent.GetChild(j).gameObject);
                }

                List<Transform> spawnLocationsUsed = new List<Transform>();

                // For each type of resource prefab
                foreach (var resourceToSpawn in _resources)
                {
                    // Check this resource's allowed dungeon configurations
                    foreach (var resourceLevel in resourceToSpawn.ResourceSO.DungeonData)
                    {
                        // Skip if resource is not configured for this dungeon level
                        if (resourceLevel.DungeonLevels != _dungeonLevel[i].DungeonLevel)
                            continue;

                        int counter = 0;

                        // Try spawning resources at available locations
                        for (int k = 0; k < _dungeonLevel[i].SpawnLocations.Count; k++)
                        {
                            // Prevent using the same spawn point twice
                            if (spawnLocationsUsed.Contains(_dungeonLevel[i].SpawnLocations[k]))
                                continue;

                            // Limit how many of this resource type can spawn in this level
                            if (counter >= resourceLevel.MaxNodeAmount)
                                break;

                            // Spawn resource prefab at spawn point
                            Instantiate(
                                resourceToSpawn,
                                _dungeonLevel[i].SpawnLocations[k].position,
                                Quaternion.identity,
                                _dungeonLevel[i].Parent
                            );

                            // Mark this spawn location as used
                            spawnLocationsUsed.Add(_dungeonLevel[i].SpawnLocations[k]);
                            counter++;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Defines resource spawning configuration for a single dungeon level.
    /// Each level has a set of spawn points and a parent object for hierarchy organization.
    /// </summary>
    [System.Serializable]
    public class ResourcesSpawningLocation
    {
        public DungeonLevels DungeonLevel;                 // Which dungeon level this configuration belongs to
        public List<Transform> SpawnLocations = new();     // List of positions where resources may spawn
        public Transform Parent;                           // Parent object for organizing spawned resources in hierarchy
    }
}
