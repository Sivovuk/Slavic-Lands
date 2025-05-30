using System;
using System.Collections.Generic;
using Gameplay.Resources;
using Interfaces;
using Managers;
using UnityEngine;

namespace Gameplay.Player
{

    public enum ActionType
    {
        Attack = 1,
        Shoot = 2,
        Cut = 3,
        Mine = 4,
        AbilitySlash = 5,
        AbilityShieldBash = 6,
        AbilityPiercedArrow = 7
    }

    public class PlayerAttack : MonoBehaviour, ILoadingStatsPlayer
    {
        [Header("Setup")]
        
        [SerializeField] private float _cutDamage;
        [SerializeField] private float _mineDamage;
        [SerializeField] private float _attackDamage;
        [SerializeField] private float _shootDamage;
        [SerializeField] private float _shootForce = 50;
        [SerializeField] private bool _isShooted = false;
        [SerializeField] private bool _isShieldActive = false;
        
        [SerializeField] private float _slashAbility;
        [SerializeField] private float _shieldAbility;
        [SerializeField] private float _piercingArrowAbility;
        
        [Header("References")]
        
        [SerializeField] private GameObject _attackCollider;
        [SerializeField] private GameObject _abilitySlashCollider;
        [SerializeField] private GameObject _abilityShieldCollider;
        
        [SerializeField] private GameObject _cutCollider;
        [SerializeField] private GameObject _mineCollider;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private GameObject _arrowPrefab;
        [SerializeField] private GameObject _piercingArrowPrefab;
        [SerializeField] private GameObject _shield;

        [Space(15)]
        
        [SerializeField] private GameObject _actionHUD;
        private Player _player;
        private PlayerSO _playerSO;
        private PlayerMovement _playerMovement;
        private PlayerInputSystem _playerInputSystem;
        
        [SerializeField] private ActionType _actionType;

