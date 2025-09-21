using System;
using Core.Constants;
using Core.Interfaces;
using Data;
using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Manages the player's energy system, including draining while sprinting and passive regeneration.
    /// Loads base values from PlayerSO and applies runtime level scaling.
    /// </summary>
    public class PlayerEnergy : MonoBehaviour, ILoadingStatsPlayer
    {
        private float _maxEnergy;
        private float _currentEnergy;
        private float _timePassed;

        private bool _isSprinting;
        private bool _isPaused;

        private PlayerController _playerController;

        // Getter to expose current energy value
        public float GetCurrentEnergy() => _currentEnergy;

        // Event to notify UI or systems when energy changes
        public Action<float, float> OnEnergyChanged;

        /// <summary>
        /// Loads player energy data using PlayerSO and scales it with level.
        /// Attempts to restore saved energy from PlayerPrefs.
        /// </summary>
        public void LoadPlayerStats(PlayerSO playerSO, PlayerController playerController)
        {
            _playerController = playerController;
            _maxEnergy = playerSO.BaseEnergy + (_playerController.PlayerProfile.PlayerLevelData.CurrentLevel * playerSO.LevelMultiplayer);

            // Restore previously saved energy or default to max
            ModifyEnergy(PlayerPrefs.GetFloat(Constants.SavedEnergy, _maxEnergy));
        }

        private void Update()
        {
            // Drain energy while sprinting and not paused
            if (_isSprinting && _currentEnergy > 0 && !_isPaused)
            {
                DrainEnergy(0.2f);
            }
            // Regenerate energy when not sprinting
            else if (_currentEnergy < _maxEnergy)
            {
                RegenerateEnergy(0.1f);
            }
        }

        /// <summary>
        /// Gradually reduces energy based on time intervals.
        /// </summary>
        private void DrainEnergy(float amount)
        {
            _timePassed += Time.deltaTime;

            if (_timePassed >= 0.05f)
            {
                _timePassed = 0f;
                ModifyEnergy(-amount);
            }
        }

        /// <summary>
        /// Gradually increases energy if not full.
        /// </summary>
        private void RegenerateEnergy(float amount)
        {
            _isPaused = true; // Pause sprinting until enough energy
            _timePassed += Time.deltaTime;

            if (_timePassed >= 0.05f)
            {
                _timePassed = 0f;
                ModifyEnergy(amount);
            }
        }

        /// <summary>
        /// Immediately reduces energy (e.g., for abilities or dashing).
        /// </summary>
        public void UseEnergy(float amount) => ModifyEnergy(-amount);

        /// <summary>
        /// Adds or subtracts energy, clamped between 0 and max.
        /// </summary>
        private void ModifyEnergy(float amount)
        {
            _currentEnergy += amount;
            _currentEnergy = Mathf.Clamp(_currentEnergy, 0f, _maxEnergy);
            OnEnergyChanged?.Invoke(_currentEnergy, _maxEnergy);
        }

        /// <summary>
        /// Initiates or stops sprinting based on energy and cooldown pause.
        /// </summary>
        public bool Sprint(bool isSprinting)
        {
            _isSprinting = isSprinting;

            // Allow sprinting again once energy has regenerated enough
            if (_currentEnergy >= _maxEnergy * 0.2f && _isPaused)
            {
                _isPaused = false;
                return true;
            }

            return _currentEnergy > 0;
        }

        /// <summary>
        /// Returns whether the player is allowed to sprint.
        /// </summary>
        public bool CanSprint() => _currentEnergy > 0f && !_isPaused;
    }
}
