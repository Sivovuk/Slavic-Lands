using System.Collections.Generic;
using Gameplay.Player;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Player Profile", menuName = "Player Profile")]
    public class PlayerProfileSO : ScriptableObject
    {
        public PlayerLevelData PlayerLevelData;
        [Header("Tool Skill Levels")]
        public List<SkillLevelEntry> SkillXpEntries;

        [Header("Ability Levels")]
        public List<PlayerAbilityLevelData> AbilityLevelDataList;

    }

}