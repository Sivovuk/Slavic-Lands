using System;
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
        private const string SaveFileName = "player_save.json";
        private const string TemporarySaveFileName = "player_save.tmp";
        private const string BackupSaveFileName = "player_save.bak";

        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        private static string TemporarySaveFilePath => Path.Combine(Application.persistentDataPath, TemporarySaveFileName);
        private static string BackupSaveFilePath => Path.Combine(Application.persistentDataPath, BackupSaveFileName);

        /// <summary>
        /// Serializes PlayerProfileSaveData to JSON and writes it to disk.
        /// </summary>
        /// <param name="saveData">The player's profile data to save.</param>
        public static bool SavePlayer(PlayerProfileSaveData saveData)
        {
            if (saveData == null)
            {
                Debug.LogError("Cannot save player data because the data is null.");
                return false;
            }

            try
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(TemporarySaveFilePath, json);

                if (File.Exists(SaveFilePath))
                {
                    File.Replace(TemporarySaveFilePath, SaveFilePath, BackupSaveFilePath, true);
                }
                else
                {
                    File.Move(TemporarySaveFilePath, SaveFilePath);
                }

                Debug.Log($"Player saved to: {SaveFilePath}");
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to save player data: {exception}");
                TryDeleteFile(TemporarySaveFilePath);
                return false;
            }
        }

        /// <summary>
        /// Attempts to load a save file and deserialize it into PlayerProfileSaveData.
        /// </summary>
        /// <param name="saveData">The output loaded save data (null if not found).</param>
        /// <returns>True if save file exists and loads successfully, false otherwise.</returns>
        public static bool TryLoadPlayer(out PlayerProfileSaveData saveData)
        {
            if (!TryReadSaveFile(SaveFilePath, out saveData) &&
                !TryReadSaveFile(BackupSaveFilePath, out saveData))
            {
                saveData = null;
                Debug.LogWarning("No valid save file found.");
                return false;
            }

            Debug.Log("Game loaded.");
            return true;
        }

        /// <summary>
        /// Deletes all save files and legacy PlayerPrefs data.
        /// </summary>
        public static void DeleteAllSavedData()
        {
            TryDeleteFile(SaveFilePath);
            TryDeleteFile(TemporarySaveFilePath);
            TryDeleteFile(BackupSaveFilePath);
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("All saved player data deleted.");
        }

        private static bool TryReadSaveFile(string path, out PlayerProfileSaveData saveData)
        {
            saveData = null;

            if (!File.Exists(path))
                return false;

            try
            {
                string json = File.ReadAllText(path);
                saveData = JsonUtility.FromJson<PlayerProfileSaveData>(json);
                return saveData != null && saveData.Skills != null && saveData.Abilities != null;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load save file '{path}': {exception}");
                return false;
            }
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to delete save file '{path}': {exception}");
            }
        }
    }
}
