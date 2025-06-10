using System;
using System.Threading.Tasks;
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
        [field:SerializeField] public PlayerSO PlayerSO {get; private set;}
        public XPDataSO XpDataSO;
        [SerializeField] private PlayerProfileSO PlayerProfileSO;
        [field:SerializeField] public PlayerProfile PlayerProfile {get; private set;}
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

        private async void Start()
        {
            await LoadPlayerProfile();
            await LoadPlayerStats();
            await LoadResource();
            
            GameManager.Instance.PlayerInit();
        }

        public void AddResource(int amount, ResourceType resourceType)
        {
            _playerResource.AddResource(amount, resourceType);
        }
        
        private Task LoadPlayerProfile()
        {
            CurrentLevel = PlayerPrefs.GetInt(Constants.PlayerLevel, 1);
            LevelMultiplayer = PlayerSO.LevelMultiplayer;
            
            PlayerProfile.PlayerLevelData = new PlayerLevelData(PlayerProfileSO.PlayerLevelData, LevelMultiplayer);
            PlayerProfile.AttackLevelData = new LevelData(PlayerProfileSO.AttackLevelData, LevelMultiplayer);
            PlayerProfile.ShootLevelData = new LevelData(PlayerProfileSO.ShootLevelData, LevelMultiplayer);
            PlayerProfile.CutLevelData = new LevelData(PlayerProfileSO.CutLevelData, LevelMultiplayer);
            PlayerProfile.MineLevelData = new LevelData(PlayerProfileSO.MineLevelData, LevelMultiplayer);
            
            PlayerProfile.AbilitySlashData = new PlayerAbilityLevelData(PlayerProfileSO.AbilitySlashData, LevelMultiplayer);
            PlayerProfile.AbilityShieldBashData = new PlayerAbilityLevelData(PlayerProfileSO.AbilityShieldBashData, LevelMultiplayer);
            PlayerProfile.AbilityPiercingArrowData = new PlayerAbilityLevelData(PlayerProfileSO.AbilityPiercingArrowData, LevelMultiplayer);
            return Task.CompletedTask;
        }

        private Task LoadPlayerStats()
        {
            PlayerMovement.LoadPlayerStats(PlayerSO, this);
            PlayerAttack.LoadPlayerStats(PlayerSO, this);
            PlayerHealth.LoadPlayerStats(PlayerSO, this);
            PlayerEnergy.LoadPlayerStats(PlayerSO, this);
            return Task.CompletedTask;
        }
        
        private Task LoadResource()
        {
            return Task.CompletedTask;
        }

    }
}