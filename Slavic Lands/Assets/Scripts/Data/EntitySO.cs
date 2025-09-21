using UnityEngine;

namespace Data
{
    /// <summary>
    /// ScriptableObject used to define core stats and behavior values for an entity (enemy, NPC, etc.).
    /// Enables easy configuration and reuse of entity data in the Unity editor.
    /// </summary>
    [CreateAssetMenu(fileName = "New Entity", menuName = "Entity Data")]
    public class EntitySO : ScriptableObject
    {
        // --- IDLE BEHAVIOR SETTINGS ---

        [Header("Idle Settings")]
        // Duration the entity remains idle before transitioning
        public float IdleDuration = 1f;

        // Delay before attacking when idle
        public float IdleAttackDuration = 0.5f;
        
        // --- MOVEMENT STATS ---

        [Header("Movement")]
        // Speed while walking (non-aggressive or patrolling)
        public float WalkSpeed;

        // Speed while running (chasing or fleeing)
        public float RunSpeed;

        // Randomized walking range for patrolling (min and max X offsets)
        public float MinWalkRange = -3f;
        public float MaxWalkRange = 3f;
        
        // --- COMBAT & ACTION PARAMETERS ---

        [Header("Attack & Actions")] 
        // Damage dealt on attack
        public float AttackDamage;

        // Cooldown between attacks
        public float AttackCooldown = 1f;

        // Distance at which the entity starts detecting targets
        public float DetectionRadius = 5f;

        // Max distance to be able to perform an attack
        public float AttackRange = 1f;
        
        // --- HEALTH STATS ---

        [Header("Health")]
        // Base health value for this entity
        public float BaseHealth;
        
        // --- GENERAL CHARACTER SETTINGS ---

        [Header("Character")] 
        // Starting level of the entity (for scaling, progression, etc.)
        public int StartingLevel = 1;

        // Multiplier applied per level (for stat scaling)
        public float LevelMultiplayer = 1.2f;
    }
}