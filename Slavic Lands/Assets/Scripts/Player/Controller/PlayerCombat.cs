using System;
using System.Collections.Generic;
using Core;
using Core.Interfaces;
using Data;
using Managers;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerCombat : MonoBehaviour, ILoadingStatsPlayer
    {
        [Header("Base Damage")]
        private float _cutDamage;
        private float _mineDamage;
        private float _attackDamage;
        private float _shootDamage;

        [Header("Ability Damage")]
        private float _slashAbilityDamage;
        private float _shieldAbilityDamage;
        private float _piercingArrowAbilityDamage;

        [Header("Shooting")]
        [SerializeField] private float _shootForce = 50f;
        private bool _isShooted;

        [Header("Shield")]
        private bool _isShieldActive;

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
            GameManager.Instance.OnPlayerInit += LoadAbilities;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnLmbClick -= UseEquippedTool;
            _playerInputSystem.OnRmbClick -= ActiveShield;
            _playerInputSystem.OnActionChanged -= ShowHUD;
            _playerInputSystem.OnAbilitySelect -= SelectAction;
            GameManager.Instance.OnPlayerInit -= LoadAbilities;
        }

        public void LoadPlayerStats(PlayerSO playerSO, PlayerController playerController)
        {
            _playerController = playerController;
            _playerSO = playerSO;
            UpdatePlayerStats();
        }

        public void UpdatePlayerStats()
        {
            if (_playerController == null || _playerSO == null) return;

            _cutDamage = _playerSO.CuttingDamage + GetScaledDamage(ToolType.Axe);
            _mineDamage = _playerSO.MiningDamage + GetScaledDamage(ToolType.Pickaxe);
            _attackDamage = _playerSO.AttackDamage + GetScaledDamage(ToolType.BattleAxe);
            _shootDamage = _playerSO.ShootDamage + GetScaledDamage(ToolType.Bow);
        }

        private float GetScaledDamage(ToolType tool)
        {
            var levelData = _playerController.PlayerProfile.GetLevelData(tool);
            return levelData != null ? levelData.CurrentLevel * _playerSO.LevelMultiplayer : 0f;
        }

        private void LoadAbilities()
        {
            _slashAbilityDamage = _playerSO.Slash + GetScaledDamage(ToolType.Slashed);
            _shieldAbilityDamage = _playerSO.ShieldBash + GetScaledDamage(ToolType.ShieldBash);
            _piercingArrowAbilityDamage = _playerSO.PiercingArrow + GetScaledDamage(ToolType.PiercingArrow);
        }

        private void UseEquippedTool()
        {
            switch (_equippedTool)
            {
                case ToolType.Axe:
                    PerformMeleeAttack((int)_cutDamage, 0);
                    break;
                case ToolType.Pickaxe:
                    PerformMeleeAttack((int)_mineDamage, 0);
                    break;
                case ToolType.Slashed:
                    PerformMeleeAttack((int)_playerSO.Slash, _playerSO.SlashPushForce);
                    break;
                case ToolType.ShieldBash:
                    PerformMeleeAttack((int)_playerSO.ShieldBash, _playerSO.ShieldBashPushForce);
                    break;
                case ToolType.Bow:
                case ToolType.PiercingArrow:
                    Shoot();
                    break;
                default:
                    PerformMeleeAttack((int)_playerSO.AttackDamage, 0);
                    break;
            }
        }

        private void PerformMeleeAttack(int damage, float pushForce)
        {
            var hits = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyLayer);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out IDamageable damageable))
                {
                    if (hit.TryGetComponent(out Rigidbody2D rb))
                    {
                        Vector2 direction = (hit.transform.position - transform.position).normalized;
                        damageable.ApplyKnockback(direction, pushForce);
                    }
                    
                    damageable.TakeDamage(damage);

                    HandleActionXp();
                }
            }
        }

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
            arrow.Init(this, _equippedTool == ToolType.PiercingArrow ? _playerSO.PiercingArrow : _playerSO.ShootDamage, _equippedTool == ToolType.PiercingArrow ? _playerSO.PiercingArrowPushForce : 0);
            Physics2D.IgnoreCollision(arrow.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            arrow.Rigidbody2D.linearVelocity = _shootPoint.right * _shootForce;
        }

        private void ActiveShield(bool value)
        {
            _shield.SetActive(value);
            _isShieldActive = value;
        }

        public void SelectAction(int toolIndex)
        {
            _equippedTool = Enum.IsDefined(typeof(ToolType), toolIndex)
                ? (ToolType)toolIndex
                : ToolType.None;

            ShowHUD(false);
        }

        private bool HandleActionXp()
        {
            int xp = _playerController.XpDataSO.GetXpForTool(_equippedTool);
            bool success = _playerController.PlayerProfile.TryAddXp(_equippedTool, xp);

            if (!success)
                Debug.LogWarning($"Unknown XP target for ToolType: {_equippedTool}");

            return success;
        }

        private void ShowHUD(bool value)
        {
            _actionHUD.SetActive(value);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (_attackPoint == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
        }
    }
}
