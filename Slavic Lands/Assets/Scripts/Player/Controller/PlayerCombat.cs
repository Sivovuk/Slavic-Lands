using System;
using System.Collections.Generic;
using Gameplay.Resources;
using Interfaces;
using Managers;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerCombat : MonoBehaviour, ILoadingStatsPlayer
    {
        [Header("Setup")]
        
        [SerializeField] private float _cutDamage;
        [SerializeField] private float _mineDamage;
        [SerializeField] private float _attackDamage;
        [SerializeField] private float _shootDamage;
        [SerializeField] private float _shootForce = 50;
        [SerializeField] private bool _isShooted = false;
        [SerializeField] private bool _isShieldActive = false;
        
        [SerializeField] private float _slashAbilityDamage;
        [SerializeField] private float _shieldAbilityDamage;
        [SerializeField] private float _piercingArrowAbilityDamage;
        
        [Header("References")]
        
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private float _attackRange;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private ArrowProjectile _arrowProjectilePrefab;
        [SerializeField] private ArrowProjectile _piercingArrowProjectilePrefab;
        [SerializeField] private GameObject _shield;
        [SerializeField] private LayerMask _enemyLayer;

        [Space(15)]
        
        [SerializeField] private GameObject _actionHUD;
        private PlayerController _playerController;
        private PlayerSO _playerSO;
        private PlayerMovement _playerMovement;
        private PlayerInputSystem _playerInputSystem;
        
        [SerializeField] private ToolType _equippedTool = ToolType.None;

        private void Awake()
        {
            _playerInputSystem = GetComponent<PlayerInputSystem>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnEnable()
        {
            _playerInputSystem.OnLMBClick += UseEquippedTool;
            _playerInputSystem.OnActionChanged += ShowHUD;
            _playerInputSystem.OnRMBClick += ActiveShield;
            _playerInputSystem.OnAbilitySelect += SelectAction;
            GameManager.Instance.OnPlayerInit += LoadAbilities;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnLMBClick -= UseEquippedTool;
            _playerInputSystem.OnRMBClick -= ActiveShield;
            _playerInputSystem.OnActionChanged -= ShowHUD;
            GameManager.Instance.OnPlayerInit -= LoadAbilities;

            // foreach (var VARIABLE in _player)
            // {
            //     
            // }
            // _player.PlayerProfile.CutLevelData.OnLevelChanged -= UpdatePlayerStats;
            // _player.PlayerProfile.MineLevelData.OnLevelChanged -= UpdatePlayerStats;
            // _player.PlayerProfile.AttackLevelData.OnLevelChanged -= UpdatePlayerStats;
            // _player.PlayerProfile.ShootLevelData.OnLevelChanged -= UpdatePlayerStats;
        }
        
        void UseEquippedTool()
        {
            switch (_equippedTool)
            {
                case ToolType.BattleAxe:
                    PerformMeleeAttack(30, _shootForce);
                    break;
                case ToolType.Bow:
                    //ShootArrow();
                    break;
                default:
                    PerformMeleeAttack(15, _shootForce); // Axe, Pickaxe default
                    break;
            }
        }
        
        void PerformMeleeAttack(int damage, float pushForce)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyLayer);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out IDamageable damageable))
                {
                    //damageable.TakeDamage(damage, HandleCollection());

                    if (hit.TryGetComponent(out Rigidbody2D rb))
                    {
                        Vector2 dir = (hit.transform.position - transform.position).normalized;
                        damageable.ApplyKnockback(dir, pushForce);
                        HandleActionXp();
                    }
                }
            }
        }
        
        // public void HandleHit(ToolType ToolType, IHit hitObject, bool applyForce, float pushForce = 0, Vector2  direction = new Vector2())
        // {
        //     hitObject.TakeDamage(GetActionDamage(ToolType), HandleCollection);
        //
        //     if (applyForce)
        //     {
        //         hitObject.ApplyForce(pushForce, direction);
        //     }
        //
        //     HandleActionXp(ToolType);
        // }

        public void SelectAction(int ToolTypeIndex)
        {
            if (ToolTypeIndex == (int)(ToolType.BattleAxe))
                _equippedTool = ToolType.BattleAxe;
            else if (ToolTypeIndex == (int)ToolType.Bow)
                _equippedTool = ToolType.Bow;
            else if (ToolTypeIndex == (int)ToolType.Axe)
                _equippedTool = ToolType.Axe;
            else if (ToolTypeIndex == (int)ToolType.Pickaxe)
                _equippedTool = ToolType.Pickaxe;
            else if (ToolTypeIndex == (int)ToolType.Slashed)
                _equippedTool = ToolType.Slashed;
            else if (ToolTypeIndex == (int)ToolType.ShieldBash)
                _equippedTool = ToolType.ShieldBash;
            else if (ToolTypeIndex == (int)ToolType.PiercingArrow)
                _equippedTool = ToolType.PiercingArrow;
            else
                Debug.LogWarning("Unknown action type! Action type : " + ToolTypeIndex);
            
            ShowHUD(false);
        }
        
        private bool HandleActionXp()
        {
            // if (_equippedTool == ToolType.Axe)
            // {
            //     _player.PlayerProfile.CutLevelData.AddXp(_player.XpDataSO.CuttingXP);
            //     return true;
            // }
            // else if (_equippedTool == ToolType.Pickaxe)
            // {
            //     _player.PlayerProfile.MineLevelData.AddXp(_player.XpDataSO.MiningXP);
            //     return true;
            //     
            // }
            // else if (_equippedTool == ToolType.BattleAxe)
            // {
            //     _player.PlayerProfile.AttackLevelData.AddXp(_player.XpDataSO.AttackXP);
            //     return true;
            // }
            // else if (_equippedTool == ToolType.Bow)
            // {
            //     _player.PlayerProfile.ShootLevelData.AddXp(_player.XpDataSO.ShootXP);
            //     return true;
            // }

            Debug.LogWarning("Unknown action type! Action type : " + _equippedTool);
            
            return false;
        }

        private void HandleCollection(List<ResourceData> resources, ResourceSO resourceData)
        {
            foreach (var resource in resources)
                _playerController.AddResource(resource.Amount, resource.ResourceType);
            
            _playerController.PlayerProfile.PlayerLevelData.AddXp(resourceData.XPReward);
        }

        private void Shoot()
        {
            if (_isShooted)
            {
                _isShooted = false;
                return;
            }

            _isShooted = true;
            
            Vector2 bowPosition = transform.position;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - bowPosition;
            _playerMovement.SetDirection(direction.x > 0 ? 1f : -1f);
            _shootPoint.right = direction;

            GameObject spawn;
            ArrowProjectile arrowProjectile;
            if (_equippedTool == ToolType.PiercingArrow)
            {
                spawn = Instantiate(_piercingArrowProjectilePrefab.gameObject, _shootPoint.position, _shootPoint.rotation);
                arrowProjectile = spawn.GetComponent<ArrowProjectile>();
                arrowProjectile.Init(this, _playerSO.PiercingArrowPushForce);
            }
            else
            {
                spawn = Instantiate(_arrowProjectilePrefab.gameObject, _shootPoint.position, _shootPoint.rotation);
                arrowProjectile = spawn.GetComponent<ArrowProjectile>();
                arrowProjectile.Init(this, 0);
            }

            _equippedTool = ToolType.Bow;
            Physics2D.IgnoreCollision(spawn.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            
            arrowProjectile.Rigidbody2D.linearVelocity = _shootPoint.right * _shootForce;
        }

        private void ActiveShield(bool value)
        {
            _shield.SetActive(value);
            _isShieldActive = value;
        }

        private void ShieldBash(bool value)
        {
            //_abilityShieldCollider.gameObject.SetActive(true);
            _playerMovement.Dash();
        }

        private void ShowHUD(bool value)
        {
            _actionHUD.SetActive(value);
        }

        public void LoadPlayerStats(PlayerSO playerSO, PlayerController playerController)
        {
            _playerController = playerController;
            _playerSO = playerSO;
            
            UpdatePlayerStats();
        }

        public void UpdatePlayerStats()
        {
            if (_playerController == null) return;
            if (_playerSO == null) return;
            
            // _cutDamage = _playerSO.CuttingDamage + (_player.PlayerProfile.CutLevelData.CurrentLevel * _playerSO.LevelMultiplayer);
            // _mineDamage = _playerSO.MiningDamage + (_player.PlayerProfile.MineLevelData.CurrentLevel * _playerSO.LevelMultiplayer);
            // _attackDamage = _playerSO.AttackDamage + (_player.PlayerProfile.AttackLevelData.CurrentLevel * _playerSO.LevelMultiplayer);
            // _shootDamage = _playerSO.ShootDamage + (_player.PlayerProfile.ShootLevelData.CurrentLevel * _playerSO.LevelMultiplayer);
            //
            // _player.PlayerProfile.CutLevelData.OnLevelChanged += UpdatePlayerStats;
            // _player.PlayerProfile.MineLevelData.OnLevelChanged += UpdatePlayerStats;
            // _player.PlayerProfile.AttackLevelData.OnLevelChanged += UpdatePlayerStats;
            // _player.PlayerProfile.ShootLevelData.OnLevelChanged += UpdatePlayerStats;
        }

        private void LoadAbilities()
        {
            // _slashAbilityDamage = _player.PlayerSO.Slash + (_player.PlayerProfile.AbilitySlashData.CurrentLevel * _player.PlayerSO.LevelMultiplayer);
            // _shieldAbilityDamage = _player.PlayerSO.ShieldBash + (_player.PlayerProfile.AbilityShieldBashData.CurrentLevel * _player.PlayerSO.LevelMultiplayer);
            // _piercingArrowAbilityDamage = _player.PlayerSO.PiercingArrow + (_player.PlayerProfile.AbilityPiercingArrowData.CurrentLevel * _player.PlayerSO.LevelMultiplayer);
            
            //_attackCollider.LoadActionHandler(ToolType.Attack, this, 0);
            //_abilitySlashCollider.LoadActionHandler(_player.PlayerProfile.AbilitySlashData.ToolType, this, _playerSO.SlashPushForce);
            //_abilityShieldCollider.LoadActionHandler(_player.PlayerProfile.AbilityShieldBashData.ToolType, this, _playerSO.ShieldBashPushForce);
        }
    }
}