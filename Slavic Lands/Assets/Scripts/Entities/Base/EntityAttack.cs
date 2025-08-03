using System;
using Core.Interfaces;
using UnityEngine;

namespace Entities
{
    public class EntityAttack : MonoBehaviour
    {
        private float _attackDamage = 10f;
        private float _attackCooldown = 1f;
        private float _attackRange = 1f;
        private float _detectionRadius = 5f;

        [SerializeField] private bool _isAfraid;
        [SerializeField] private LayerMask _detectionLayers;

        private float _attackTimer;
        private Transform _target;
        private bool _wasChasing;

        public Transform Target => _target;
        public bool IsAttacking { get; private set; }
        public bool IsChasing => !_isAfraid && _target != null;
        public bool IsFleeing => _isAfraid && _target != null;

        public event Action<bool> OnStartAttacking;
        public event Action<bool> OnStopAttacking;
        public event Action<bool> OnTargetDetected;
        public event Action<bool> OnStartChasing;
        public event Action<bool> OnStopChasing;

        private void Update()
        {
            DetectTarget();

            bool isCurrentlyChasing = IsChasing;
            if (isCurrentlyChasing != _wasChasing)
            {
                if (isCurrentlyChasing)
                    OnStartChasing?.Invoke(true);
                else
                    OnStopChasing?.Invoke(false);

                _wasChasing = isCurrentlyChasing;
            }

            if (_target == null)
            {
                IsAttacking = false;
                return;
            }

            float distance = Vector2.Distance(transform.position, _target.position);

            if (!_isAfraid && distance <= _attackRange)
            {
                HandleAttack();
            }
            else if (IsAttacking)
            {
                IsAttacking = false;
                OnStopAttacking?.Invoke(false);
            }
        }

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

        private void HandleAttack()
        {
            _attackTimer += Time.deltaTime;

            if (_attackTimer >= _attackCooldown)
            {
                _attackTimer = 0f;
                if (_target.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(_attackDamage);
                    OnStartAttacking?.Invoke(true);
                }
            }
        }

        public void SetAttackStats(float damage, float cooldown, float detectionRadius, float attackRange)
        {
            _attackDamage = damage;
            _attackCooldown = cooldown;
            _detectionRadius = detectionRadius;
            _attackRange = attackRange;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}
