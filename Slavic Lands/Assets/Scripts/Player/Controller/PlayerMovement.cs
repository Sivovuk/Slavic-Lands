using System;
using System.Collections;
using Core.Interfaces;
using Data;
using TMPro;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerMovement : MonoBehaviour, ILoadingStatsPlayer
    {
        // Movement
        private float _activeSpeed;
        private float _walkSpeed;
        private float _runSpeed;
        private bool _goingRight;

        // Jump
        private float _jumpForce;
        [SerializeField] private float _jumpRaycastDistance;

        // Dash
        private float _dashingCost;
        private float _dashingPower;
        private float _dashingTime;
        private float _dashCooldown;
        private bool _canDash = true;
        private bool _isDashing;

        // State
        private bool _isMoving;
        private bool _isGrounded;

        // References
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Transform _jumpRaycast;
        [SerializeField] private Transform _playerSetup;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private TMP_Text _velocityText;

        private PlayerController _playerController;
        private PlayerInputSystem _playerInputSystem;
        private PlayerEnergy _playerEnergy;
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private BoxCollider2D _boxCollider2D;

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
            CheckIfGrounded();

            //delete this
            if (_velocityText != null)
                _velocityText.text = $"Velocity : {_rigidbody2D.linearVelocity.x:F2}";
        }

        private void FixedUpdate()
        {
            if (_isMoving && !_isDashing)
                Move();
        }

        private void Move()
        {
            var direction = _playerInputSystem.MovementValue.x;
            transform.Translate(Vector2.right * direction * _activeSpeed * Time.deltaTime);
            SetDirection(direction);
        }

        private void Sprint(bool isSprinting)
        {
            if (!_playerController.PlayerEnergy.Sprint(isSprinting && _playerInputSystem.MovementValue.x != 0f) || _isDashing)
                return;

            _activeSpeed = isSprinting ? _runSpeed : _walkSpeed;
        }

        private void Jump()
        {
            if (_isGrounded)
                _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }

        private void CheckIfGrounded()
        {
            _isGrounded = Physics2D.Raycast(_jumpRaycast.position, Vector3.down, _jumpRaycastDistance, _groundLayer);
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

        public void LoadPlayerStats(PlayerSO playerSO, PlayerController playerController)
        {
            _playerController = playerController;

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
            if (_dashingCost <= _playerEnergy.GetCurrentEnergy() && !_isDashing)
            {
                StartCoroutine(DashRoutine());
                _playerEnergy.UseEnergy(_dashingCost);
            }
        }

        private IEnumerator DashRoutine()
        {
            _canDash = false;
            _isDashing = true;

            float originalGravity = _rigidbody2D.gravityScale;
            _rigidbody2D.gravityScale = 0f;

            float direction = _goingRight ? 1f : -1f;
            _rigidbody2D.linearVelocity  = new Vector2(direction * _dashingPower, _rigidbody2D.linearVelocity .y);

            _trailRenderer.emitting = true;

            yield return new WaitForSeconds(_dashingTime);

            _trailRenderer.emitting = false;
            _rigidbody2D.linearVelocity  = new Vector2(0f, _rigidbody2D.linearVelocity .y);
            _rigidbody2D.gravityScale = originalGravity;
            _isDashing = false;

            yield return new WaitForSeconds(_dashCooldown);

            _canDash = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_jumpRaycast.position, _jumpRaycast.position + Vector3.down * _jumpRaycastDistance);
        }
    }
}
