using System.IO;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveManager
    {
        private static readonly string SaveFilePath = Path.Combine(Application.persistentDataPath, "player_save.json");

        public static void SavePlayer(PlayerProfileSaveData saveData)
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SaveFilePath, json);
            Debug.Log($"Player saved to : {SaveFilePath}");
        }

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