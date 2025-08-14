using System;
using System.Collections.Generic;
using Gameplay.Resources;
using UnityEngine;

namespace Gameplay.Player
{
    [System.Serializable]
    public class PlayerResource
    {
        private readonly Dictionary<ResourceType, int> _resources = new();

        public event Action<ResourceType, int> OnResourceChanged;

        public int GetResource(ResourceType type) => 
            _resources.TryGetValue(type, out var amount) ? amount : 0;

        public void AddResource(int amount, ResourceType resourceType)
        {
            if (!_resources.ContainsKey(resourceType))
                _resources[resourceType] = 0;

            _resources[resourceType] += amount;

            OnResourceChanged?.Invoke(resourceType, _resources[resourceType]);
        }
    }
}