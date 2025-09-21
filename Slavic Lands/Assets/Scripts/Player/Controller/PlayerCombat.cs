using System;
using System.Collections.Generic;
using Core;
using Core.Interfaces;
using Data;
using Managers;
using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles all player combat-related logic including melee attacks, ranged attacks, shield activation, 
    /// ability switching, and XP reward handling. Integrates with tool system and player profile.
    /// </summary>
    public class PlayerCombat : MonoBehaviour, ILoadingStatsPlayer
    {
        // --- SHOOTING CONFIG ---

        [Header("Shooting")]
        [SerializeField] private float _shootForce = 50f;
        private bool _isShooted; // Prevents rapid re-firing

        // --- SHIELD CONFIG ---

        [Header("Shield")]
        private bool _isShieldActive;

        // --- REFERENCES ---

        [Header("References")]
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private float _attackRange;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private ArrowProjectile _arrowProjectilePrefab;
        [SerializeField] private ArrowProjectile _piercingArrowProjectilePrefab;
        [SerializeField] private GameObject _shield;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private GameObject _actionHUD;

        private PlayerController _playerController;
        private PlayerSO _playerSO;
        private PlayerMovement _playerMovement;
        private PlayerInputSystem _playerInputSystem;

        private ToolType _equippedTool = ToolType.None;

        private void Awake()
        {
            _playerInputSystem = GetComponent<PlayerInputSystem>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnEnable()
        {
            _playerInputSystem.OnLmbClick += UseEquippedTool;
            _playerInputSystem.OnRmbClick += ActiveShield;
            _playerInputSystem.OnActionChanged += ShowHUD;
            _playerInputSystem.OnAbilitySelect += SelectAction;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnLmbClick -= UseEquippedTool;
            _playerInputSystem.OnRmbClick -= ActiveShield;
            _playerInputSystem.OnActionChanged -= ShowHUD;
            _playerInputSystem.OnAbilitySelect -= SelectAction;
        }

        /// <summary>
        /// Loads player stats and stores references for later use.
        /// </summary>
        public void LoadPlayerStats(PlayerSO playerSO, PlayerController playerController)
        {
            _playerController = playerController;
            _playerSO = playerSO;
        }

        /// <summary>
        /// Calculates the damage based on current level of the equipped tool.
        /// </summary>
        private float GetScaledDamage(ToolType tool)
        {
            var levelData = _playerController.PlayerProfile.GetLevelData(tool);
            return levelData != null ? levelData.CurrentLevel * _playerSO.LevelMultiplayer : 0f;
        }

        /// <summary>
        /// Determines action based on the currently equipped tool.
        /// </summary>
        private void UseEquippedTool()
        {
            switch (_equippedTool)
            {
                case ToolType.Axe:
                case ToolType.Pickaxe:
                    PerformMeleeAttack(GetScaledDamage(_equippedTool), 0, _equippedTool);
                    break;
                case ToolType.Slashed:
                    PerformMeleeAttack(GetScaledDamage(_equippedTool), _playerSO.SlashPushForce, _equippedTool);
                    break;
                case ToolType.ShieldBash:
                    PerformMeleeAttack(GetScaledDamage(_equippedTool), _playerSO.ShieldBashPushForce, _equippedTool);
                    break;
                case ToolType.Bow:
                case ToolType.PiercingArrow:
                    Shoot();
                    break;
                default:
                    PerformMeleeAttack(GetScaledDamage(_equippedTool), 0, _equippedTool);
                    break;
            }
        }

        /// <summary>
        /// Executes a melee attack and applies knockback + XP.
        /// </summary>
        private void PerformMeleeAttack(float damage, float pushForce, ToolType toolType)
        {
            var hits = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyLayer);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage, toolType);

                    if (hit.TryGetComponent(out Rigidbody2D rb))
                    {
                        Vector2 direction = (hit.transform.position - transform.position).normalized;
                        damageable.ApplyKnockback(direction, pushForce);
                    }

                    HandleActionXp();
                }
            }
        }

        /// <summary>
        /// Fires an arrow projectile (normal or piercing) based on the equipped tool.
        /// </summary>
        private void Shoot()
        {
            if (_isShooted)
            {
                _isShooted = false;
                return;
            }

            _isShooted = true;

            Vector2 shootDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            _playerMovement.SetDirection(shootDir.x > 0 ? 1f : -1f);
            _shootPoint.right = shootDir;

            var prefab = _equippedTool == ToolType.PiercingArrow ? _piercingArrowProjectilePrefab : _arrowProjectilePrefab;
            var arrow = Instantiate(prefab, _shootPoint.position, _shootPoint.rotation);
            arrow.Init(this,
                _equippedTool == ToolType.PiercingArrow ? _playerSO.PiercingArrow : _playerSO.ShootDamage,
                _equippedTool == ToolType.PiercingArrow ? _playerSO.PiercingArrowPushForce : 0);

            Physics2D.IgnoreCollision(arrow.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            arrow.Rigidbody2D.linearVelocity = _shootPoint.right * _shootForce;
        }

        /// <summary>
        /// Activates or deactivates the shield.
        /// </summary>
        private void ActiveShield(bool value)
        {
            _shield.SetActive(value);
            _isShieldActive = value;
        }

        /// <summary>
        /// Called when player switches action (tool/ability).
        /// </summary>
        public void SelectAction(int toolIndex)
        {
            _equippedTool = Enum.IsDefined(typeof(ToolType), toolIndex)
                ? (ToolType)toolIndex
                : ToolType.None;

            ShowHUD(false);
        }

        /// <summary>
        /// Grants XP based on the current action/tool and updates the profile.
        /// </summary>
        private bool HandleActionXp()
        {
            int xp = _playerController.XpDataSO.GetXpForTool(_equippedTool);
            bool success = _playerController.PlayerProfile.TryAddXp(_equippedTool, xp);

            if (!success)
                Debug.LogWarning($"Unknown XP target for ToolType: {_equippedTool}");

            return success;
        }

        /// <summary>
        /// Shows or hides the action HUD (used when changing abilities).
        /// </summary>
        private void ShowHUD(bool value)
        {
            _actionHUD.SetActive(value);
        }

        /// <summary>
        /// Gizmo to visualize melee attack range in the editor.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (_attackPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
        }
    }
}
