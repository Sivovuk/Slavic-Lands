using System.Collections.Generic;
using Core.Interfaces;
using Gameplay.Resources;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Resource", menuName = "Resource Data")]
    public class ResourceSO : ScriptableObject
    {
        [SerializeField] 
        private List<ResourceData> _resourceData = new List<ResourceData>();
        public IReadOnlyList<ResourceData> ResourceData => _resourceData;
        
        [SerializeField] 
        private List<DungeonData> _dungeonData = new List<DungeonData>();
        public IReadOnlyList<DungeonData> DungeonData => _dungeonData;
        
        public ToolType ToolType;
        public float Health;
        public int XPReward = 1;
    }

    [System.Serializable]
    public class ResourceData
    {
        public ResourceType ResourceType;
        public int DropAmount;
    }

}