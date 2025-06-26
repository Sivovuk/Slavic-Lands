using System;
using System.Collections.Generic;
using Gameplay.Dungeon;
using Gameplay.NPC;
using Interfaces;
using UnityEngine;

namespace Gameplay.Resources
{
    public class Resource : Health, IHit, ILoadStatsResource
    {
        [SerializeField] private List<ResourceData> _resourceData = new  List<ResourceData>();
        
        [field:SerializeField] public List<DungeonData>  DungeonData = new  List<DungeonData>();

        [field: SerializeField] public ResourceSO _resourceSO { get; private set; }

        public override void Init()
        {
            LoadResourceStats(_resourceSO);
        }

        protected override void ResourceCollection(Action<List<ResourceData>, ResourceSO> callback = null)
        {
            callback?.Invoke(_resourceData, _resourceSO);
        }

        public void LoadResourceStats(ResourceSO resourceSO)
        {
            _maxHealth = resourceSO.Health;
            _currentHealth = resourceSO.Health;
            _resourceData = resourceSO.Resources;
            
            // dodaj ucitavanje iz save-a
        }

        public virtual void ApplyForce(float force, Vector2 direction)
        {
            
        }

        public List<ResourceData> GetResourceType()
        {
            return _resourceData;
        }
    }

    [System.Serializable]
    public class DungeonData
    {
        public DungeonLevels DungeonLevels;
        public int MaxNodeAmount;
    }
}