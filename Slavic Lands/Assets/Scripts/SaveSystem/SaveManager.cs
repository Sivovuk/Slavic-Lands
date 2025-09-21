using System.IO;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// Static class responsible for saving, loading, and deleting player save data.
    /// Uses JSON serialization with Unity's JsonUtility and stores the file in persistent data path.
    /// </summary>
    public static class SaveManager
    {
        // Path to the save file (platform-specific safe directory)
        private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "player_save.json");

        /// <summary>
        /// Serializes PlayerProfileSaveData to JSON and writes it to disk.
        /// </summary>
        /// <param name="saveData">The player's profile data to save.</param>
        public static void SavePlayer(PlayerProfileSaveData saveData)
        {
            string json = JsonUtility.ToJson(saveData, true); // Pretty print JSON
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"Player saved to : {SaveFilePath}");
        }

        /// <summary>
        /// Attempts to load a save file and deserialize it into PlayerProfileSaveData.
        /// </summary>
        /// <param name="saveData">The output loaded save data (null if not found).</param>
        /// <returns>True if save file exists and loads successfully, false otherwise.</returns>
        public static bool TryLoadPlayer(out PlayerProfileSaveData saveData)
        {
            if (File.Exists(SaveFilePath))
            {
                string json = File.ReadAllText(SaveFilePath);
                saveData = JsonUtility.FromJson<PlayerProfileSaveData>(json);
                Debug.Log("Game loaded.");
                return true;
            }
            
            saveData = null;
            Debug.LogWarning("No save file found.");
            return false;
        }

        /// <summary>
        /// Deletes the player save file if it exists.
        /// </summary>
        public static void DeleteSave()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                Debug.Log("Save file deleted.");
            }
        }
    }
}
