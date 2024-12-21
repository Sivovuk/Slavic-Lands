using System;
using Gameplay.Player;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class HUDController : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Image _healthBar;

        [Header("Energy")]
        [SerializeField] private Image _energyBar;

        private void Start()
        {
            GameManager.Instance.OnPlayerInit += OnInit;
        }

        public void OnInit()
        {
            Player.Instance.PlayerHealth.OnHealthChanged += UpdateHealthBar;
            Player.Instance.PlayerEnergy.OnEnergyChanged += UpdateEnergyBar;
        }

        private void OnDisable()
        {
            Player.Instance.PlayerHealth.OnHealthChanged -= UpdateHealthBar;
            Player.Instance.PlayerEnergy.OnEnergyChanged -= UpdateEnergyBar;
        }

        public void UpdateHealthBar(float current, float max)
        {
            _healthBar.fillAmount = current / max;
        }

        public void UpdateEnergyBar(float current, float max)
        {
            _energyBar.fillAmount = current / max;
        }
    }
}