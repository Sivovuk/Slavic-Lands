using System.Collections.Generic;
using Core.Interfaces;
using Gameplay.Resources;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// ScriptableObject defining a resource node's data (e.g., trees, rocks, minerals).
    /// Contains what tools can interact with it, how much health it has, what it drops, and how much XP is rewarded.
    /// </summary>
    [CreateAssetMenu(fileName = "New Resource", menuName = "Resource Data")]
    public class ResourceSO : ScriptableObject
    {
        // --- RESOURCE DROP CONFIGURATION ---

        [SerializeField] 
        private List<ResourceData> _resourceData = new List<ResourceData>();

        // Read-only access to the list of resource drops
        public IReadOnlyList<ResourceData> ResourceData => _resourceData;
        
        // --- DUNGEON DROPS (OR SPECIAL LOOT) ---

        [SerializeField] 
        private List<DungeonData> _dungeonData = new List<DungeonData>();

        // Read-only access to dungeon-specific drops
        public IReadOnlyList<DungeonData> DungeonData => _dungeonData;
        
        // --- INTERACTION & PROGRESSION ---

        // Tool type required to interact with or harvest this resource (e.g., axe, pickaxe)
        public ToolType ToolType;

        // Total health of the resource node (how many hits it can take)
        public float Health;

        // XP granted to the player upon harvesting or destroying
        public int XPReward = 1;
    }

    /// <summary>
    /// Represents a specific item type and how much is dropped from the resource.
    /// </summary>
    [System.Serializable]
    public class ResourceData
    {
        public ResourceType ResourceType; // Type of resource (e.g., wood, stone)
        public int DropAmount;           // Amount given when this resource is harvested
    }
}