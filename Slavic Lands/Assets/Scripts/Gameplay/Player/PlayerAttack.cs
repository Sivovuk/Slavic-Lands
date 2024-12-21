using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerAttack : MonoBehaviour, ILoadingStats
    {
        private float _cutDamage;
        private float _mineDamage;
        private float _attackDamage;
        private float _shootDamage;
        
        public void LoadPlayerStats(PlayerSO playerSO, Player player)
        {
            
        }
    }
}