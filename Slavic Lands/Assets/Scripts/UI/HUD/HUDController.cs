using Gameplay.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class HUDController : MonoBehaviour
    {
        [field: SerializeField] public ResourceDisplay ResourceDisplay { get; private set; }
        
        [Header("Health")]
        [SerializeField] private Image _healthBar;

        [Header("Energy")]
        [SerializeField] private Image _energyBar;

        public static HUDController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        public void OnInit(PlayerController player)
        {
            player.PlayerHealth.OnHealthChanged += UpdateHealthBar;
            player.PlayerEnergy.OnEnergyChanged += UpdateEnergyBar;

            // Initialize resource UI
            ResourceDisplay.Initialize(player.PlayerResources);
        }

        private void OnDisable()
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.PlayerHealth.OnHealthChanged -= UpdateHealthBar;
                PlayerController.Instance.PlayerEnergy.OnEnergyChanged -= UpdateEnergyBar;
            }
        }

        public void UpdateHealthBar(float current, float max) =>
            _healthBar.fillAmount = current / max;

        public void UpdateEnergyBar(float current, float max) =>
            _energyBar.fillAmount = current / max;
    }
}