        private void Awake()
        {
            _playerInputSystem = GetComponent<PlayerInputSystem>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnEnable()
        {
            _playerInputSystem.OnLMBClick += HandleAction;
            _playerInputSystem.OnActionChanged += ShowHUD;
            _playerInputSystem.OnRMBClick += ActiveShield;
            _playerInputSystem.OnAbilitySelect += SelectAction;
            GameManager.Instance.OnPlayerInit += LoadAbilities;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnLMBClick -= HandleAction;
            _playerInputSystem.OnRMBClick -= ActiveShield;
            _playerInputSystem.OnActionChanged -= ShowHUD;
            GameManager.Instance.OnPlayerInit -= LoadAbilities;
            
            _player.PlayerProfile.CutLevelData.OnLevelChanged -= UpdatePlayerStats;
            _player.PlayerProfile.MineLevelData.OnLevelChanged -= UpdatePlayerStats;
            _player.PlayerProfile.AttackLevelData.OnLevelChanged -= UpdatePlayerStats;
            _player.PlayerProfile.ShootLevelData.OnLevelChanged -= UpdatePlayerStats;
        }
        
        public void HandleHit(ActionType actionType, IHit hitObject)
        {
            hitObject.TakeDamage(GetActionDamage(actionType), HandleCollection);
        
            HandleActionXp(actionType);
        }

        private bool HandleActionXp(ActionType actionType)
        {
            if (actionType == ActionType.Cut)
            {
                _player.PlayerProfile.CutLevelData.AddXp(_player.XpDataSO.CuttingXP);
                return true;
            }
            else if (actionType == ActionType.Mine)
            {
                _player.PlayerProfile.MineLevelData.AddXp(_player.XpDataSO.MiningXP);
                return true;
                
            }
            else if (actionType == ActionType.Attack || actionType == ActionType.AbilitySlash || actionType == ActionType.AbilityShieldBash)
            {
                _player.PlayerProfile.AttackLevelData.AddXp(_player.XpDataSO.AttackXP);
                return true;
            }
            else if (actionType == ActionType.Shoot || actionType == ActionType.AbilityPiercedArrow)
            {
                _player.PlayerProfile.ShootLevelData.AddXp(_player.XpDataSO.ShootXP);
                return true;
            }

            Debug.LogWarning("Unknown action type! Action type : " + _actionType);
            
            return false;
        }

        private void HandleCollection(List<ResourceData> resources, ResourceSO resourceData)
        {
            foreach (var resource in resources)
                _player.AddResource(resource.Amount, resource.ResourceType);
            
            _player.PlayerProfile.PlayerLevelData.AddXp(resourceData.XPReward);
        }
        
        private void HandleAction(bool value)
        {
            if (_isShieldActive ) return;
            
            _attackCollider.SetActive(false);
            _cutCollider.SetActive(false);
            _mineCollider.SetActive(false);
            
            Debug.LogError(_actionType);
            Debug.LogError((int)_actionType);
            Debug.LogError((int)ActionType.AbilitySlash);
            
            if (_actionType == ActionType.Attack)
                _attackCollider.SetActive(value);
            else if (_actionType == ActionType.Cut)
                _cutCollider.SetActive(value);
            else if (_actionType == ActionType.Mine)
                _mineCollider.SetActive(value);
            else if (_actionType == ActionType.Shoot)
                Shoot();
            else if (_actionType == ActionType.AbilitySlash)
            {
                _abilitySlashCollider.SetActive(true);
                _actionType = ActionType.Attack;
            }
            else if (_actionType == ActionType.AbilityShieldBash)
            {
                _abilityShieldCollider.SetActive(true);
                _actionType = ActionType.Attack;
            }
            else if(_actionType == ActionType.AbilityPiercedArrow)
                Shoot();
            else
                Debug.LogWarning("Action is not defined!");
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
            if (_actionType ==  ActionType.AbilityPiercedArrow)
                spawn = Instantiate(_piercingArrowPrefab, _shootPoint.position, _shootPoint.rotation);
            else
                spawn = Instantiate(_arrowPrefab, _shootPoint.position, _shootPoint.rotation);

            _actionType = ActionType.Shoot;
            Physics2D.IgnoreCollision(spawn.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            Arrow arrow = spawn.GetComponent<Arrow>();
            
            arrow.Init(this);
            arrow.Rigidbody2D.linearVelocity = _shootPoint.right * _shootForce;
        }

        private float GetActionDamage(ActionType actionType)
        {
            if (actionType == ActionType.Attack)
                return _attackDamage;
            else if (actionType == ActionType.Shoot)
                return _shootDamage;
            else if (actionType == ActionType.Cut)
                return _cutDamage;
            else if (actionType == ActionType.Mine)
                return _mineDamage;
            else if(_actionType == ActionType.AbilitySlash)
                return _slashAbility;
            else if(_actionType == ActionType.AbilitySlash)
                return _shieldAbility;
            else if(_actionType == ActionType.AbilitySlash)
                return _piercingArrowAbility;
            else
            {
                Debug.LogWarning("Unknown action type! Action type : " + actionType);
                return 0;
            }
        }

        public void SelectAction(int actionTypeIndex)
        {
            if (actionTypeIndex == (int)ActionType.Attack)
                _actionType = ActionType.Attack;
            else if (actionTypeIndex == (int)ActionType.Shoot)
                _actionType = ActionType.Shoot;
            else if (actionTypeIndex == (int)ActionType.Cut)
                _actionType = ActionType.Cut;
            else if (actionTypeIndex == (int)ActionType.Mine)
                _actionType = ActionType.Mine;
            else if (actionTypeIndex == (int)ActionType.AbilitySlash)
                _actionType = ActionType.AbilitySlash;
            else if (actionTypeIndex == (int)ActionType.AbilityShieldBash)
                _actionType = ActionType.AbilityShieldBash;
            else if (actionTypeIndex == (int)ActionType.AbilityPiercedArrow)
                _actionType = ActionType.AbilityPiercedArrow;
            else
                Debug.LogWarning("Unknown action type! Action type : " + actionTypeIndex);
            
            ShowHUD(false);
        }

        private void ActiveShield(bool value)
        {
            _shield.SetActive(value);
            _isShieldActive = value;
        }

        private void ShowHUD(bool value)
        {
            _actionHUD.SetActive(value);
        }

        public void LoadPlayerStats(PlayerSO playerSO, Player player)
        {
            _player = player;
            _playerSO = playerSO;
            
            UpdatePlayerStats();
        }

        public void UpdatePlayerStats()
        {
            if (_player == null) return;
            if (_playerSO == null) return;
            
            _cutDamage = _playerSO.CuttingDamage + (_player.PlayerProfile.CutLevelData.CurrentLevel * _playerSO.LevelMultiplayer);
            _mineDamage = _playerSO.MiningDamage + (_player.PlayerProfile.MineLevelData.CurrentLevel * _playerSO.LevelMultiplayer);
            _attackDamage = _playerSO.AttackDamage + (_player.PlayerProfile.AttackLevelData.CurrentLevel * _playerSO.LevelMultiplayer);
            _shootDamage = _playerSO.ShootDamage + (_player.PlayerProfile.ShootLevelData.CurrentLevel * _playerSO.LevelMultiplayer);

            _player.PlayerProfile.CutLevelData.OnLevelChanged += UpdatePlayerStats;
            _player.PlayerProfile.MineLevelData.OnLevelChanged += UpdatePlayerStats;
            _player.PlayerProfile.AttackLevelData.OnLevelChanged += UpdatePlayerStats;
            _player.PlayerProfile.ShootLevelData.OnLevelChanged += UpdatePlayerStats;
        }

        private void LoadAbilities()
        {
            _slashAbility = _player.PlayerSO.Slash + (_player.PlayerProfile.AbilitySlashData.CurrentLevel * _player.PlayerSO.LevelMultiplayer);
            _shieldAbility = _player.PlayerSO.ShieldBash + (_player.PlayerProfile.AbilityShieldBashData.CurrentLevel * _player.PlayerSO.LevelMultiplayer);
            _piercingArrowAbility = _player.PlayerSO.PiercingArrow + (_player.PlayerProfile.AbilityPiercingArrowData.CurrentLevel * _player.PlayerSO.LevelMultiplayer);
        }
    }
}