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
        
        [field: SerializeField] public ResourceSO ResourceSO { get; private set; }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            LoadResourceData(ResourceSO);
        }

        public void LoadResourceData(ResourceSO resourceSO)
        {
            _maxHealth = resourceSO.Health;
            _currentHealth = resourceSO.Health;
            
            // dodaj ucitavanje iz save-a
        }

        public IReadOnlyList<ResourceData> GetResourceType()
        {
            return ResourceSO.ResourceData;
        }

        public void TakeDamage(float damage, ToolType toolType)
        {
            if (toolType != ResourceSO.ToolType) return;
            
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                HandleBreak();
            }
        }
        
        private void HandleBreak(Action callback = null)
        {
            var player = PlayerController.Instance;

            foreach (var resource in ResourceSO.ResourceData)
            {
                player.AddResource(resource.DropAmount, resource.ResourceType);
            }

            player.PlayerProfile.PlayerLevelData.AddXp(ResourceSO.XPReward);
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