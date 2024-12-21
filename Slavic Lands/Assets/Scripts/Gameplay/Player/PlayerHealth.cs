using System;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour, ILoadingStats
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

        public void TakeDamage(float damage)
        {
            ModifyHealth(-damage);
        }

        public void ModifyHealth(float amount)
        {
            if (_isDead)return;
            
            _currentHealth += amount;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                _isDead = true;
                OnDeath?.Invoke();
            }
        }

        public void LoadPlayerStats(PlayerSO playerSO, Player player)
        {
            _player = player;
            _maxHealth = playerSO.BaseHealth + (_player.CurrentLevel * playerSO.LevelMultiplayer);
            ModifyHealth(PlayerPrefs.GetFloat(Constants.SavedHealth, _maxHealth));
        }

    }
}