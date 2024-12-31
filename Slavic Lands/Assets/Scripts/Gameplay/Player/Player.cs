using System;
using Gameplay.Resources;
using Managers;
using UI.HUD;
using UnityEngine;

namespace Gameplay.Player
{
    public class Player : MonoBehaviour
    {
        public int CurrentLevel {get; private set; }
        public float LevelMultiplayer {get; private set;}

        [SerializeField] private PlayerResource _playerResource;

        public Action OnInit;

        // References
        [Header("References")]
        [SerializeField] private PlayerSO _player;
        public PlayerMovement PlayerMovement { get; private set; }
        public PlayerAttack PlayerAttack { get; private set; }
        public PlayerHealth PlayerHealth { get; private set; }
        public PlayerEnergy PlayerEnergy { get; private set; }
        
        public static Player Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
            
            PlayerMovement = GetComponent<PlayerMovement>();
            PlayerAttack = GetComponent<PlayerAttack>();
            PlayerHealth = GetComponent<PlayerHealth>();
            PlayerEnergy = GetComponent<PlayerEnergy>();
            
            _playerResource = new PlayerResource();
        }

        private void Start()
        {
            LoadPlayerStats();
            LoadResource();
        }

        public void AddResource(int amount, ResourceType resourceType)
        {
            _playerResource.AddResource(amount, resourceType);
        }

        private void LoadResource()
        {
            
        }

        private void LoadPlayerStats()
        {
            CurrentLevel = PlayerPrefs.GetInt(Constants.PlayerLevel, 1);
            LevelMultiplayer = _player.LevelMultiplayer;
            
            PlayerMovement.LoadPlayerStats(_player, this);
            PlayerAttack.LoadPlayerStats(_player, this);
            PlayerHealth.LoadPlayerStats(_player, this);
            PlayerEnergy.LoadPlayerStats(_player, this);

            GameManager.Instance.PlayerInit();
        }
    }
}