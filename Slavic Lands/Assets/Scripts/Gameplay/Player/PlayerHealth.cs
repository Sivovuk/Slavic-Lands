using System;
using Gameplay.Resources;
using Interfaces;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour, ILoadingStatsPlayer, IHit
    {
        private float _maxHealth;
        private float _currentHealth;
        
        private bool _isDead;
        
        public Action OnDeath;
        public Action<float, float> OnHealthChanged;
        
        private Player _player;

        public void Heal(float healAmount)
        {
            ModifyHealth(healAmount);
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