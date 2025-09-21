using System;
using System.Collections.Generic;
using Core.Interfaces;
using Data;
using Gameplay.Resources;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Base class for all entities (enemies, NPCs, etc.) that can be damaged and configured using data.
    /// Implements core logic for health, stats loading, knockback, and death handling.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class EntityBase : MonoBehaviour, IDamageable, ILoadStatsEntity
    {
        // --- ENTITY CONFIGURATION ---

        [Header("Entity Config")]
        [field: SerializeField] public EntitySO EntityData { get; private set; }          // Stats for the entity
        [field: SerializeField] public ResourceSO ResourceData { get; private set; }      // Resource drop and interaction data
        [field: SerializeField] public List<DungeonData> DungeonData = new List<DungeonData>(); // Optional additional loot data

        // --- HEALTH SYSTEM ---

        protected float _maxHealth;
        protected float _currentHealth;
        protected bool _isDead;

        public bool IsDead => _isDead;

        // --- EVENTS ---

        public event Action OnDeath;
        public event Action<float, float> OnHealthChanged; // current, max

        protected virtual void Awake()
        {
            if (EntityData != null)
                LoadEntityData(EntityData);
        }

        /// <summary>
        /// Loads base stats from the EntitySO and initializes health.
        /// </summary>
        public void LoadEntityData(EntitySO entityData)
        {
            EntityData = entityData;

            // Scale health based on level
            float levelMultiplier = Mathf.Pow(entityData.LevelMultiplayer, entityData.StartingLevel - 1);
            _maxHealth = entityData.BaseHealth * levelMultiplier;
            _currentHealth = _maxHealth;
            _isDead = false;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            ApplyStatsToChildren();
        }

        /// <summary>
        /// Applies stats from EntitySO to other components like movement or attack modules.
        /// </summary>
        protected virtual void ApplyStatsToChildren()
        {
            if (TryGetComponent(out EntityMovement movement))
                movement.SetMovementStats(EntityData);

            if (TryGetComponent(out EntityAttack attack))
                attack.SetAttackStats(
                    EntityData.AttackDamage,
                    EntityData.AttackCooldown,
                    EntityData.DetectionRadius,
                    EntityData.AttackRange
                );
        }

        /// <summary>
        /// Handles damage logic; only applies if the correct tool type is used.
        /// </summary>
        public virtual void TakeDamage(float damage, ToolType toolType)
        {
            if (toolType != ResourceData.ToolType) return;
            if (_isDead) return;

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(_currentHealth, 0);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0)
                Die();
        }

        /// <summary>
        /// Applies knockback force to the entity in a given direction.
        /// </summary>
        public virtual void ApplyKnockback(Vector2 direction, float force)
        {
            if (TryGetComponent<Rigidbody2D>(out var rb))
                rb.AddForce(direction * force, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Handles death of the entity and triggers destruction.
        /// </summary>
        protected virtual void Die()
        {
            _isDead = true;
            OnDeath?.Invoke();
            Destroy(gameObject);
        }

        // --- INTERFACE COMPATIBILITY ---

        public virtual void ApplyForce(float force, Vector2 direction) => ApplyKnockback(direction, force);

        public virtual List<ResourceData> GetResourceType() => null; // Override in child if entity drops items
    }
}
