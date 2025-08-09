using System;
using System.Collections.Generic;
using Core.Interfaces;
using Data;
using Gameplay.Resources;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(Collider2D))]
    public class EntityBase : MonoBehaviour, IDamageable, ILoadStatsEntity
    {
        [Header("Entity Config")]
        [field:SerializeField] public EntitySO EntityData { get; private set; }
        [field:SerializeField] public ResourceSO ResourceData { get; private set; }
        [field:SerializeField] public List<DungeonData>  DungeonData = new  List<DungeonData>();

        protected float _maxHealth;
        protected float _currentHealth;
        protected bool _isDead;

        public bool IsDead => _isDead;

        public event Action OnDeath;
        public event Action<float, float> OnHealthChanged;

        protected virtual void Awake()
        {
            if (EntityData != null)
                LoadEntityData(EntityData);
        }
        
        public void LoadEntityData(EntitySO entityData)
        {
            EntityData = entityData;

            float levelMultiplier = Mathf.Pow(entityData.LevelMultiplayer, entityData.StartingLevel - 1);
            _maxHealth = entityData.BaseHealth * levelMultiplier;
            _currentHealth = _maxHealth;
            _isDead = false;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            ApplyStatsToChildren();
        }

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

        public virtual void ApplyKnockback(Vector2 direction, float force)
        {
            if (TryGetComponent<Rigidbody2D>(out var rb))
                rb.AddForce(direction * force, ForceMode2D.Impulse);
        }

        protected virtual void Die()
        {
            _isDead = true;
            OnDeath?.Invoke();
            Destroy(gameObject);
        }

        public virtual void ApplyForce(float force, Vector2 direction) => ApplyKnockback(direction, force);
        public virtual List<ResourceData> GetResourceType() => null;
        
    }
}
