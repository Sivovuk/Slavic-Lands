using System;
using Core.Interfaces;
using Gameplay.Player;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] private ToolType _actionType = ToolType.Bow;
    [SerializeField] private bool _applyForce;
    
    public Rigidbody2D Rigidbody2D { get; private set; }
    
    private Collider2D _collider2D;
    private PlayerCombat _playerCombat;
    
    private float _damage;
    private float _pushForce;
    
    private bool _isFlying = false;
    private bool _alreadyHit = false;

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (_isFlying)
        {
            float angle = Mathf.Atan2(Rigidbody2D.linearVelocity.y, Rigidbody2D.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void Init(PlayerCombat playerCombat, float damage, float pushForce)
    {
        _playerCombat = playerCombat;
        _pushForce = pushForce;
        _isFlying = true;
        _damage = damage;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_alreadyHit) return;
        
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            damageable.TakeDamage(_damage, ToolType.Bow);

            if (_applyForce)
            {
                damageable.ApplyKnockback(direction, _pushForce);
            }

            StopProjectile(RigidbodyType2D.Kinematic);
        }
        else if (other.CompareTag("Ground"))
        {
            StopProjectile(RigidbodyType2D.Static);
        }
    }

    private void StopProjectile(RigidbodyType2D bodyType)
    {
        _isFlying = false;
        _alreadyHit = true;

        Rigidbody2D.linearVelocity = Vector2.zero;
        Rigidbody2D.bodyType = bodyType;

        Destroy(gameObject, 3f);
    }
}
