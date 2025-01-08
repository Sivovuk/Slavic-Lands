using UnityEngine;

namespace Gameplay.Player
{
    [CreateAssetMenu(fileName = "New Player Profile", menuName = "Player Profile")]
    public class PlayerProfileSO : ScriptableObject
    {
        public PlayerLevelData PlayerLevelData;
        public PlayerLevelData AttackLevelData;
        public PlayerLevelData ShootLevelData;
        public PlayerLevelData CutLevelData;
        public PlayerLevelData MineLevelData;
    }

}