using System;
using Gameplay.Resources;
using UnityEngine;

namespace Gameplay
{
    public class Health : MonoBehaviour
    {
        [SerializeField] protected float _maxHealth;
        [SerializeField] protected float _currentHealth;

        protected bool _isDead;
        
        public Action OnDeath;
        public Action<float, float> OnHealthChanged;
        
        protected void Start()
        {
            Init();
        }

        public virtual void Init()
        {
            
        }
        
        public virtual bool TakeDamage(float damage, Action<ResourceType, int> callback = null)
        {
            return ModifyHealth(-damage, callback);
        }

        protected virtual bool ModifyHealth(float amount, Action<ResourceType, int> callback = null)
        {
            if (_isDead) return false;
            
            _currentHealth += amount;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                _isDead = true;
                OnDeath?.Invoke();
                ResourceCollection(callback);
                Destroy(gameObject);
                return true;
            }
            
            return false;
        }

        protected virtual void ResourceCollection(Action<ResourceType, int> callback = null)
        {
            
        }

    }
}