using System;
using System.Collections.Generic;
using Core.Constants;
using Core.Interfaces;
using Data;
using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Manages player health, damage handling, healing, and death logic.
    /// Loads data from PlayerSO and scales it based on the player's level.
    /// Implements IDamageable and ILoadingStatsPlayer for systemic interaction.
    /// </summary>
    public class PlayerHealth : MonoBehaviour, ILoadingStatsPlayer, IDamageable
    {
        private float _maxHealth;
        private float _currentHealth;
        private bool _isDead;

        // Public getter for death state
        public bool IsDead => _isDead;

        // Events
        public Action OnDeath;
        public Action<float, float> OnHealthChanged;

        private PlayerController _playerController;

        /// <summary>
        /// Initializes health stats from PlayerSO and loads from saved values if available.
        /// </summary>
        public void LoadPlayerStats(PlayerSO playerSO, PlayerController playerController)
        {
            _playerController = playerController;

            _maxHealth = playerSO.BaseHealth + (_playerController.PlayerProfile.PlayerLevelData.CurrentLevel * playerSO.LevelMultiplayer);
            _currentHealth = PlayerPrefs.GetFloat(Constants.SavedHealth, _maxHealth);

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        /// <summary>
        /// Adds health to the player (e.g., from healing items).
        /// </summary>
        public void Heal(float healAmount)
        {
            ModifyHealth(healAmount);
        }

        /// <summary>
        /// Reduces player health based on incoming damage.
        /// </summary>
        public void TakeDamage(float damage, ToolType toolType)
        {
            ModifyHealth(-damage);
        }

        /// <summary>
        /// Adjusts health, clamps to valid range, triggers death if depleted.
        /// </summary>
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

        /// <summary>
        /// Applies knockback force when hit by enemies or effects.
        /// </summary>
        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (TryGetComponent<Rigidbody2D>(out var rb))
                rb.AddForce(direction * force, ForceMode2D.Force);
        }
    }
}
