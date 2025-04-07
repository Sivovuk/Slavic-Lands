using System;
using Data;
using Interfaces;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerEnergy : MonoBehaviour, ILoadingStatsPlayer
    {
        private float _maxEnergy;
        [SerializeField] private float _currentEnergy;
        public float GetCurrentEnergy () => _currentEnergy;

        private float _timePassed;
        
        [SerializeField] private bool _isSprinting;
        [SerializeField] private bool _isPaused;
        
        public Action<float, float> OnEnergyChanged;
        
        private Player _player;
        
        
        private void Update()
        {
            if (_isSprinting && _currentEnergy > 0 && !_isPaused)
            {
                _timePassed += Time.deltaTime;
                if (_timePassed >= 0.05f)
                {
                    _timePassed = 0;
                    ModifyEnergy(-0.2f);
                }
            }
            else if (_currentEnergy < _maxEnergy)
            {
                _isPaused = true;
                _timePassed += Time.deltaTime;
                if (_timePassed >= 0.05f)
                {
                    _timePassed = 0;
                    ModifyEnergy(0.1f);
                }
            }
        }

        public void BoostEnergy(float energy)
        {
            ModifyEnergy(energy);
        }

        public void UseEnergy(float energy)
        {
            ModifyEnergy(-energy);
        }

        private void ModifyEnergy(float energy)
        {
            _currentEnergy += energy;
            _currentEnergy = Mathf.Clamp(_currentEnergy, 0, _maxEnergy);
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
            else if (_currentEnergy > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanSprint()
        {
            return _currentEnergy > 0 && !_isPaused;
        }

        public void LoadPlayerStats(PlayerSO playerSO, Player player)
        {
            _player = player;
            _maxEnergy = playerSO.BaseEnergy + (_player.CurrentLevel * playerSO.LevelMultiplayer);
            ModifyEnergy(PlayerPrefs.GetFloat(Constants.SavedHealth, _maxEnergy));
        }
    }
}