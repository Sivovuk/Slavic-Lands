using System;
using System.Collections.Generic;
using Gameplay.Resources;
using SaveSystem;
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

        public List<ResourceSaveData> ToSaveData()
        {
            var data = new List<ResourceSaveData>();
            foreach (var resource in _resources)
            {
                data.Add(new ResourceSaveData
                {
                    ResourceType = resource.Key,
                    Amount = resource.Value
                });
            }

            return data;
        }

        public void LoadFromSaveData(IEnumerable<ResourceSaveData> resources)
        {
            _resources.Clear();

            if (resources == null)
                return;

            foreach (var resource in resources)
            {
                if (resource.Amount < 0)
                    continue;

                _resources[resource.ResourceType] = resource.Amount;
            }
        }

        public void AddResource(int amount, ResourceType resourceType)
        {
            if (!_resources.ContainsKey(resourceType))
                _resources[resourceType] = 0;

            _resources[resourceType] += amount;

            OnResourceChanged?.Invoke(resourceType, _resources[resourceType]);
        }
    }
}