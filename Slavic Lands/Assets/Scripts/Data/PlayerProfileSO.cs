using System.Collections.Generic;
using Gameplay.Player;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// ScriptableObject representing a player’s profile data, including level, tool skills, and ability levels.
    /// Useful for saving/loading player progression or initializing player stats in a modular way.
    /// </summary>
    [CreateAssetMenu(fileName = "New Player Profile", menuName = "Player Profile")]
    public class PlayerProfileSO : ScriptableObject
    {
        // Stores the player's overall level data (e.g., XP, level, progression)
        public PlayerLevelData PlayerLevelData;

        [Header("Tool Skill Levels")]
        // List of XP/level entries for different tools (e.g., axe, bow, pickaxe)
        public List<SkillLevelEntry> SkillXpEntries;

        [Header("Ability Levels")]
        // List of player abilities and their respective levels
        public List<PlayerAbilityLevelData> AbilityLevelDataList;
    }
}