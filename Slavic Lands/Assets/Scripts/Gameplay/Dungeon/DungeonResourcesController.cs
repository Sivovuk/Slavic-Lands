using System.Collections.Generic;
using Gameplay.Resources;
using UnityEngine;

namespace Gameplay.Dungeon
{
    public class DungeonResourcesController : MonoBehaviour
    {
        [SerializeField] private List<Resource> _resources = new List<Resource>();
        [SerializeField] private List<ResourcesSpawningLocation> _dungeonLevel = new List<ResourcesSpawningLocation>();

        public void UpdateDungeonResources()
        {
            Debug.Log("UpdateDungeonResources");
            for (int i = 0; i < _dungeonLevel.Count; i++)
            {
                for (int j = 0; j < _dungeonLevel[i].Parent.childCount; j++)
                {
                    Destroy(_dungeonLevel[i].Parent.GetChild(j).gameObject);
                }
                
                List<Transform> spawnLocationsUsed = new List<Transform>();

                foreach (var resourceToSpawn in _resources)
                {
                    foreach (var resourceLevel in resourceToSpawn.DungeonData)
                    {
                        if (resourceLevel.DungeonLevels != _dungeonLevel[i].DungeonLevel)
                            continue;

                        int counter = 0;

                        for (int k = 0; k < _dungeonLevel[i].SpawnLocations.Count; k++)
                        {
                            if (spawnLocationsUsed.Contains(_dungeonLevel[i].SpawnLocations[k]))
                                continue;

                            if (counter >= resourceLevel.MaxNodeAmount)
                                break;

                            Instantiate(resourceToSpawn,
                                _dungeonLevel[i].SpawnLocations[k].position,
                                Quaternion.identity,
                                _dungeonLevel[i].Parent);

                            spawnLocationsUsed.Add(_dungeonLevel[i].SpawnLocations[k]);
                            counter++;
                        }
                    }
                }
            }
        }
    }

    
    [System.Serializable]
    public class ResourcesSpawningLocation
    {
        public DungeonLevels DungeonLevel;
        public List<Transform> SpawnLocations = new List<Transform>();
        public Transform Parent;
    }

}