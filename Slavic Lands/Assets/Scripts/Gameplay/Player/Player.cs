using System;
using Data;
using Gameplay.Resources;
using Managers;
using UnityEngine;

namespace Gameplay.Player
{
    public class Player : MonoBehaviour
    {
        public int CurrentLevel {get; private set; }
        public float LevelMultiplayer {get; private set;}

        [SerializeField] private PlayerResource _playerResource;

        // References
        [Header("References")]
        [SerializeField] private PlayerSO _playerSO;
        public XPDataSO XpDataSO;
        public PlayerProfileSO PlayerProfileSO;
        public PlayerProfile PlayerProfile {get; private set;}
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
            PlayerProfile = new PlayerProfile();
        }

        private void Start()
        {
            LoadPlayerStats();
            LoadResource();
            LoadPlayerProfile();
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
            LevelMultiplayer = _playerSO.LevelMultiplayer;
            
            PlayerMovement.LoadPlayerStats(_playerSO, this);
            PlayerAttack.LoadPlayerStats(_playerSO, this);
            PlayerHealth.LoadPlayerStats(_playerSO, this);
            PlayerEnergy.LoadPlayerStats(_playerSO, this);
        }

        private void LoadPlayerProfile()
        {
            PlayerProfile.PlayerLevelData = new PlayerLevelData(PlayerProfileSO.PlayerLevelData, LevelMultiplayer);
            PlayerProfile.AttackLevelData = new PlayerLevelData(PlayerProfileSO.AttackLevelData, LevelMultiplayer);
            PlayerProfile.ShootLevelData = new PlayerLevelData(PlayerProfileSO.ShootLevelData, LevelMultiplayer);
            PlayerProfile.CutLevelData = new PlayerLevelData(PlayerProfileSO.CutLevelData, LevelMultiplayer);
            PlayerProfile.MineLevelData = new PlayerLevelData(PlayerProfileSO.MineLevelData, LevelMultiplayer);
            GameManager.Instance.PlayerInit();
        }
    }
}