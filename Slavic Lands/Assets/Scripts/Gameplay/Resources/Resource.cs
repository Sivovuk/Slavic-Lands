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
        
        

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            LoadResourceStats(_resourceSO);
        }

        public bool TakeDamage(float damage, Action<ResourceType, int> callback = null)
        {
            return ModifyHealth(-damage, callback);
        }

        private bool ModifyHealth(float amount, Action<ResourceType, int> callback = null)
        {
            if (_isDead) return false;
            
            _currentHealth += amount;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                _isDead = true;
                OnDeath?.Invoke();
                callback?.Invoke(_resourceType, _resourceAmount);
                Destroy(gameObject);
                return true;
            }
            
            return false;
        }

        public void LoadResourceStats(ResourceSO resourceSO)
        {
            _maxHealth = resourceSO.Health;
            _currentHealth = resourceSO.Health;
            _resourceAmount = resourceSO.Amount;
            _resourceType = resourceSO.ResourceType;
            
            // dodaj ucitavanje iz save-a
        }
    }
}