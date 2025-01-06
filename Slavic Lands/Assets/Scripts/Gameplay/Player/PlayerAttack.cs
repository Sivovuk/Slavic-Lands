using System;
using Gameplay.Resources;
using Interfaces;
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

        [SerializeField] private GameObject _attackCollider; 
        [SerializeField] private GameObject _cutCollider;
        [SerializeField] private GameObject _mineCollider;

        [SerializeField] private GameObject _actionHUD;
        private Player _player;
        private PlayerInputSystem _playerInputSystem;
        
        [SerializeField] private ActionType _actionType;

        private void Awake()
        {
            _playerInputSystem = GetComponent<PlayerInputSystem>();
        }

        private void OnEnable()
        {
            _playerInputSystem.OnLMBClick += HandleAction;
            _playerInputSystem.OnActionChanged += ShowHUD;
        }

        private void OnDisable()
        {
            _playerInputSystem.OnLMBClick -= HandleAction;
            _playerInputSystem.OnActionChanged -= ShowHUD;
        }
        
        public void HandleHit(ActionType actionType, IHit hitObject)
        {
            if (HandleAttackType(actionType, hitObject.GetResourceType()))
            {
                hitObject.TakeDamage(GetActionDamage(actionType), HandleCollection);
            }
        }

        private bool HandleAttackType(ActionType actionType, ResourceType resourceType)
        {
            if (actionType == ActionType.Cut)
            {
                if (resourceType == ResourceType.Wood)
                {
                    return true;
                }
            }
            else if (actionType == ActionType.Mine)
            {
                if (resourceType == ResourceType.Stone)
                {
                    return true;
                }
            }
            else if (actionType == ActionType.Attack)
            {
                if (resourceType == ResourceType.Hide)
                {
                    return true;
                }
            }
            
            return false;
        }

        private void HandleCollection(ResourceType resourceType, int amount)
        {
            _player.AddResource(amount, resourceType);
        }
        
        private void HandleAction(bool value)
        {
            _attackCollider.SetActive(false);
            _cutCollider.SetActive(false);
            _mineCollider.SetActive(false);
            
            if (_actionType == ActionType.Attack)
            {
                _attackCollider.SetActive(value);
            }
            else if (_actionType == ActionType.Cut)
            {
                _cutCollider.SetActive(value);
            }
            else if (_actionType == ActionType.Mine)
            {
                _mineCollider.SetActive(value);
            }
            
        }

        private float GetActionDamage(ActionType actionType)
        {
            if (actionType == ActionType.Attack)
            {
                return _attackDamage;
            }
            else if (actionType == ActionType.Shoot)
            {
                return _shootDamage;
            }
            else if (actionType == ActionType.Cut)
            {
                return _cutDamage;
            }
            else if (actionType == ActionType.Mine)
            {
                return _mineDamage;
            }
            else
            {
                Debug.LogError("Unknown action type! Action type : " + actionType);
                return 0;
            }
        }

        public void SelectAction(int actionTypeIndex)
        {
            if (actionTypeIndex == (int)ActionType.Attack)
            {
                _actionType = ActionType.Attack;
            }
            else if (actionTypeIndex == (int)ActionType.Shoot)
            {
                _actionType = ActionType.Shoot;
            }
            else if (actionTypeIndex == (int)ActionType.Cut)
            {
                _actionType = ActionType.Cut;
            }
            else if (actionTypeIndex == (int)ActionType.Mine)
            {
                _actionType = ActionType.Mine;
            }
            
            ShowHUD(false);
        }
        
        
        private void ShowHUD(bool value)
        {
            _actionHUD.SetActive(value);
        }


        public void LoadPlayerStats(PlayerSO playerSO, Player player)
        {
            _player = player;
            
            _cutDamage = playerSO.CutingDamage + (_player.CurrentLevel * playerSO.LevelMultiplayer);
            _mineDamage = playerSO.MiningDamage + (_player.CurrentLevel * playerSO.LevelMultiplayer);
            _attackDamage = playerSO.AttackDamage + (_player.CurrentLevel * playerSO.LevelMultiplayer);
            _shootDamage = playerSO.ShootDamage + (_player.CurrentLevel * playerSO.LevelMultiplayer);
        }
    }
}