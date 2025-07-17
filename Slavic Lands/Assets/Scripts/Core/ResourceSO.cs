using System.Collections.Generic;
using Core.Interfaces;
using Gameplay.Resources;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "New Resource", menuName = "Resource Data")]
    public class ResourceSO : ScriptableObject
    {
        public List<ResourceData> Resources = new List<ResourceData>();
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