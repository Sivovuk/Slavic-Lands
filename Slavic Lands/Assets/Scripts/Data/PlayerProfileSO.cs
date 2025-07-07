using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Player
{
    [CreateAssetMenu(fileName = "New Player Profile", menuName = "Player Profile")]
    public class PlayerProfileSO : ScriptableObject
    {
        public PlayerLevelData PlayerLevelData;
        [Header("Tool Skill Levels")]
        public List<SkillLevelEntry> SkillXPEntries;

        [Header("Ability Levels")]
        public List<PlayerAbilityLevelData> AbilityLevelDataList;

    }

}