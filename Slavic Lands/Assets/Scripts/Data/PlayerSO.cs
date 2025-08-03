using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Player", menuName = "Player")]
    public class PlayerSO : ScriptableObject
    {
        [Header("Movement")]
        public float WalkSpeed;
        public float RunSpeed;
        
        [Header("Jump")]
        public float JumpForce;
        
        [Header("Dash")]
        public float DashCost;
        public float DashingPower;
        public float DashingTime;
        public float DashCooldown;
        
        [Header("Health")]
        public float BaseHealth;

        [Header("Energy")] 
        public float BaseEnergy;

        [Header("Attack & Actions")] 
        public float AttackDamage;
        public float ShootDamage;
        public float CuttingDamage;
        public float MiningDamage;

        [Header("Abilities")] 
        public float Slash;
        public float SlashPushForce;
        [Space(10)]
        public float ShieldBash;
        public float ShieldBashPushForce;
        [Space(10)]
        public float PiercingArrow;
        public float PiercingArrowPushForce;
        
        [Header("Character")] 
        public int StartingLevel = 1;
        
        
        public float LevelMultiplayer = 1.2f;
    }
}