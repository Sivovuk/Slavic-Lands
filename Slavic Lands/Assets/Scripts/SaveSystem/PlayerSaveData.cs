using System;
using System.Collections.Generic;
using Core.Interfaces;

namespace SaveSystem
{
    /// <summary>
    /// Root save data structure for persisting the player's overall progression.
    /// Stores player level, XP progression, skill data, and ability data.
    /// </summary>
    [Serializable]
    public class PlayerProfileSaveData
    {
        // --- PLAYER LEVEL STATE ---

        public int PlayerLevel;           // Current overall player level
        public int PlayerXp;              // Current XP accumulated in this level
        public int PlayerXpToNext;        // Required XP for next level
        public int LevelPointsAvailable;  // Points available to allocate into skills/abilities
        public int MaxLevel;              // Maximum cap for player level

        // --- DETAILED PROGRESSION ---

        public List<SkillXpSaveData> Skills = new();     // List of all tool-based skill progressions
        public List<AbilitySaveData> Abilities = new();  // List of all learned/leveled abilities
    }

    /// <summary>
    /// Save data for a single skill progression tied to a specific tool type.
    /// </summary>
    [Serializable]
    public class SkillXpSaveData
    {
        public ToolType ToolType;   // The tool/skill this data belongs to (Axe, Pickaxe, etc.)
        public int CurrentXp;       // Current XP in this skill
        public int CurrentLevel;    // Current skill level
        public int XpToNextLevel;   // Required XP to reach next skill level
        public int MaxLevel;        // Maximum allowed skill level
    }

    /// <summary>
    /// Save data for a player ability (special combat or skill actions).
    /// </summary>
    [Serializable]
    public class AbilitySaveData
    {
        public ToolType ToolType;   // The ability type (e.g., Slash, Shield Bash, Piercing Arrow)
        public int CurrentLevel;    // Current upgrade level of this ability
    }
}