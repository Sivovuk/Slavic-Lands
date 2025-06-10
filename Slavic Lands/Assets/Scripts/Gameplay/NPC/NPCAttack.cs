using System;
using Interfaces;
using UnityEngine;

namespace Gameplay.NPC
{
    public class NPCAttack : MonoBehaviour
    {
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _attackDamage = 1f;
        [field:SerializeField] public bool IsAttacking { get; private set; }
        [field:SerializeField] public bool IsChasing { get; private set; }
        [field:SerializeField] public Transform Target { get; private set; }
        
        [Header("Boxes")]
        [SerializeField] private Collider2D _boxCollider;
        [SerializeField] private Vector2 _detectionBoxSize;
        [SerializeField] private Vector2 _attackBoxSize;
        [SerializeField] private LayerMask _playerLayer, _npcLayer, _animalLayer;
        
        private float _timePassed;

        public Action<bool> OnStartChasing;
        public Action<bool> OnStopChasing;
        public Action<bool> OnStartAttacking;
        public Action<bool> OnStopAttacking;

        [Header("DEBUG STUFF")] 
        [SerializeField] private GameObject _attackIndicator;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();

            _timePassed = _attackCooldown;
        }

        private void Update()
        {
            if (IsAttacking)
            {
                _timePassed += Time.deltaTime;
                if (_timePassed >= _attackCooldown)
                {
                    _timePassed = 0;
                    Attack();
                }
            }
        }

        private void Attack()
        {
            Target.GetComponent<IHit>().TakeDamage(_attackDamage);
            _attackIndicator.SetActive(true);
            Invoke(nameof(AttackIndicatorDEBUG), 0.2f);
            // add logic here for leveling the animals, npc, ect.
        }

        private void AttackIndicatorDEBUG()
        {
            _attackIndicator.SetActive(false);
        }
        
        

        public void CheckDetectCollider(Collider2D other, bool isEntered)
        {
            //if (other != _boxCollider) return;
            //Debug.Log("CheckDetectCollider called");
            if (isEntered)
            {
                if (Target != null) return;
                //Debug.Log(other.tag + " " + other.name);
                if (other.CompareTag("Player"))
                {
                    IsChasing = true;
                    Target = other.transform;
                    OnStartChasing?.Invoke(IsChasing);
                }
            }
            else
            {
                if (Target != null && Target.CompareTag(other.tag))
                {
                    Target = null;
                    IsChasing = false;
                    OnStopChasing?.Invoke(IsChasing);
                }
            }
        }
        
        public void CheckAttackCollider(Collider2D other, bool isEntered)
        {
            if (Target == null && other != _boxCollider) return;
            //Debug.Log("CheckAttackCollider called");
            
            if (isEntered)
            {
                if (Target.CompareTag(other.tag))
                {
                    IsAttacking = true;
                    OnStartAttacking?.Invoke(IsAttacking);
                }
            }
            else
            {
                if (Target.CompareTag(other.tag))
                {
                    IsAttacking = false;
                    OnStopAttacking?.Invoke(IsAttacking);
                    _timePassed = 0;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            //Vector2 boxCenter = (Vector2)transform.position + _detectionBoxSize;
            Gizmos.DrawWireCube(transform.position, _detectionBoxSize);
            
            Gizmos.color = Color.red;
            //Vector2 boxCenter = (Vector2)transform.position + _attackBoxSize;
            Gizmos.DrawWireCube(transform.position, _attackBoxSize);
        }
    }
}