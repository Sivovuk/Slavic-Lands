using System;
using Interfaces;
using UnityEngine;

namespace Gameplay.Resources
{
    public class Resource : Health, IHit, ILoadStatsResource
    {
        [SerializeField] protected int _resourceAmount;
        
        [SerializeField] protected ResourceType _resourceType;
        
        [SerializeField] private ResourceSO _resourceSO;

        public override void Init()
        {
            LoadResourceStats(_resourceSO);
        }

        protected override void ResourceCollection(Action<ResourceType, int> callback = null)
        {
            callback?.Invoke(_resourceType, _resourceAmount);
        }

        public void LoadResourceStats(ResourceSO resourceSO)
        {
            _maxHealth = resourceSO.Health;
            _currentHealth = resourceSO.Health;
            _resourceAmount = resourceSO.Amount;
            _resourceType = resourceSO.ResourceType;
            
            // dodaj ucitavanje iz save-a
        }

        public ResourceType GetResourceType()
        {
            return _resourceType;
        }
    }
}