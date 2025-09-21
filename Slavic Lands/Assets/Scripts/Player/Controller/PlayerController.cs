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
    /// <summary>
    /// Central controller for the player object.
    /// Handles player stat loading, saving, initialization, and access to subsystems like combat and movement.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // --- PLAYER CONFIGURATION & REFERENCES ---

        [Header("References")]
        [field: SerializeField] public PlayerSO PlayerSO { get; private set; }                    // Base stat configuration
        [field: SerializeField] public XPDataSO XpDataSO { get; private set; }                    // XP values per tool/action
        [field: SerializeField] public PlayerProfileSO PlayerProfileSO { get; private set; }      // Default profile data
        [field: SerializeField] public PlayerProfile PlayerProfile { get; private set; }          // Runtime profile instance
        [field: SerializeField] public PlayerMovement PlayerMovement { get; private set; }
        [field: SerializeField] public PlayerCombat PlayerCombat { get; private set; }
        [field: SerializeField] public PlayerHealth PlayerHealth { get; private set; }
        [field: SerializeField] public PlayerEnergy PlayerEnergy { get; private set; }

        public PlayerResource PlayerResources { get; private set; }                               // Tracks collected resources

        // Singleton pattern
        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            // Get component references
            PlayerMovement = GetComponent<PlayerMovement>();
            PlayerCombat = GetComponent<PlayerCombat>();
            PlayerHealth = GetComponent<PlayerHealth>();
            PlayerEnergy = GetComponent<PlayerEnergy>();

            PlayerResources = new PlayerResource();
            PlayerProfile = new PlayerProfile();
        }

        private async void Start()
        {
            // Clear previous saves during development/test
            SaveManager.DeleteSave();

            // Load saved data or defaults
            if (!LoadFromSaveFile())
            {
                LoadFromDefaults();
            }

            // Initialize components with loaded data
            await LoadPlayerStats();
            await LoadResource();

            // Notify other systems that the player is initialized
            GameManager.Instance.PlayerInit();
        }

        /// <summary>
        /// Tries to load player data from a save file.
        /// </summary>
        private bool LoadFromSaveFile()
        {
            if (SaveManager.TryLoadPlayer(out var saveData))
            {
                PlayerProfile.LoadFromSaveData(saveData, PlayerSO.LevelMultiplayer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Loads default data from ScriptableObjects when no save is found.
        /// </summary>
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

        /// <summary>
        /// Distributes stat data to all components implementing ILoadingStatsPlayer.
        /// </summary>
        private Task LoadPlayerStats()
        {
            foreach (var component in GetComponents<ILoadingStatsPlayer>())
            {
                component.LoadPlayerStats(PlayerSO, this);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Placeholder for loading resource-related data if needed.
        /// </summary>
        private Task LoadResource()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds collected resources to the player's resource inventory.
        /// </summary>
        public void AddResource(int amount, ResourceType resourceType)
        {
            Debug.LogError("AddResource");
            PlayerResources.AddResource(amount, resourceType);
        }

        /// <summary>
        /// Serializes and saves the current player profile to persistent storage.
        /// </summary>
        public void SavePlayer()
        {
            var saveData = PlayerProfile.ToSaveData();
            SaveManager.SavePlayer(saveData);
        }

        /// <summary>
        /// Ensures player data is saved when the application quits.
        /// </summary>
        private void OnApplicationQuit()
        {
            SavePlayer();
        }
    }
}
