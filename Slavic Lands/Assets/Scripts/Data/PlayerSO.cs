using UnityEngine;

namespace Data
{
    /// <summary>
    /// ScriptableObject that stores base stats and ability values for the player.
    /// Provides a centralized configuration point for movement, combat, and progression attributes.
    /// </summary>
    [CreateAssetMenu(fileName = "New Player", menuName = "Player")]
    public class PlayerSO : ScriptableObject
    {
        // --- MOVEMENT STATS ---

        [Header("Movement")]
        // Walking speed for normal traversal
        public float WalkSpeed;

        // Running speed when sprinting
        public float RunSpeed;
        
        // --- JUMPING ---

        [Header("Jump")]
        // Upward force applied when jumping
        public float JumpForce;
        
        // --- DASHING ABILITIES ---

        [Header("Dash")]
        // Energy cost per dash
        public float DashCost;

        // Distance or strength of the dash movement
        public float DashingPower;

        // Duration of the dash
        public float DashingTime;

        // Time before another dash can be triggered
        public float DashCooldown;
        
        // --- HEALTH SYSTEM ---

        [Header("Health")]
        // Initial health value
        public float BaseHealth;

        // --- ENERGY SYSTEM ---

        [Header("Energy")] 
        // Initial energy pool for actions like dashing or abilities
        public float BaseEnergy;

        // --- COMBAT DAMAGE TYPES ---

        [Header("Attack & Actions")] 
        // Damage for melee attacks
        public float AttackDamage;

        // Damage from ranged shooting
        public float ShootDamage;

        // Damage when cutting (e.g., using an axe)
        public float CuttingDamage;

        // Damage when mining (e.g., using a pickaxe)
        public float MiningDamage;

        // --- ABILITY STATS ---

        [Header("Abilities")] 
        // Slash ability base damage
        public float Slash;

        // Force applied to enemies hit by Slash
        public float SlashPushForce;

        [Space(10)]
        // Shield Bash ability base damage
        public float ShieldBash;

        // Force applied by Shield Bash
        public float ShieldBashPushForce;

        [Space(10)]
        // Piercing Arrow ability base damage
        public float PiercingArrow;

        // Force applied by Piercing Arrow
        public float PiercingArrowPushForce;
        
        // --- CHARACTER PROGRESSION ---

        [Header("Character")] 
        // Starting level of the player
        public int StartingLevel = 1;

        // Multiplier for scaling stats with level progression
        public float LevelMultiplayer = 1.2f;
    }
}
