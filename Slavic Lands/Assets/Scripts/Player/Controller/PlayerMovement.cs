using System;
using System.Collections;
using Core.Interfaces;
using Data;
using TMPro;
using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Manages all player movement behaviors including walking, sprinting, jumping, and dashing.
    /// It loads movement-related stats from PlayerSO and integrates with PlayerInputSystem.
    /// </summary>
    public class PlayerMovement : MonoBehaviour, ILoadingStatsPlayer
    {
        // --- Movement ---
        private float _activeSpeed;
        private float _walkSpeed;
        private float _runSpeed;
        private bool _goingRight;

        // --- Jumping ---
        private float _jumpForce;
        [SerializeField] private float _jumpRaycastDistance;

        // --- Dashing ---
        private float _dashingCost;
        private float _dashingPower;
        private float _dashingTime;
        private float _dashCooldown;
        private bool _canDash = true;
        private bool _isDashing;

        // --- State ---
        private bool _isMoving;
        private bool _isGrounded;

        public event Action<bool> OnActionLockChanged;
        public bool IsActionLocked { get; private set; }

        // --- References ---
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private Transform _jumpRaycast;
        [SerializeField] private Transform _playerSetup;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private TMP_Text _velocityText;

        private PlayerController _playerController;
        private PlayerInputSystem _playerInputSystem;
        private PlayerEnergy _playerEnergy;
        private Rigidbody2D _rigidbody2D;
        private BoxCollider2D _boxCollider2D;
        private SpriteRenderer _spriteRenderer;
        private PlayerAnimationController _animationController;

        private void Awake()
        {
            _playerInputSystem = GetComponent<PlayerInputSystem>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _animationController = GetComponent<PlayerAnimationController>();
            _playerEnergy = GetComponent<PlayerEnergy>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
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

            // Debug velocity display
            if (_velocityText != null)
                _velocityText.text = $"Velocity : {_rigidbody2D.linearVelocity.x:F2}";
        }

        private void FixedUpdate()
        {
            if (_isMoving && !_isDashing)
                Move();

            UpdateMovementAnimation();
        }

        /// <summary>
        /// Handles standard left/right movement based on input.
        /// </summary>
        private void Move()
        {
            var direction = _playerInputSystem.MovementValue.x;
            transform.Translate(Vector2.right * direction * _activeSpeed * Time.deltaTime);
            SetDirection(direction);
        }

        /// <summary>
        /// Toggles between walk and sprint depending on stamina availability and player input.
        /// </summary>
        private void Sprint(bool isSprinting)
        {
            if (!_playerController.PlayerEnergy.Sprint(isSprinting && _playerInputSystem.MovementValue.x != 0f) ||
                _isDashing)
                return;

            _activeSpeed = isSprinting ? _runSpeed : _walkSpeed;
        }

        /// <summary>
        /// Allows the player to jump when grounded.
        /// </summary>
        private void Jump()
        {
            if (_isGrounded)
                _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }

        /// <summary>
        /// Checks if the player is grounded using a raycast.
        /// </summary>
        private void CheckIfGrounded()
        {
            _isGrounded = Physics2D.Raycast(_jumpRaycast.position, Vector3.down, _jumpRaycastDistance, _groundLayer);
        }

        /// <summary>
        /// Updates player sprite orientation based on direction.
        /// </summary>
        public void SetDirection(float direction)
        {
            if (direction > 0)
            {
                _goingRight = true;
                _playerSetup.rotation = Quaternion.Euler(0f, 0f, 0f);
                _spriteRenderer.flipX = false;
            }
            else if (direction < 0)
            {
                _goingRight = false;
                _playerSetup.rotation = Quaternion.Euler(0f, 180f, 0f);
                _spriteRenderer.flipX = true;
            }
        }

        /// <summary>
        /// Loads all player movement stats from PlayerSO.
        /// </summary>
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

        /// <summary>
        /// Initiates a dash if enough energy is available.
        /// </summary>
        public void Dash()
        {
            if (_dashingCost <= _playerEnergy.GetCurrentEnergy() && !_isDashing)
            {
                StartCoroutine(DashRoutine());
                _playerEnergy.UseEnergy(_dashingCost);
            }
        }

        /// <summary>
        /// Executes dash behavior with cooldown and visual effects.
        /// </summary>
        private IEnumerator DashRoutine()
        {
            _canDash = false;
            _isDashing = true;

            float originalGravity = _rigidbody2D.gravityScale;
            _rigidbody2D.gravityScale = 0f;

            float direction = _goingRight ? 1f : -1f;
            _rigidbody2D.linearVelocity = new Vector2(direction * _dashingPower, _rigidbody2D.linearVelocity.y);

            _trailRenderer.emitting = true;

            yield return new WaitForSeconds(_dashingTime);

            _trailRenderer.emitting = false;
            _rigidbody2D.linearVelocity = new Vector2(0f, _rigidbody2D.linearVelocity.y);
            _rigidbody2D.gravityScale = originalGravity;
            _isDashing = false;

            yield return new WaitForSeconds(_dashCooldown);
            _canDash = true;
        }

        /// <summary>
        /// Draws the jump raycast line for debugging in the editor.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_jumpRaycast.position, _jumpRaycast.position + Vector3.down * _jumpRaycastDistance);
        }

        /// <summary>
        /// Updates the movement blend tree parameter via PlayerAnimationController.
        /// </summary>
        private void UpdateMovementAnimation()
        {
            float inputMagnitude = Mathf.Abs(_playerInputSystem.MovementValue.x);

            if (inputMagnitude < 0.01f || _isDashing)
            {
                _animationController.UpdateMovement(0f);
            }
            else
            {
                float blend = Mathf.Approximately(_activeSpeed, _runSpeed) ? 1f : 0.5f;
                _animationController.UpdateMovement(blend);
            }
        }

        /// <summary>
        /// Locks or unlocks player movement. Used by StateMachineBehaviours during actions.
        /// </summary>
        public void SetMovementLock(bool isLocked)
        {
            _isMoving = !isLocked;
            IsActionLocked = isLocked;
            OnActionLockChanged?.Invoke(isLocked);
            
            // If we just got locked, ensure we stop sliding
            if (isLocked)
            {
                _animationController.UpdateMovement(0f);
            }
        }
    }
}
