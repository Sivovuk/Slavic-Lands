using System;
using UnityEngine;

namespace Gameplay.Player
{
    [System.Serializable]
    public class PlayerProfile
    {
        public PlayerLevelData PlayerLevelData;
        public LevelData AttackLevelData;
        public LevelData ShootLevelData;
        public LevelData CutLevelData;
        public LevelData MineLevelData;
        
        [Space(10)]
        
        public PlayerAbilityLevelData AbilitySlashData;
        public PlayerAbilityLevelData AbilityShieldBashData;
        public PlayerAbilityLevelData AbilityPiercingArrowData;

        public void SaveNewAbilityLevelData(PlayerAbilityLevelData abilityLevelData)
        {
            if (abilityLevelData.ActionType == AbilitySlashData.ActionType)
                AbilitySlashData = abilityLevelData;
            else if (abilityLevelData.ActionType == AbilityShieldBashData.ActionType)
                AbilityShieldBashData = abilityLevelData;
            else if (abilityLevelData.ActionType == AbilityPiercingArrowData.ActionType)
                AbilityPiercingArrowData = abilityLevelData;
        }
    }

    [System.Serializable]
    public class PlayerLevelData : LevelData
    {
        public int LevelPointsAvailable;
        
        public Action OnPointsChanged;
        
        public PlayerLevelData(PlayerLevelData levelData, float levelMultiplayer)
        {
            CurrentXp = levelData.CurrentXp;
            CurrentLevel = levelData.CurrentLevel;
            XpToNextLevel = levelData.XpToNextLevel;
            FirstLevelXp = levelData.FirstLevelXp;
            LevelMultiplayer = levelMultiplayer;
            LevelPointsAvailable = levelData.LevelPointsAvailable;
        }

        public override void AddXp(int xpToAdd)
        {
            int level = CurrentLevel;
            base.AddXp(xpToAdd);

            if (CurrentLevel > level)
            {
                LevelPointsAvailable++;
                OnPointsChanged?.Invoke();
            }
        }

        public bool CanUsePoints()
        {
            if (LevelPointsAvailable > 0)
            {
                return true;
            }
            
            return false;
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public int CurrentXp;
        public int CurrentLevel;
        public int XpToNextLevel;
        public int FirstLevelXp;
        public float LevelMultiplayer;
        public int MaxLevel;

        public Action<LevelData> OnXpChanged;
        public Action OnLevelChanged;

        public LevelData() { }

        public LevelData(LevelData levelData, float levelMultiplayer)
        {
            CurrentXp = levelData.CurrentXp;
            CurrentLevel = levelData.CurrentLevel;
            XpToNextLevel = levelData.XpToNextLevel;
            FirstLevelXp = levelData.FirstLevelXp;
            LevelMultiplayer = levelMultiplayer;
        }
        
        public LevelData(int firstLevelXp, float levelMultiplayer)
        {
            CurrentXp = 0;
            CurrentLevel = 0;
            XpToNextLevel = firstLevelXp;
            FirstLevelXp = firstLevelXp;
            LevelMultiplayer = levelMultiplayer;
        }

        public virtual void AddXp(int xpToAdd)
        {
            if (CurrentLevel == MaxLevel) return;
            
            CurrentXp += xpToAdd;

            if (CurrentXp >= XpToNextLevel)
            {
                CurrentLevel++;
                XpToNextLevel = Mathf.CeilToInt(XpToNextLevel * LevelMultiplayer);
                CurrentXp = 0;
                OnLevelChanged?.Invoke();
            }
            
            OnXpChanged?.Invoke(this);
        }

    }

    [System.Serializable]
    public class PlayerAbilityLevelData
    {
        public ActionType ActionType;
        public string Name;
        public int CurrentLevel;
        public float LevelMultiplayer;
        
        public Action OnLevelChanged;

        public PlayerAbilityLevelData( PlayerAbilityLevelData data, float levelMultiplayer )
        {
            ActionType = data.ActionType;
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