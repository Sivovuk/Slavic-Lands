using System;
using System.Collections.Generic;
using Core;
using Core.Constants;
using Core.Interfaces;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerHealth : MonoBehaviour, ILoadingStatsPlayer, IDamageable
    {
        private float _maxHealth;
        private float _currentHealth;
        
        [SerializeField] private bool _isDead;
        
        public Action OnDeath;
        public Action<float, float> OnHealthChanged;
        
        private PlayerController _playerController;

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

        public void LoadPlayerStats(PlayerSO playerSO, PlayerController playerController)
        {
            _playerController = playerController;
            _maxHealth = playerSO.BaseHealth + (_playerController.PlayerProfile.PlayerLevelData.CurrentLevel * playerSO.LevelMultiplayer);
            ModifyHealth(PlayerPrefs.GetFloat(Constants.SavedHealth, _maxHealth));
        }

        public void TakeDamage(int amount, Action callback = null)
        {
            throw new NotImplementedException();
        }

        public void TakeDamage(float amount, Action callback = null)
        {
            throw new NotImplementedException();
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            throw new NotImplementedException();
        }

        public bool IsDead { get; }
    }
}