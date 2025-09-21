using System;
using Core.Interfaces;
using Gameplay.Player;
using UnityEngine;

/// <summary>
/// This script controls the behavior of an arrow projectile fired by the player.
/// It handles movement, collision detection, applying damage, and visual rotation.
/// </summary>
public class ArrowProjectile : MonoBehaviour
{
    // Type of tool used for this projectile (in this case, a bow)
    [SerializeField] private ToolType _actionType = ToolType.Bow;
    
    // Determines whether the projectile should apply knockback force on hit
    [SerializeField] private bool _applyForce;

    // Public getter for the Rigidbody2D component
    public Rigidbody2D Rigidbody2D { get; private set; }

    // Cached references
    private Collider2D _collider2D;
    private PlayerCombat _playerCombat;

    // Damage and push force values to apply on hit
    private float _damage;
    private float _pushForce;

    // Flags to track projectile state
    private bool _isFlying = false;
    private bool _alreadyHit = false;

    private void Awake()
    {
        // Get required components at initialization
        Rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // While flying, rotate the arrow to match the velocity direction
        if (_isFlying)
        {
            float angle = Mathf.Atan2(Rigidbody2D.linearVelocity.y, Rigidbody2D.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    /// <summary>
    /// Initializes the projectile with damage, push force, and the originating player combat script.
    /// </summary>
    public void Init(PlayerCombat playerCombat, float damage, float pushForce)
    {
        _playerCombat = playerCombat;
        _pushForce = pushForce;
        _isFlying = true;
        _damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Avoid multiple collisions
        if (_alreadyHit) return;

        // If the arrow hits something damageable (e.g., enemy)
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            // Calculate direction for knockback
            Vector2 direction = (other.transform.position - transform.position).normalized;

            // Apply damage using the bow tool type
            damageable.TakeDamage(_damage, ToolType.Bow);

            // Optionally apply knockback
            if (_applyForce)
            {
                damageable.ApplyKnockback(direction, _pushForce);
            }

            // Stop movement and mark the projectile as spent
            StopProjectile(RigidbodyType2D.Kinematic);
        }
        // If it hits the ground, just stop without applying effects
        else if (other.CompareTag("Ground"))
        {
            StopProjectile(RigidbodyType2D.Static);
        }
    }

    /// <summary>
    /// Stops the projectile's movement and schedules it for destruction.
    /// </summary>
    private void StopProjectile(RigidbodyType2D bodyType)
    {
        _isFlying = false;
        _alreadyHit = true;

        // Stop movement
        Rigidbody2D.linearVelocity = Vector2.zero;
        Rigidbody2D.bodyType = bodyType;

        // Destroy the arrow after 3 seconds (e.g., allow time for animations or effects)
        Destroy(gameObject, 3f);
    }
}
