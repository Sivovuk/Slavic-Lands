using UnityEngine;

namespace Gameplay.Player
{
    [CreateAssetMenu(fileName = "New Player Profile", menuName = "Player Profile")]
    public class PlayerProfileSO : ScriptableObject
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
    }

}