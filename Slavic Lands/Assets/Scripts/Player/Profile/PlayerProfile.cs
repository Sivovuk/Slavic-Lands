using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using SaveSystem;
using UnityEngine;

namespace Gameplay.Player
{
    [System.Serializable]
    public class PlayerProfile
    {
        public PlayerLevelData PlayerLevelData { get; set; }

        [field: SerializeField] public List<SkillLevelEntry> SkillXpEntries { get; private set; } = new();
        private Dictionary<ToolType, LevelDataBase> _skillMap;

        [field: SerializeField] public List<PlayerAbilityLevelData> AbilityLevelDataList { get; private set; } = new();
        public Dictionary<ToolType, PlayerAbilityLevelData> AbilityMap { get; private set; }

        private bool _initialized;

        private void EnsureInitialized()
        {
            if (_initialized) return;
            _skillMap = SkillXpEntries.ToDictionary(entry => entry.ToolType, entry => entry.LevelData);
            AbilityMap = AbilityLevelDataList.ToDictionary(a => a.ToolType, a => a);
            _initialized = true;
        }

        public void SaveNewAbilityLevelData(PlayerAbilityLevelData newData)
        {
            EnsureInitialized();

            if (AbilityMap.ContainsKey(newData.ToolType))
            {
                AbilityMap[newData.ToolType] = newData;
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
        
        public LevelDataBase GetLevelData(ToolType toolType)
        {
            EnsureInitialized();
            return _skillMap.TryGetValue(toolType, out var data) ? data : null;
        }

        public PlayerProfileSaveData ToSaveData()
        {
            var data = new PlayerProfileSaveData
            {
                PlayerLevel = PlayerLevelData.CurrentLevel,
                PlayerXp = PlayerLevelData.CurrentXp,
                PlayerXpToNext = PlayerLevelData.XpToNextLevel,
                LevelPointsAvailable = PlayerLevelData.LevelPointsAvailable
            };

            foreach (var entry in SkillXpEntries)
            {
                if (entry.LevelData is LevelDataBase level)
                {
                    data.Skills.Add(new SkillXpSaveData
                    {
                        ToolType = entry.ToolType,
                        CurrentXp = level.CurrentXp,
                        CurrentLevel = level.CurrentLevel,
                        XpToNextLevel = level.XpToNextLevel
                    });
                }
            }

            foreach (var ability in AbilityLevelDataList)
            {
                data.Abilities.Add(new AbilitySaveData
                {
                    ToolType = ability.ToolType,
                    CurrentLevel = ability.CurrentLevel
                });
            }

            return data;
        }

        public void LoadFromSaveData(PlayerProfileSaveData saveData, float levelMultiplier)
        {
            PlayerLevelData = new PlayerLevelData(saveData.PlayerLevel, saveData.PlayerXp, saveData.PlayerXpToNext, saveData.MaxLevel, saveData.LevelPointsAvailable, levelMultiplier);

            SkillXpEntries.Clear();
            foreach (var skill in saveData.Skills)
            {
                var levelData = new LevelData(skill.CurrentLevel, skill.CurrentXp, skill.XpToNextLevel, skill.MaxLevel,  levelMultiplier);
                SkillXpEntries.Add(new SkillLevelEntry { ToolType = skill.ToolType, LevelData = levelData });
            }

            AbilityLevelDataList.Clear();
            foreach (var ability in saveData.Abilities)
            {
                AbilityLevelDataList.Add(new PlayerAbilityLevelData(ability.ToolType, ability.CurrentLevel, levelMultiplier));
            }
        }
        
        public bool TryAddXp(ToolType tool, int amount)
        {
            if (_skillMap.TryGetValue(tool, out var level))
            {
                Debug.LogError(amount);
                level.AddXp(amount);
                return true;
            }
            return false;
        }
    }

    [Serializable]
    public class LevelDataBase : ILevelData
    {
        [SerializeField] private int _currentXp;
        [SerializeField] private int _currentLevel;
        [SerializeField] private int _xpToNextLevel;
        [SerializeField] private int _maxLevel;
        [SerializeField] private int _firstLevelXp;
        [SerializeField] private float _levelMultiplier;

        public event Action<LevelDataBase> OnXpChanged;
        public event Action OnLevelChanged;

        public int CurrentXp => _currentXp;
        public int CurrentLevel => _currentLevel;
        public int XpToNextLevel => _xpToNextLevel;
        public float LevelMultiplier => _levelMultiplier;

        protected LevelDataBase() { }

        protected LevelDataBase(LevelDataBase copyFrom, float newMultiplier)
        {
            _currentXp = copyFrom._currentXp;
            _currentLevel = copyFrom._currentLevel;
            _xpToNextLevel = copyFrom._firstLevelXp;
            _firstLevelXp = copyFrom._firstLevelXp;
            _levelMultiplier = newMultiplier;
            _maxLevel = copyFrom._maxLevel;
        }

        protected LevelDataBase(int baseXp, float multiplier, int maxLvl)
        {
            _firstLevelXp = baseXp;
            _xpToNextLevel = baseXp;
            _levelMultiplier = multiplier;
            _maxLevel = maxLvl;
        }

        protected LevelDataBase(int level, int xp, int xpToNext, int maxLvl, int firstLevelXp, float multiplier)
        {
            _currentLevel = level;
            _currentXp = xp;
            _xpToNextLevel = xpToNext;
            _levelMultiplier = multiplier;
            _maxLevel = maxLvl;
            _firstLevelXp = firstLevelXp; // default or configurable value
        }

        public virtual void AddXp(int xp)
        {
            if (_currentLevel >= _maxLevel) return;

            _currentXp += xp;

            while (_currentXp >= _xpToNextLevel && _currentLevel < _maxLevel)
            {
                _currentXp -= _xpToNextLevel;
                _currentLevel++;
                _xpToNextLevel = Mathf.CeilToInt(_xpToNextLevel * _levelMultiplier);
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

        public LevelData(int baseXp, float multiplier, int maxLvl)
            : base(baseXp, multiplier, maxLvl) { }

        public LevelData(int level, int xp, int xpToNext, int maxLevel, float multiplier)
            : base(level, xp, xpToNext, maxLevel, maxLevel, multiplier) { }
    }

    [Serializable]
    public class PlayerLevelData : LevelDataBase
    {
        [SerializeField] private int _levelPointsAvailable;

        public int LevelPointsAvailable => _levelPointsAvailable;

        public event Action OnPointsChanged;

        public PlayerLevelData(PlayerLevelData copyFrom, float multiplier)
            : base(copyFrom, multiplier)
        {
            _levelPointsAvailable = copyFrom._levelPointsAvailable;
        }

        public PlayerLevelData(int level, int xp, int xpToNext, int maxLevel, int points, float multiplier)
            : base(level, xp, xpToNext, maxLevel, points, multiplier)
        {
            _levelPointsAvailable = points;
        }

        public override void AddXp(int xp)
        {
            int oldLevel = CurrentLevel;
            base.AddXp(xp);
            if (CurrentLevel > oldLevel)
            {
                _levelPointsAvailable++;
                OnPointsChanged?.Invoke();
            }
        }

        public bool CanUsePoints() => _levelPointsAvailable > 0;
    }

    [Serializable]
    public class PlayerAbilityLevelData
    {
        public ToolType ToolType;
        public int CurrentLevel;
        public float LevelMultiplier;

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

    [Serializable]
    public class SkillLevelEntry
    {
        public ToolType ToolType;
        public LevelDataBase LevelData;
    }
}
