using System.Collections;
using Data;
using UnityEngine;

namespace Entities
{
    public class EntityMovement : MonoBehaviour
    {
        [Header("Movement State")]
        [SerializeField] private bool _isIdle = true;
        [SerializeField] private bool _isRunning;
        [SerializeField] private bool _isStunned;
        [SerializeField] private bool _goRight = true;

        [Header("Idle Settings")]
        [SerializeField] private float _idleDuration = 1f;
        [SerializeField] private float _idleAttackDuration = 0.5f;

        [Header("Movement Range")]
        [SerializeField] private float _minWalkRange = -3f;
        [SerializeField] private float _maxWalkRange = 3f;

        private float _walkSpeed = 1f;
        private float _runSpeed = 2f;
        private float _activeSpeed;
        private float _targetXPosition;

        private Coroutine _idleCoroutine;
        private EntityAttack _attackComponent;

        protected virtual void Awake()
        {
            _attackComponent = GetComponent<EntityAttack>();
        }

        protected virtual void OnEnable()
        {
            if (_attackComponent != null)
            {
                _attackComponent.OnStartChasing += SetRunningState;
                _attackComponent.OnStopChasing += SetRunningState;
                _attackComponent.OnStartChasing += SetNewTargetPosition;
                _attackComponent.OnStopChasing += SetNewTargetPosition;
                _attackComponent.OnStartAttacking += HandleStartAttacking;
                _attackComponent.OnStopAttacking += HandleStopAttacking;
            }
        }

        protected virtual void OnDisable()
        {
            if (_attackComponent != null)
            {
                _attackComponent.OnStartChasing -= SetRunningState;
                _attackComponent.OnStopChasing -= SetRunningState;
                _attackComponent.OnStartChasing -= SetNewTargetPosition;
                _attackComponent.OnStopChasing -= SetNewTargetPosition;
                _attackComponent.OnStartAttacking -= HandleStartAttacking;
                _attackComponent.OnStopAttacking -= HandleStopAttacking;
            }
        }

        protected virtual void Start()
        {
            SetNewTargetPosition(_attackComponent != null && _attackComponent.IsChasing);
            _activeSpeed = _walkSpeed;
        }

        protected virtual void FixedUpdate()
        {
            if (_attackComponent != null && _attackComponent.IsAttacking) return;
            if (_isIdle || _isStunned) return;

            Move();

            if (HasReachedDestination())
            {
                _isIdle = true;
                SetNewTargetPosition(_attackComponent != null && _attackComponent.IsChasing);
            }
        }

        protected virtual void Move()
        {
            float direction = _goRight ? 1f : -1f;
            transform.Translate(Vector2.right * direction * _activeSpeed * Time.deltaTime);
        }

        protected virtual void SetRunningState(bool isChasing)
        {
            _isRunning = isChasing;
            _activeSpeed = isChasing ? _runSpeed : _walkSpeed;
        }

        protected virtual void SetNewTargetPosition(bool isChasingOrFleeing)
        {
            if (_idleCoroutine != null)
                StopCoroutine(_idleCoroutine);

            float newTarget;

            if (_attackComponent != null && _attackComponent.Target != null)
            {
                float targetX = _attackComponent.Target.position.x;
                float currentX = transform.position.x;

                if (_attackComponent.IsFleeing)
                {
                    // Move away from target
                    float distance = Random.Range(_minWalkRange, _maxWalkRange);
                    newTarget = targetX > currentX
                        ? currentX - distance
                        : currentX + distance;
                }
                else if (_attackComponent.IsChasing)
                {
                    newTarget = targetX;
                }
                else
                {
                    newTarget = Random.Range(_minWalkRange, _maxWalkRange) + currentX;
                }
            }
            else
            {
                newTarget = Random.Range(_minWalkRange, _maxWalkRange) + transform.position.x;
            }

            _goRight = newTarget > transform.position.x;
            _targetXPosition = newTarget;

            _idleCoroutine = StartCoroutine(IdleWaitCoroutine(isChasingOrFleeing));
        }

        protected virtual bool HasReachedDestination()
        {
            return _goRight
                ? transform.position.x >= _targetXPosition
                : transform.position.x <= _targetXPosition;
        }

        protected virtual IEnumerator IdleWaitCoroutine(bool isChasing)
        {
            float delay = isChasing ? _idleAttackDuration : _idleDuration;
            yield return new WaitForSeconds(delay);
            _isIdle = false;
        }

        protected virtual void HandleStartAttacking(bool isAttacking) => _isIdle = isAttacking;
        protected virtual void HandleStopAttacking(bool isAttacking = false) =>
            SetNewTargetPosition(_attackComponent != null && _attackComponent.IsChasing);

        public void SetMovementStats(EntitySO data)
        {
            _walkSpeed = data.WalkSpeed;
            _runSpeed = data.RunSpeed;
            _activeSpeed = _walkSpeed;
            _minWalkRange = data.MinWalkRange;
            _maxWalkRange = data.MaxWalkRange;
            _idleDuration = data.IdleDuration;
            _idleAttackDuration = data.IdleAttackDuration;
        }

        public bool IsStunned
        {
            get => _isStunned;
            set => _isStunned = value;
        }
    }
}
