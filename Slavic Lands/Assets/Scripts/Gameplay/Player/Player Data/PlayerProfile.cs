using System;
using UnityEngine;

namespace Gameplay.Player
{
    [System.Serializable]
    public class PlayerProfile
    {
        public PlayerLevelData PlayerLevelData;
        public PlayerLevelData AttackLevelData;
        public PlayerLevelData ShootLevelData;
        public PlayerLevelData CutLevelData;
        public PlayerLevelData MineLevelData;
    }

    [System.Serializable]
    public class PlayerLevelData
    {
        public int CurrentXp;
        public int CurrentLevel;
        public int XpToNextLevel;
        public int FirstLevelXp;
        public float LevelMultiplayer;
        
        public Action<PlayerLevelData> OnXpChanged;

        public PlayerLevelData(PlayerLevelData playerLevelData, float levelMultiplayer)
        {
            CurrentXp = playerLevelData.CurrentXp;
            CurrentLevel = playerLevelData.CurrentLevel;
            XpToNextLevel = playerLevelData.XpToNextLevel;
            FirstLevelXp = playerLevelData.FirstLevelXp;
            LevelMultiplayer = levelMultiplayer;
        }
        
        public PlayerLevelData(int firstLevelXp, float levelMultiplayer)
        {
            CurrentXp = 0;
            CurrentLevel = 0;
            XpToNextLevel = firstLevelXp;
            FirstLevelXp = firstLevelXp;
            LevelMultiplayer = levelMultiplayer;
        }

        public void AddXp(int xpToAdd)
        {
            CurrentXp += xpToAdd;

            if (CurrentXp >= XpToNextLevel)
            {
                CurrentLevel++;
                XpToNextLevel = Mathf.CeilToInt(XpToNextLevel * LevelMultiplayer);
                CurrentXp = 0;
            }
            
            OnXpChanged?.Invoke(this);
        }
    }
}