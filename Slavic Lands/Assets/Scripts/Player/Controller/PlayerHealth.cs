using System;
using System.Collections.Generic;
using Core.Constants;
using Core.Interfaces;
using Data;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour, ILoadingStatsPlayer, IDamageable
    {
        private float _maxHealth;
        private float _currentHealth;
        private bool _isDead;
        
        public bool IsDead => _isDead;
        
        public Action OnDeath;
        public Action<float, float> OnHealthChanged;
        
        private PlayerController _playerController;

        public void LoadPlayerStats(PlayerSO playerSO, PlayerController playerController)
        {
            _playerController = playerController;
            _maxHealth = playerSO.BaseHealth + (_playerController.PlayerProfile.PlayerLevelData.CurrentLevel * playerSO.LevelMultiplayer);
            _currentHealth = PlayerPrefs.GetFloat(Constants.SavedHealth, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
        
        public void Heal(float healAmount)
        {
            ModifyHealth(healAmount);
        }

        public void TakeDamage(float damage)
        {
            ModifyHealth(-damage);
        }

        private bool ModifyHealth(float amount)
        {
            if (_isDead) return false;

            _currentHealth += amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
            {
                _isDead = true;
                OnDeath?.Invoke();
                return true;
            }

            return false;
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (TryGetComponent<Rigidbody2D>(out var rb))
                rb.AddForce(direction * force, ForceMode2D.Force);
        }
    }
}