using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

namespace Gameplay.Player
{
    [System.Serializable]
    public class PlayerProfile
    {
        public PlayerLevelData PlayerLevelData;

        [field: SerializeField] public List<SkillLevelEntry> SkillXPEntries { get; private set; } = new();
        private Dictionary<ToolType, LevelDataBase> _skillMap;

        [field: SerializeField] public List<PlayerAbilityLevelData> AbilityLevelDataList { get; private set; } = new();
        public Dictionary<ToolType, PlayerAbilityLevelData> AbilityMap;

        private bool _initialized;
        
        private void EnsureInitialized()
        {
            if (_initialized) return;
            _skillMap = SkillXPEntries.ToDictionary(entry => entry.ToolType, entry => entry.LevelData);
            AbilityMap = AbilityLevelDataList.ToDictionary(a => a.ToolType, a => a);
            _initialized = true;
        }

        public void SaveNewAbilityLevelData(PlayerAbilityLevelData newData)
        {
            EnsureInitialized();

            if (AbilityMap.ContainsKey(newData.ToolType))
            {
                AbilityMap[newData.ToolType] = newData;

                // Sync back to list if needed
                var index = AbilityLevelDataList.FindIndex(x => x.ToolType == newData.ToolType);
                if (index != -1)
                    AbilityLevelDataList[index] = newData;
            }
            else
            {
                AbilityLevelDataList.Add(newData);
                AbilityMap.Add(newData.ToolType, newData);
            }
        }

        public PlayerAbilityLevelData GetAbilityData(ToolType toolType)
        {
            EnsureInitialized();
            return AbilityMap.TryGetValue(toolType, out var data) ? data : null;
        }
    }
    
    [System.Serializable]
    public abstract class LevelDataBase : ILevelData
    {
        [field:SerializeField] public int CurrentXp { get; private set; }
        [field:SerializeField] public int CurrentLevel { get; private set; }
        [field:SerializeField] public int XPToNextLevel { get; private set; }
        [field:SerializeField] public int MaxLevel { get; private set; }
        [field:SerializeField] public int FirstLevelXp { get; private set; }
        [field:SerializeField] public float LevelMultiplier { get; private set; }

        public event Action<LevelDataBase> OnXpChanged;
        public event Action OnLevelChanged;

        protected LevelDataBase() { }

        protected LevelDataBase(LevelDataBase copyFrom, float newMultiplier)
        {
            CurrentXp = copyFrom.CurrentXp;
            CurrentLevel = copyFrom.CurrentLevel;
            XPToNextLevel = copyFrom.XPToNextLevel;
            FirstLevelXp = copyFrom.FirstLevelXp;
            LevelMultiplier = newMultiplier;
            MaxLevel = copyFrom.MaxLevel;
        }

        protected LevelDataBase(int baseXp, float multiplier, int maxLevel)
        {
            FirstLevelXp = baseXp;
            XPToNextLevel = baseXp;
            LevelMultiplier = multiplier;
            MaxLevel = maxLevel;
        }

        public virtual void AddXp(int xp)
        {
            if (CurrentLevel >= MaxLevel) return;

            CurrentXp += xp;

            if (CurrentXp >= XPToNextLevel && CurrentLevel < MaxLevel)
            {
                CurrentXp -= XPToNextLevel;
                CurrentLevel++;
                XPToNextLevel = Mathf.CeilToInt(XPToNextLevel * LevelMultiplier);
                OnLevelChanged?.Invoke();
            }

            OnXpChanged?.Invoke(this);
        }
    }
    
    [Serializable]
    public class LevelData : LevelDataBase
    {
        public LevelData(LevelDataBase copyFrom, float multiplier)
            : base(copyFrom, multiplier) { }

        public LevelData(int baseXp, float multiplier, int maxLvl = 99)
            : base(baseXp, multiplier, maxLvl) { }
    }
    
    [System.Serializable]
    public class PlayerLevelData : LevelDataBase
    {
        [field:SerializeField] public int LevelPointsAvailable{ get; private set; }
        
        public event Action OnPointsChanged;

        public PlayerLevelData(PlayerLevelData copyFrom, float multiplier)
            : base(copyFrom, multiplier)
        {
            LevelPointsAvailable = copyFrom.LevelPointsAvailable;
        }

        public override void AddXp(int xp)
        {
            int oldLevel = CurrentLevel;
            base.AddXp(xp);
            if (CurrentLevel > oldLevel)
            {
                LevelPointsAvailable++;
                OnPointsChanged?.Invoke();
            }
        }

        public bool CanUsePoints() => LevelPointsAvailable > 0;
    }
    

    [System.Serializable]
    public class PlayerAbilityLevelData
    {
        public ToolType ToolType { get; private set; }
        public int CurrentLevel { get; private set; }
        public float LevelMultiplier { get; private set; }

        public event Action OnLevelChanged;

        public PlayerAbilityLevelData(ToolType toolType, int level, float multiplier)
        {
            ToolType = toolType;
            CurrentLevel = level;
            LevelMultiplier = multiplier;
        }

        public void LevelUp()
        {
            CurrentLevel++;
            OnLevelChanged?.Invoke();
        }
    }
    
    [System.Serializable]
    public class SkillLevelEntry
    {
        public ToolType ToolType;
        public LevelDataBase LevelData;
    }
}