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
    }

    [Serializable]
    public class AbilitySaveData
    {
        public ToolType ToolType;
        public int CurrentLevel;
    }
}