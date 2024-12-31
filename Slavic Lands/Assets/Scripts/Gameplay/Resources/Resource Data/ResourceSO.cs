using UnityEngine;

namespace Gameplay.Resources
{
    
    public enum ResourceType
    {
        Wood,
        Stone,
        Hide
    }

    [CreateAssetMenu(fileName = "New Resource", menuName = "Resource Data")]
    public class ResourceSO : ScriptableObject
    {
        public ResourceType ResourceType;
        public float Health;
        public int Amount;
    }
}