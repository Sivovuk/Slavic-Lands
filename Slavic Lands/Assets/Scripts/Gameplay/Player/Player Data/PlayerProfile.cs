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
        
        [Space(10)]
        
        public PlayerAbilityLevelData AbilitySlashData;
        public PlayerAbilityLevelData AbilityShieldBashData;
        public PlayerAbilityLevelData AbilityPiercingArrowData;

        public void SaveNewAbilityLevelData(PlayerAbilityLevelData abilityLevelData)
        {
            if (abilityLevelData.Name == AbilitySlashData.Name)
                AbilitySlashData = abilityLevelData;
            else if (abilityLevelData.Name == AbilityShieldBashData.Name)
                AbilityShieldBashData = abilityLevelData;
            else if (abilityLevelData.Name == AbilityPiercingArrowData.Name)
                AbilityPiercingArrowData = abilityLevelData;
        }
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

    [System.Serializable]
    public class PlayerAbilityLevelData
    {
        public string Name;
        public int CurrentLevel;
        public float LevelMultiplayer;
        
        public Action OnLevelChanged;

        public PlayerAbilityLevelData( PlayerAbilityLevelData data, float levelMultiplayer )
        {
            Name = data.Name;
            CurrentLevel = data.CurrentLevel;
            LevelMultiplayer = levelMultiplayer;
        }
        
        public PlayerAbilityLevelData(string name, int currentLevel, float levelMultiplayer)
        {
            Name = name;
            CurrentLevel = currentLevel;
            LevelMultiplayer = levelMultiplayer;
        }
    }
}