using System;
using System.Collections.Generic;
using Core.Interfaces;
using Data;
using Gameplay.Dungeon;
using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Resources
{
    
    public enum ResourceType
    {
        Wood,
        Stone,
        Hide,
        Food,
        Coal,
        Iron,
        Gold,
        Crystal
    }
    
    public class Resource : MonoBehaviour, IDamageable, ILoadStatsResource
    {
        private float _maxHealth;
        private float _currentHealth;
        
        [SerializeField] private List<ResourceData> _resourceData = new  List<ResourceData>();
        
        [field:SerializeField] public List<DungeonData>  DungeonData = new  List<DungeonData>();

        [field: SerializeField] public ResourceSO _resourceSO { get; private set; }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            LoadResourceData(_resourceSO);
        }

        public void LoadResourceData(ResourceSO resourceSO)
        {
            _maxHealth = resourceSO.Health;
            _currentHealth = resourceSO.Health;
            _resourceData = resourceSO.Resources;
            
            // dodaj ucitavanje iz save-a
        }

        public List<ResourceData> GetResourceType()
        {
            return _resourceData;
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                HandleBreak();
            }
        }
        
        private void HandleBreak(Action callback = null)
        {
            // Drop loot + grant XP
            var player = PlayerController.Instance;

            foreach (var resource in _resourceData)
            {
                player.AddResource(resource.DropAmount, resource.ResourceType);
            }

            player.PlayerProfile.TryAddXp(_resourceSO.ToolType, _resourceSO.XPReward);
            Destroy(gameObject);
        }

        public virtual void ApplyKnockback(Vector2 direction, float force)
        {
            
        }

        public bool IsDead { get; }
    }

    [System.Serializable]
    public class DungeonData
    {
        public DungeonLevels DungeonLevels;
        public int MaxNodeAmount;
    }
}