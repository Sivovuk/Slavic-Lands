using System;
using System.Collections.Generic;
using Data;
using Gameplay.Resources;
using Interfaces;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour, ILoadingStatsPlayer, IHit
    {
        private float _maxHealth;
        private float _currentHealth;
        
        [SerializeField] private bool _isDead;
        
        public Action OnDeath;
        public Action<float, float> OnHealthChanged;
        
        private Player _player;

        public void Heal(float healAmount)
        {
            ModifyHealth(healAmount);
        }

        public bool TakeDamage(float damage, Action<List<ResourceData>, ResourceSO> callback = null)
        {
            return ModifyHealth(-damage, callback);
        }

        public void ApplyForce(float force, Vector2 direction)
        {
            TryGetComponent<Rigidbody2D>(out Rigidbody2D rg);
            rg.AddForce(direction *  force, ForceMode2D.Force);
        }

        public List<ResourceData> GetResourceType()
        {
            return null;
        }

        private bool ModifyHealth(float amount, Action<List<ResourceData>, ResourceSO> callback = null)
        {
            if (_isDead) return false;
            
            _currentHealth += amount;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                _isDead = true;
                OnDeath?.Invoke();
                
                return true;
            }
            
            return false;
        }

        public void LoadPlayerStats(PlayerSO playerSO, Player player)
        {
            _player = player;
            _maxHealth = playerSO.BaseHealth + (_player.CurrentLevel * playerSO.LevelMultiplayer);
            ModifyHealth(PlayerPrefs.GetFloat(Constants.SavedHealth, _maxHealth));
        }

    }
}