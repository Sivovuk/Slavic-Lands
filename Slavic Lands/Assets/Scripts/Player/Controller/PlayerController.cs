using System;
using System.Threading.Tasks;
using Core;
using Core.Interfaces;
using Data;
using Gameplay.Resources;
using Managers;
using SaveSystem;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerResource _playerResource;

        // References
        [Header("References")]
        [field: SerializeField]
        public PlayerSO PlayerSO { get; private set; }

        public XPDataSO XpDataSO;
        [SerializeField] private PlayerProfileSO _playerProfileSO;
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
            SaveManager.DeleteSave();
            
            if (!LoadFromSaveFile())
            {
                LoadFromDefaults();
            }

            await LoadPlayerStats();
            await LoadResource();

            GameManager.Instance.PlayerInit();
        }

        private bool LoadFromSaveFile()
        {
            if (SaveManager.TryLoadPlayer(out var saveData))
            {
                PlayerProfile.LoadFromSaveData(saveData, PlayerSO.LevelMultiplayer);
                return true;
            }

            return false;
        }

        private void LoadFromDefaults()
        {
            PlayerProfile.PlayerLevelData = new PlayerLevelData(_playerProfileSO.PlayerLevelData, PlayerSO.LevelMultiplayer);

            PlayerProfile.SkillXpEntries.Clear();
            foreach (var entry in _playerProfileSO.SkillXpEntries)
            {
                var newEntry = new SkillLevelEntry
                {
                    ToolType = entry.ToolType,
                    LevelData = new LevelData(entry.LevelData, PlayerSO.LevelMultiplayer)
                };
                PlayerProfile.SkillXpEntries.Add(newEntry);
            }

            PlayerProfile.AbilityLevelDataList.Clear();
            foreach (var ability in _playerProfileSO.AbilityLevelDataList)
            {
                PlayerProfile.AbilityLevelDataList.Add(new PlayerAbilityLevelData(
                    ability.ToolType,
                    ability.CurrentLevel,
                    ability.LevelMultiplier
                ));
            }
        }

        private Task LoadPlayerStats()
        {
            foreach (var component in GetComponents<ILoadingStatsPlayer>())
            {
                component.LoadPlayerStats(PlayerSO, this);
            }

            return Task.CompletedTask;
        }

        private Task LoadResource()
        {
            // Optionally preload or restore resources
            return Task.CompletedTask;
        }

        public void AddResource(int amount, ResourceType resourceType)
        {
            _playerResource.AddResource(amount, resourceType);
        }

        public void SavePlayer()
        {
            var saveData = PlayerProfile.ToSaveData();
            SaveManager.SavePlayer(saveData);
        }

        private void OnApplicationQuit()
        {
            SavePlayer();
        }
    }
}