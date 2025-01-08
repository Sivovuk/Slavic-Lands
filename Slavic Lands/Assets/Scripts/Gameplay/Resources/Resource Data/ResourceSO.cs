using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Resources
{
    
    public enum ResourceType
    {
        Wood,
        Stone,
        Hide,
        Food
    }

    [CreateAssetMenu(fileName = "New Resource", menuName = "Resource Data")]
    public class ResourceSO : ScriptableObject
    {
        public List<ResourceData> Resources = new List<ResourceData>();
        public float Health;
        public float XPMultiplayer = 1;
        public int XPReward = 1;
    }

    [System.Serializable]
    public class ResourceData
    {
        public ResourceType ResourceType;
        public int Amount;
    }

}