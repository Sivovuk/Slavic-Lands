using System.Collections;
using Data;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Controls the movement behavior of AI entities, supporting idle wandering, chasing, fleeing,
    /// and responding to attack state via events from EntityAttack.
    /// </summary>
    public class EntityMovement : MonoBehaviour
    {
        // --- MOVEMENT STATES ---

        [Header("Movement State")]
        [SerializeField] private bool _isIdle = true;     // Whether the entity is currently idle
        [SerializeField] private bool _isRunning;         // Whether the entity is chasing
        [SerializeField] private bool _isStunned;         // Whether movement is disabled
        [SerializeField] private bool _goRight = true;    // Determines direction of movement

        // --- IDLE TIMING CONFIGURATION ---

        [Header("Idle Settings")]
        [SerializeField] private float _idleDuration = 1f;
        [SerializeField] private float _idleAttackDuration = 0.5f;

        // --- WALK RANGE CONFIGURATION ---

        [Header("Movement Range")]
        [SerializeField] private float _minWalkRange = -3f;
        [SerializeField] private float _maxWalkRange = 3f;

        // --- INTERNAL STATE VARIABLES ---

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
            // Subscribe to events for combat-driven state changes
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
            // Unsubscribe from events to prevent memory leaks
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
            // Begin by setting an initial target location
            SetNewTargetPosition(_attackComponent != null && _attackComponent.IsChasing);
            _activeSpeed = _walkSpeed;
        }

        protected virtual void FixedUpdate()
        {
            // Do not move if attacking, idle, or stunned
            if (_attackComponent != null && _attackComponent.IsAttacking) return;
            if (_isIdle || _isStunned) return;

            Move();

            // If destination reached, pause and reset behavior
            if (HasReachedDestination())
            {
                _isIdle = true;
                SetNewTargetPosition(_attackComponent != null && _attackComponent.IsChasing);
            }
        }

        /// <summary>
        /// Moves the entity in the currently set direction.
        /// </summary>
        protected virtual void Move()
        {
            float direction = _goRight ? 1f : -1f;
            transform.Translate(Vector2.right * direction * _activeSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Switches between walking and running states based on chase logic.
        /// </summary>
        protected virtual void SetRunningState(bool isChasing)
        {
            _isRunning = isChasing;
            _activeSpeed = isChasing ? _runSpeed : _walkSpeed;
        }

        /// <summary>
        /// Determines a new target X position based on idle, chase, or flee state.
        /// </summary>
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
                    float distance = Random.Range(_minWalkRange, _maxWalkRange);
                    newTarget = targetX > currentX ? currentX - distance : currentX + distance;
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

        /// <summary>
        /// Checks if the entity has reached its intended horizontal destination.
        /// </summary>
        protected virtual bool HasReachedDestination()
        {
            return _goRight
                ? transform.position.x >= _targetXPosition
                : transform.position.x <= _targetXPosition;
        }

        /// <summary>
        /// Waits for idle delay before starting movement again.
        /// </summary>
        protected virtual IEnumerator IdleWaitCoroutine(bool isChasing)
        {
            float delay = isChasing ? _idleAttackDuration : _idleDuration;
            yield return new WaitForSeconds(delay);
            _isIdle = false;
        }

        /// <summary>
        /// Stops movement temporarily when attack begins.
        /// </summary>
        protected virtual void HandleStartAttacking(bool isAttacking) => _isIdle = isAttacking;

        /// <summary>
        /// Resumes movement and recalculates position after an attack ends.
        /// </summary>
        protected virtual void HandleStopAttacking(bool isAttacking = false) =>
            SetNewTargetPosition(_attackComponent != null && _attackComponent.IsChasing);

        /// <summary>
        /// Receives movement stats from EntitySO.
        /// </summary>
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

        // Property for externally controlling stun state
        public bool IsStunned
        {
            get => _isStunned;
            set => _isStunned = value;
        }
    }
}
