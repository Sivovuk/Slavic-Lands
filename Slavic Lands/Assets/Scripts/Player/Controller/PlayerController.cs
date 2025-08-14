using System;
using System.Threading.Tasks;
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
        [Header("References")]
        [field: SerializeField] public PlayerSO PlayerSO { get; private set; }
        [field: SerializeField] public XPDataSO XpDataSO { get; private set; }
        [field: SerializeField] public PlayerProfileSO PlayerProfileSO { get; private set; }
        [field: SerializeField] public PlayerProfile PlayerProfile { get; private set; }
        [field: SerializeField] public PlayerMovement PlayerMovement { get; private set; }
        [field: SerializeField] public PlayerCombat PlayerCombat { get; private set; }
        [field: SerializeField] public PlayerHealth PlayerHealth { get; private set; }
        [field: SerializeField] public PlayerEnergy PlayerEnergy { get; private set; }
        public PlayerResource PlayerResources { get; private set; }

        
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
            
            PlayerResources = new PlayerResource();
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
            PlayerProfile.PlayerLevelData = new PlayerLevelData(PlayerProfileSO.PlayerLevelData, PlayerSO.LevelMultiplayer);

            PlayerProfile.SkillXpEntries.Clear();
            foreach (var entry in PlayerProfileSO.SkillXpEntries)
            {
                var newEntry = new SkillLevelEntry
                {
                    ToolType = entry.ToolType,
                    LevelData = new LevelData(entry.LevelData, PlayerSO.LevelMultiplayer)
                };
                PlayerProfile.SkillXpEntries.Add(newEntry);
            }

            PlayerProfile.AbilityLevelDataList.Clear();
            foreach (var ability in PlayerProfileSO.AbilityLevelDataList)
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
            Debug.LogError("AddResource");
            PlayerResources.AddResource(amount, resourceType);
            
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