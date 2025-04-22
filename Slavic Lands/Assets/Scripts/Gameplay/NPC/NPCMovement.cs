using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.NPC
{
    public class NPCMovement : MonoBehaviour
    {
        [SerializeField] protected float _walkSpeed = 1f;
        [SerializeField] protected float _runSpeed = 1f;
        [SerializeField] protected float _activeSpeed = 1f;
        [SerializeField] protected float _idleDuration = 1f;
        [SerializeField] protected float _idleAttackDuration = 0.5f;

        [SerializeField] protected bool _isIdle = true;
        [SerializeField] protected bool _isRunning;
        [SerializeField] protected bool _goRight = true;

        [SerializeField] protected float _minWalkRange;
        [SerializeField] protected float _maxWalkRange;

        [SerializeField] protected float _movePoint;

        private NPCAttack _npcAttack;

        private void Awake()
        {
            _npcAttack = GetComponent<NPCAttack>();
        }

        private void OnEnable()
        {
            _npcAttack.OnStartChasing += IsRunning;
            _npcAttack.OnStopChasing += IsRunning;
            _npcAttack.OnStartChasing += GetNewWalkPoint;
            _npcAttack.OnStopChasing += GetNewWalkPoint;
            _npcAttack.OnStartAttacking += OnStartAttacking;
            _npcAttack.OnStopAttacking += OnStopAttacking;
        }

        private void OnDisable()
        {
            _npcAttack.OnStartChasing -= IsRunning;
            _npcAttack.OnStopChasing -= IsRunning;
            _npcAttack.OnStartChasing -= GetNewWalkPoint;
            _npcAttack.OnStopChasing -= GetNewWalkPoint;
            _npcAttack.OnStartAttacking -= OnStartAttacking;
            _npcAttack.OnStopAttacking -= OnStopAttacking;
        }

        protected void Start()
        {
            GetNewWalkPoint(_npcAttack.IsChasing);
            _activeSpeed = _walkSpeed;
        }

        protected void FixedUpdate()
        {
            if (_npcAttack.IsAttacking) return;
            
            if (_isIdle) return;
            
            Move();

            if (IsArrived())
            {
                Debug.Log("Idle");
                _isIdle = true;
                GetNewWalkPoint(_npcAttack.IsChasing);
            }
        }

        protected virtual void Move()
        {
            if (_goRight)
            {
                Debug.Log("Moving right --->");
                transform.Translate(Vector2.right * _activeSpeed * Time.deltaTime);
            }
            else
            {
                Debug.Log("Moving right <---");
                transform.Translate(Vector2.left * _activeSpeed * Time.deltaTime);
            }
        }

        protected virtual void SetSpeed()
        {
            if (_isRunning)
                _activeSpeed = _runSpeed;
            else
                _activeSpeed = _walkSpeed;
        }

        protected virtual void GetNewWalkPoint(bool isChasing = false)
        {
            float newPoint = 0;

            if (isChasing)
            {
                Debug.Log("New Point chasing");
                newPoint = _npcAttack.Target.position.x;
            }
            else
            {
                Debug.Log("New Point");
                newPoint = Random.Range(_minWalkRange, _maxWalkRange);
            }


            if (newPoint < transform.position.x)
                _goRight = false;
            else if (newPoint > transform.position.x)
                _goRight = true;

            StartCoroutine(Wait());
            _movePoint = newPoint;
        }

        protected virtual bool IsArrived()
        {
            if (_goRight)
            {
                if (transform.position.x >= _movePoint)
                {
                    return true;
                }
            }
            else
            {
                if (transform.position.x <= _movePoint)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual IEnumerator Wait()
        {
            float timer = _npcAttack.IsChasing ? _idleAttackDuration : _idleDuration;
            yield return new WaitForSeconds(timer);
            _isIdle = false;
        }

        protected virtual void IsRunning(bool isChasing)
        {
            _isRunning = isChasing;
            SetSpeed();
        }

        private void OnStartAttacking(bool isAttacking)
        {
            Debug.Log("On START Attacking");
            _isIdle = isAttacking;
            
        }

        private void OnStopAttacking(bool isAttacking = false)
        {
            Debug.Log("On STOP Attacking");
            GetNewWalkPoint(_npcAttack.IsChasing);
        }
    }
}