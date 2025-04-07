using System;
using System.Collections;
using Interfaces;
using TMPro;
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
        
        [Header("Jump")]
        private float _jumpForce;
        [SerializeField] private float _jumpRaycastDistance;
        
        [Header("Dash")]
        [SerializeField]private float _dashingCost;
        [SerializeField]private float _dashingPower;
        [SerializeField]private float _dashingTime;
        [SerializeField]private float _dashCooldown;
        [SerializeField] private TrailRenderer _trailRenderer;
        private bool _canDash = true;
        private bool _isDashing;

        [SerializeField] private bool _isMoving;
        [SerializeField] private bool _isGrounded;
        
        [Header("References")]
        [SerializeField] private Transform _jumpRaycast;
        [SerializeField] private Transform _playerSetup;
        
        private Player _player;
        private PlayerInputSystem _playerInputSystem;
        private PlayerEnergy _playerEnergy;
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private BoxCollider2D _boxCollider2D;
        
        [SerializeField] private LayerMask _groundLayer;
        
        [Header("DEBUG")]
        [SerializeField] private TMP_Text Velocity;
        
        private void Awake()
        {
            _playerInputSystem = GetComponent<PlayerInputSystem>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _boxCollider2D = GetComponent<BoxCollider2D>(); 
            _playerEnergy = GetComponent<PlayerEnergy>();
        }

        private void OnEnable()
        {
            _playerInputSystem.OnSprintClick += Sprint;
            _playerInputSystem.OnJumpClick += Jump;
            _playerInputSystem.OnDashClicked += Dash;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnSprintClick -= Sprint;
            _playerInputSystem.OnJumpClick -= Jump;
            _playerInputSystem.OnDashClicked -= Dash;
        }

        private void Update()
        {
            IsGrounded();
            
            Velocity.text = $"Velocity : {_rigidbody2D.linearVelocity.x}";
        }

        private void FixedUpdate()
        {
            if (_isMoving && !_isDashing)
                Move();
        }

        private void Move()
        {
            transform.Translate((Vector2.right * _playerInputSystem.MovementValue.x) * _activeSpeed * Time.deltaTime);
            SetDirection(_playerInputSystem.MovementValue.x);
        }

        private void Sprint(bool value)
        {
            if(!_player.PlayerEnergy.Sprint(value && (_playerInputSystem.MovementValue.x != 0)) || _isDashing)
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
            _dashingCost = playerSO.DashCost;
            _dashingPower = playerSO.DashingPower;
            _dashingTime = playerSO.DashingTime;
            _dashCooldown = playerSO.DashCooldown;
            
            _activeSpeed = _walkSpeed;
            _isMoving = true;
        }

        public void Dash()
        {
            if (_dashingCost <= _playerEnergy.GetCurrentEnergy())
                if (!_isDashing)
                {
                    StartCoroutine(DashCorutine());
                    _playerEnergy.UseEnergy(_dashingCost);
                }
        }

        private IEnumerator DashCorutine()
        {
            _canDash = false;
            _isDashing = true;
            
            float originalGrabity = _rigidbody2D.gravityScale;
            _rigidbody2D.gravityScale = 0f;
            
            float direction = _goingRight ? transform.localScale.x : -transform.localScale.x;
            _rigidbody2D.linearVelocity = new Vector2(direction * _dashingPower, _rigidbody2D.linearVelocity.y);
            
            _trailRenderer.emitting = true;
            
            yield return new WaitForSeconds(_dashingTime);
            
            _trailRenderer.emitting = false;
            _rigidbody2D.linearVelocity = new Vector2(0f, _rigidbody2D.linearVelocity.y);
            _rigidbody2D.gravityScale = originalGrabity;
            _isDashing = false;
            
            yield return new WaitForSeconds(_dashCooldown);
            
            _canDash = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - (_playerHeight * 0.5f + 0.2f), transform.position.z));
            Gizmos.DrawLine(_jumpRaycast.position, new Vector3(_jumpRaycast.position.x , _jumpRaycast.position.y - _jumpRaycastDistance, _jumpRaycast.position.z));
        }
    }
}