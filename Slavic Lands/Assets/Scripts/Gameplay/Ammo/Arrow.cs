using System;
using Gameplay.Player;
using Interfaces;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private ActionType _actionType = ActionType.Shoot;
    public Rigidbody2D Rigidbody2D { get; private set; }
    private Collider2D _collider2D;
    private PlayerAttack _playerAttack;
    
    private float _pushForce;
    
    [SerializeField] private bool _applyForce;
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

    public void Init(PlayerAttack playerAttack, float pushForce)
    {
        _playerAttack = playerAttack;
        _isFlying = true;
        _pushForce = pushForce;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IHit>(out IHit hit) && !_alreadyHit)
        {
            Vector2 direction = other.transform.position - _playerAttack.transform.position;
            direction.x = direction.x > 0 ? 1 : -1;
            direction.y = direction.y > 0 ? 1 : -1;
            Debug.Log(_applyForce + " : " + _pushForce);
            _playerAttack.HandleHit(_actionType, hit, _applyForce, _pushForce, direction);
            
            _isFlying = false;
            Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            Rigidbody2D.linearVelocity = Vector2.zero;
            Destroy(gameObject, 3);
            _alreadyHit = true;
        }
        else if (other.CompareTag("Ground"))
        {
            _isFlying = false;
            Rigidbody2D.bodyType = RigidbodyType2D.Static;
            Destroy(gameObject, 3);
            _alreadyHit = true;
        }
    }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.TryGetComponent<IHit>(out IHit hit))
    //     {
    //         Debug.LogError("hit " + other.name);
    //         
    //         _isFlying = true;
    //         Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    //     }
    // }
}
