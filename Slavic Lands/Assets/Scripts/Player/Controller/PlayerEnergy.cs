using System;
using Core.Constants;
using Core.Interfaces;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerEnergy : MonoBehaviour, ILoadingStatsPlayer
    {
        private float _maxEnergy;
        private float _currentEnergy;
        private float _timePassed;
        private bool _isSprinting;
        private bool _isPaused;
        
        private PlayerController _playerController;
        
        public float GetCurrentEnergy () => _currentEnergy;
        
        public Action<float, float> OnEnergyChanged;
        
        public void LoadPlayerStats(PlayerSO playerSO, PlayerController playerController)
        {
            _playerController = playerController;
            _maxEnergy = playerSO.BaseEnergy + (_playerController.PlayerProfile.PlayerLevelData.CurrentLevel * playerSO.LevelMultiplayer);
            ModifyEnergy(PlayerPrefs.GetFloat(Constants.SavedEnergy, _maxEnergy));
        }
        
        private void Update()
        {
            if (_isSprinting && _currentEnergy > 0 && !_isPaused)
            {
                DrainEnergy(0.2f);
            }
            else if (_currentEnergy < _maxEnergy)
            {
                RegenerateEnergy(0.1f);
            }
        }
        
        private void DrainEnergy(float amount)
        {
            _timePassed += Time.deltaTime;
            if (_timePassed >= 0.05f)
            {
                _timePassed = 0f;
                ModifyEnergy(-amount);
            }
        }

        private void RegenerateEnergy(float amount)
        {
            _isPaused = true;
            _timePassed += Time.deltaTime;
            if (_timePassed >= 0.05f)
            {
                _timePassed = 0f;
                ModifyEnergy(amount);
            }
        }

        public void UseEnergy(float amount) => ModifyEnergy(-amount);

        private void ModifyEnergy(float amount)
        {
            _currentEnergy += amount;
            _currentEnergy = Mathf.Clamp(_currentEnergy, 0f, _maxEnergy);
            OnEnergyChanged?.Invoke(_currentEnergy, _maxEnergy);
        }

        public bool Sprint(bool isSprinting)
        {
            _isSprinting = isSprinting;

            if (_currentEnergy >= _maxEnergy * 0.2f && _isPaused)
            {
                _isPaused = false;
                return true;
            }

            return _currentEnergy > 0;
        }

        public bool CanSprint() => _currentEnergy > 0f && !_isPaused;
    }
}