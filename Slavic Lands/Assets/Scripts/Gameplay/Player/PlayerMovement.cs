using System;
using Interfaces;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerMovement : MonoBehaviour, ILoadingStatsPlayer
    {
        //  Move
        private float _activeSpeed;
        private float _walkSpeed;
        private float _runSpeed;
        private bool _goingRight;
        
        // Jump
        [Header("Jump")]
        private float _jumpForce;
        [SerializeField] private float _jumpRaycastDistance;

        [SerializeField] private bool _isMoving;
        [SerializeField] private bool _isGrounded;
        
        // References
        [Header("References")]
        [SerializeField] private Transform _jumpRaycast;
        [SerializeField] private Transform _playerSetup;
        private Player _player;
        private PlayerInputSystem _playerInputSystem;
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private BoxCollider2D _boxCollider2D;
        
        [SerializeField] private LayerMask _groundLayer;
        
        private void Awake()
        {
            _playerInputSystem = GetComponent<PlayerInputSystem>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _boxCollider2D = GetComponent<BoxCollider2D>(); 
        }

        private void OnEnable()
        {
            _playerInputSystem.OnSprintClick += Sprint;
            _playerInputSystem.OnJumpClick += Jump;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnSprintClick -= Sprint;
            _playerInputSystem.OnJumpClick -= Jump;
        }

        private void Update()
        {
            IsGrounded();
        }

        private void FixedUpdate()
        {
            if (_isMoving)
            {
                Move();
            }
        }

        private void Move()
        {
            transform.Translate((Vector2.right * _playerInputSystem.MovementValue.x) * _activeSpeed * Time.deltaTime);
            SetDirection(_playerInputSystem.MovementValue.x);
        }

        private void Sprint(bool value)
        {
            if(!_player.PlayerEnergy.Sprint(value && (_playerInputSystem.MovementValue.x != 0)))
                return;
            
            if (value)
                _activeSpeed = _runSpeed;
            else
                _activeSpeed = _walkSpeed;
        }

        private void Jump()
        {
            if (!_isGrounded) return;
            _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }

        private void IsGrounded()
        {
            if (Physics2D.Raycast(_jumpRaycast.position, Vector3.down, 0.1f, _groundLayer))
                _isGrounded = true;
            else
                _isGrounded = false;
        }

        public void SetDirection(float direction)
        {
            if (direction > 0)
            {
                _goingRight = true;
                _playerSetup.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (direction < 0)
            {
                _goingRight = false;
                _playerSetup.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

        }

        public void LoadPlayerStats(PlayerSO playerSO, Player player)
        {
            _player = player;
            _walkSpeed = playerSO.WalkSpeed;
            _runSpeed = playerSO.RunSpeed;
            _jumpForce = playerSO.JumpForce;
            
            _activeSpeed = _walkSpeed;
            _isMoving = true;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - (_playerHeight * 0.5f + 0.2f), transform.position.z));
            Gizmos.DrawLine(_jumpRaycast.position, new Vector3(_jumpRaycast.position.x , _jumpRaycast.position.y - _jumpRaycastDistance, _jumpRaycast.position.z));
        }
    }
}