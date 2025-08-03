using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "New Entity", menuName = "Entity Data")]
    public class EntitySO : ScriptableObject
    {
        [Header("Idle Settings")]
        public float IdleDuration = 1f;
        public float IdleAttackDuration = 0.5f;
        
        [Header("Movement")]
        public float WalkSpeed;
        public float RunSpeed;
        public float MinWalkRange = -3f;
        public float MaxWalkRange = 3f;
        
        [Header("Attack & Actions")] 
        public float AttackDamage;
        public float AttackCooldown = 1f;
        public float DetectionRadius = 5f;
        public float AttackRange = 1f;
        
        [Header("Health")]
        public float BaseHealth;
        
        [Header("Character")] 
        public int StartingLevel = 1;
        public float LevelMultiplayer = 1.2f;
    }
}