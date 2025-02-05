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
        Mine = 4
    }

    public class PlayerAttack : MonoBehaviour, ILoadingStatsPlayer
    {
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
        
        [Space(10)]
        
        [SerializeField] private GameObject _attackCollider; 
        [SerializeField] private GameObject _cutCollider;
        [SerializeField] private GameObject _mineCollider;
        [SerializeField] private Transform _shootPoint;
        [SerializeField] private GameObject _arrow;
        [SerializeField] private GameObject _shield;

        [Space(10)]
        
        [SerializeField] private GameObject _actionHUD;
        private Player _player;
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
            GameManager.Instance.OnPlayerInit += LoadAbilities;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnLMBClick -= HandleAction;
            _playerInputSystem.OnRMBClick -= ActiveShield;
            _playerInputSystem.OnActionChanged -= ShowHUD;
            GameManager.Instance.OnPlayerInit -= LoadAbilities;
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
            else if (actionType == ActionType.Attack)
            {
                _player.PlayerProfile.AttackLevelData.AddXp(_player.XpDataSO.AttackXP);
                return true;
            }
            else if (actionType == ActionType.Shoot)
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
            
            if (_actionType == ActionType.Attack)
                _attackCollider.SetActive(value);
            else if (_actionType == ActionType.Cut)
                _cutCollider.SetActive(value);
            else if (_actionType == ActionType.Mine)
                _mineCollider.SetActive(value);
            else if (_actionType == ActionType.Shoot)
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

            GameObject spawn = Instantiate(_arrow, _shootPoint.position, _shootPoint.rotation);
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
            
            _cutDamage = playerSO.CuttingDamage + (_player.CurrentLevel * playerSO.LevelMultiplayer);
            _mineDamage = playerSO.MiningDamage + (_player.CurrentLevel * playerSO.LevelMultiplayer);
            _attackDamage = playerSO.AttackDamage + (_player.CurrentLevel * playerSO.LevelMultiplayer);
            _shootDamage = playerSO.ShootDamage + (_player.CurrentLevel * playerSO.LevelMultiplayer);
        }

        private void LoadAbilities()
        {
            _slashAbility = _player._playerSO.Slash + (_player.PlayerProfile.AbilitySlashData.CurrentLevel * _player._playerSO.LevelMultiplayer);
            _shieldAbility = _player._playerSO.ShieldBash + (_player.PlayerProfile.AbilityShieldBashData.CurrentLevel * _player._playerSO.LevelMultiplayer);
            _piercingArrowAbility = _player._playerSO.PiercingArrow + (_player.PlayerProfile.AbilityPiercingArrowData.CurrentLevel * _player._playerSO.LevelMultiplayer);
        }
    }
}