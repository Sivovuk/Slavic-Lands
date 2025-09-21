using System;
using Core.Interfaces;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Handles enemy AI attacking logic including detecting the player,
    /// deciding whether to chase, attack, or flee, and invoking relevant events.
    /// </summary>
    public class EntityAttack : MonoBehaviour
    {
        // Combat stats
        private float _attackDamage = 10f;
        private float _attackCooldown = 1f;
        private float _attackRange = 1f;
        private float _detectionRadius = 5f;

        [SerializeField] private bool _isAfraid;                 // Determines if the entity flees instead of chases
        [SerializeField] private LayerMask _detectionLayers;     // Which layers can be detected (e.g., Player)

        private float _attackTimer;      // Tracks time since last attack
        private Transform _target;       // Reference to the detected player
        private bool _wasChasing;        // Tracks state transition for chasing

        // --- Public Properties for External Access ---

        public Transform Target => _target;

        public bool IsAttacking { get; private set; }

        public bool IsChasing => !_isAfraid && _target != null;

        public bool IsFleeing => _isAfraid && _target != null;

        // --- Events ---

        public event Action<bool> OnStartAttacking;
        public event Action<bool> OnStopAttacking;
        public event Action<bool> OnTargetDetected;
        public event Action<bool> OnStartChasing;
        public event Action<bool> OnStopChasing;

        private void Update()
        {
            DetectTarget();

            // Detect change in chasing state
            bool isCurrentlyChasing = IsChasing;
            if (isCurrentlyChasing != _wasChasing)
            {
                if (isCurrentlyChasing)
                    OnStartChasing?.Invoke(true);
                else
                    OnStopChasing?.Invoke(false);

                _wasChasing = isCurrentlyChasing;
            }

            // No target to act on
            if (_target == null)
            {
                IsAttacking = false;
                return;
            }

            float distance = Vector2.Distance(transform.position, _target.position);

            // If close enough and not afraid, attempt attack
            if (!_isAfraid && distance <= _attackRange)
            {
                HandleAttack();
            }
            // If target moved out of range, stop attacking
            else if (IsAttacking)
            {
                IsAttacking = false;
                OnStopAttacking?.Invoke(false);
            }
        }

        /// <summary>
        /// Detects potential targets in the specified detection radius using layer masks.
        /// </summary>
        private void DetectTarget()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _detectionLayers);
            bool detected = false;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    if (_target != hit.transform)
                    {
                        _target = hit.transform;
                        OnTargetDetected?.Invoke(true);
                    }
                    detected = true;
                    break;
                }
            }

            if (!detected && _target != null)
            {
                _target = null;
                OnTargetDetected?.Invoke(false);
            }
        }

        /// <summary>
        /// Executes an attack if cooldown has passed, and applies damage to the target.
        /// </summary>
        private void HandleAttack()
        {
            _attackTimer += Time.deltaTime;

            if (_attackTimer >= _attackCooldown)
            {
                _attackTimer = 0f;

                if (_target.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(_attackDamage, ToolType.BattleAxe);
                    OnStartAttacking?.Invoke(true);
                }
            }
        }

        /// <summary>
        /// Allows setting attack-related stats externally (e.g., from a ScriptableObject or spawner).
        /// </summary>
        public void SetAttackStats(float damage, float cooldown, float detectionRadius, float attackRange)
        {
            _attackDamage = damage;
            _attackCooldown = cooldown;
            _detectionRadius = detectionRadius;
            _attackRange = attackRange;
        }

        /// <summary>
        /// Draws visual gizmos in the scene view to help visualize detection and attack range.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}
