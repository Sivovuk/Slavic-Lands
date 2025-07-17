using System;
using System.Collections.Generic;
using Core.Interfaces;

namespace SaveSystem
{
    [Serializable]
    public class PlayerProfileSaveData
    {
        public int PlayerLevel;
        public int PlayerXp;
        public int PlayerXpToNext;
        public int LevelPointsAvailable;
        public int MaxLevel;

        public List<SkillXpSaveData> Skills = new();
        public List<AbilitySaveData> Abilities = new();
    }

    [Serializable]
    public class SkillXpSaveData
    {
        public ToolType ToolType;
        public int CurrentXp;
        public int CurrentLevel;
        public int XpToNextLevel;
        public int MaxLevel;
    }

    [Serializable]
    public class AbilitySaveData
    {
        public ToolType ToolType;
        public int CurrentLevel;
    }
}