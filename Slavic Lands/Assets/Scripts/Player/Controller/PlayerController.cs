using System.Threading.Tasks;
using Core.Interfaces;
using Data;
using Gameplay.Resources;
using Managers;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [field: SerializeField] public PlayerAnimationController PlayerAnimationController { get; private set; }
        [field: SerializeField] public PlayerAnimationEventHandler PlayerAnimationEventHandler { get; private set; }
        [field: SerializeField] public Gameplay.Inventory.PlayerInventory Inventory { get; private set; }
        [field: SerializeField] public Data.Items.ItemDatabaseSO ItemDatabase { get; private set; }

        public PlayerResource PlayerResources { get; private set; }                               // Tracks collected resources

        // Singleton pattern
        public static PlayerController Instance { get; private set; }
        private PlayerProfileSaveData _loadedSaveData;

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
            PlayerAnimationController = GetComponent<PlayerAnimationController>();
            PlayerAnimationEventHandler = GetComponent<PlayerAnimationEventHandler>();
            Inventory = GetComponent<Gameplay.Inventory.PlayerInventory>();
            if (Inventory == null)
            {
                Inventory = gameObject.AddComponent<Gameplay.Inventory.PlayerInventory>();
            }

            if (ItemDatabase == null)
            {
#if UNITY_EDITOR
                ItemDatabase = UnityEditor.AssetDatabase.LoadAssetAtPath<Data.Items.ItemDatabaseSO>("Assets/Data/Items/ItemDatabase.asset");
#endif
            }

            if (Inventory != null && ItemDatabase != null)
            {
                Inventory.SetItemDatabase(ItemDatabase);
            }

            PlayerResources = new PlayerResource();
            PlayerProfile = new PlayerProfile();
        }

        private async void Start()
        {
            // Load saved data or defaults
            if (!LoadFromSaveFile())
            {
                LoadFromDefaults();
            }

            // Initialize components with loaded data
            await LoadPlayerStats();
            await LoadResource();

            if (_loadedSaveData != null)
            {
                var saveData = _loadedSaveData;
                if (saveData.SchemaVersion >= 2)
                {
                    PlayerHealth.LoadCurrentHealth(saveData.CurrentHealth);
                    PlayerEnergy.LoadCurrentEnergy(saveData.CurrentEnergy);

                    if (saveData.SceneName == SceneManager.GetActiveScene().name)
                        transform.position = new Vector3(saveData.PositionX, saveData.PositionY, transform.position.z);
                }
            }

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
                NormalizeLegacySaveData(saveData);
                PlayerProfile.LoadFromSaveData(saveData, PlayerSO.LevelMultiplayer);
                _loadedSaveData = saveData;
                return true;
            }

            _loadedSaveData = null;
            return false;
        }

        private void NormalizeLegacySaveData(PlayerProfileSaveData saveData)
        {
            if (saveData.MaxLevel <= 0)
                saveData.MaxLevel = PlayerProfileSO.PlayerLevelData.MaxLevel;

            foreach (var skill in saveData.Skills)
            {
                if (skill.MaxLevel > 0)
                    continue;

                var defaultSkill = PlayerProfileSO.SkillXpEntries.Find(entry => entry.ToolType == skill.ToolType);
                if (defaultSkill != null)
                    skill.MaxLevel = defaultSkill.LevelData.MaxLevel;
            }
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

            _loadedSaveData = null;
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
            var saveData = _loadedSaveData;
            if (saveData != null && saveData.SchemaVersion >= 2)
            {
                PlayerResources.LoadFromSaveData(saveData.Resources);
                if (Inventory != null && saveData.InventoryItems != null)
                {
                    Inventory.LoadFromSaveData(saveData.InventoryItems);
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds collected resources to the player's resource inventory.
        /// </summary>
        public void AddResource(int amount, ResourceType resourceType)
        {
            PlayerResources.AddResource(amount, resourceType);
            SavePlayer();
        }

        /// <summary>
        /// Attempts to spend the specified amount of a resource. Returns true and saves if successful.
        /// </summary>
        public bool TrySpendResource(ResourceType resourceType, int amount)
        {
            bool success = PlayerResources.TrySpendResource(resourceType, amount);
            if (success)
            {
                SavePlayer();
            }
            return success;
        }

        /// <summary>
        /// Attempts to spend the specified amount of Gold.
        /// </summary>
        public bool TrySpendGold(int amount)
        {
            return TrySpendResource(ResourceType.Gold, amount);
        }

        /// <summary>
        /// Serializes and saves the current player profile to persistent storage.
        /// </summary>
        public void SavePlayer()
        {
            var saveData = PlayerProfile.ToSaveData();
            saveData.CurrentHealth = PlayerHealth.CurrentHealth;
            saveData.CurrentEnergy = PlayerEnergy.GetCurrentEnergy();
            saveData.SceneName = SceneManager.GetActiveScene().name;
            saveData.PositionX = transform.position.x;
            saveData.PositionY = transform.position.y;
            saveData.Resources = PlayerResources.ToSaveData();
            if (Inventory != null)
            {
                saveData.InventoryItems = Inventory.ToSaveData();
            }
            SaveManager.SavePlayer(saveData);
        }

        /// <summary>
        /// Ensures player data is saved when the application quits.
        /// </summary>
        private void OnApplicationQuit()
        {
            SavePlayer();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SavePlayer();
        }
    }
}
