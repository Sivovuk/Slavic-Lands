using System;
using System.Threading.Tasks;
using Core;
using Data;
using Managers;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerResource _playerResource;

        // References
        [Header("References")]
        [field: SerializeField] public PlayerSO PlayerSO { get; private set; }
        public XPDataSO XpDataSO;
        [SerializeField] private PlayerProfileSO PlayerProfileSO;
        [field: SerializeField] public PlayerProfile PlayerProfile { get; private set; }
        public PlayerMovement PlayerMovement { get; private set; }
        public PlayerCombat PlayerCombat { get; private set; }
        public PlayerHealth PlayerHealth { get; private set; }
        public PlayerEnergy PlayerEnergy { get; private set; }

        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            PlayerMovement = GetComponent<PlayerMovement>();
            PlayerCombat = GetComponent<PlayerCombat>();
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
            // Initialize base player level
            PlayerProfile.PlayerLevelData = new PlayerLevelData(PlayerProfileSO.PlayerLevelData, PlayerSO.LevelMultiplayer);

            // Initialize skill XP levels
            foreach (var entry in PlayerProfileSO.SkillXPEntries)
            {
                var newData = new SkillLevelEntry
                {
                    ToolType = entry.ToolType,
                    LevelData = new LevelData(entry.LevelData, PlayerSO.LevelMultiplayer)
                };
                PlayerProfile.SkillXPEntries.Add(newData);
            }

            // Initialize ability levels
            foreach (var ability in PlayerProfileSO.AbilityLevelDataList)
            {
                var newData = new PlayerAbilityLevelData(
                    ability.ToolType,
                    ability.CurrentLevel,
                    ability.LevelMultiplier
                );
                PlayerProfile.AbilityLevelDataList.Add(newData);
            }

            return Task.CompletedTask;
        }

        private Task LoadPlayerStats()
        {
            PlayerMovement.LoadPlayerStats(PlayerSO, this);
            PlayerCombat.LoadPlayerStats(PlayerSO, this);
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