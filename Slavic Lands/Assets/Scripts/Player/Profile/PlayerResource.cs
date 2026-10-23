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

        public bool HasResource(ResourceType type, int amount)
        {
            if (amount <= 0) return true;
            return GetResource(type) >= amount;
        }

        public bool TrySpendResource(ResourceType type, int amount)
        {
            if (amount < 0) return false;
            if (amount == 0) return true;
            if (!HasResource(type, amount)) return false;

            _resources[type] -= amount;
            OnResourceChanged?.Invoke(type, _resources[type]);
            return true;
        }

        public int GetGold()
        {
            return GetResource(ResourceType.Gold);
        }

        public void AddGold(int amount)
        {
            AddResource(amount, ResourceType.Gold);
        }

        public bool TrySpendGold(int amount)
        {
            return TrySpendResource(ResourceType.Gold, amount);
        }
    }
}