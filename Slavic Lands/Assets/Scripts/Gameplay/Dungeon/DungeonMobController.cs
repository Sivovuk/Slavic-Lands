using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Dungeon
{
    public class DungeonMobController : MonoBehaviour
    {
        [SerializeField] private List<NPC.NPC> _mobs = new List<NPC.NPC>();
        [SerializeField] private List<MobSpawningLocation> _dungeonLevel = new List<MobSpawningLocation>();

        public void UpdateDungeonMobs()
        {
            for (int i = 0; i < _dungeonLevel.Count; i++)
            {
                for (int j = 0; j < _dungeonLevel[i].Parent.childCount; j++)
                {
                    Destroy(_dungeonLevel[i].Parent.GetChild(j).gameObject);
                }
                
                List<Transform> spawnLocationsUsed = new List<Transform>();

                foreach (var mobToSpawn in _mobs)
                {
                    foreach (var mobLevel in mobToSpawn.DungeonData)
                    {
                        if (mobLevel.DungeonLevels != _dungeonLevel[i].DungeonLevel)
                            continue;

                        int counter = 0;

                        for (int k = 0; k < _dungeonLevel[i].SpawnLocations.Count; k++)
                        {
                            if (spawnLocationsUsed.Contains(_dungeonLevel[i].SpawnLocations[k]))
                                continue;

                            if (counter >= mobLevel.MaxNodeAmount)
                                break;

                            Instantiate(mobToSpawn,
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
    public class MobSpawningLocation
    {
        public DungeonLevels DungeonLevel;
        public List<Transform> SpawnLocations = new List<Transform>();
        public Transform Parent;
    }
}