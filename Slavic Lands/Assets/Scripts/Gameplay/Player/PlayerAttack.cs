using System;
using System.Collections.Generic;
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
            foreach (var resource in hitObject.GetResourceType())
            {
                if (HandleAttackType(actionType, resource.ResourceType))
                {
                    hitObject.TakeDamage(GetActionDamage(actionType), HandleCollection);
                }
            }
        }

        private bool HandleAttackType(ActionType actionType, ResourceType resourceType)
        {
            if (actionType == ActionType.Cut)
            {
                if (resourceType == ResourceType.Wood)
                {
                    _player.PlayerProfile.CutLevelData.AddXp(_player.XpDataSO.CuttingXP);
                    return true;
                }
            }
            else if (actionType == ActionType.Mine)
            {
                if (resourceType == ResourceType.Stone)
                {
                    _player.PlayerProfile.MineLevelData.AddXp(_player.XpDataSO.MiningXP);
                    return true;
                }
            }
            else if (actionType == ActionType.Attack)
            {
                if (resourceType == ResourceType.Hide)
                {
                    _player.PlayerProfile.AttackLevelData.AddXp(_player.XpDataSO.AttackXP);
                    return true;
                }
            }
            
            return false;
        }

        public void HandleTargetHit(ActionType actionType, IHit hitObject)
        {
            _player.PlayerProfile.ShootLevelData.AddXp(_player.XpDataSO.ShootXP);
        }

        private void HandleCollection(List<ResourceData> resources, ResourceSO resourceData)
        {
            foreach (var resource in resources)
            {
                _player.AddResource(resource.Amount, resource.ResourceType);
            }
            
            _player.PlayerProfile.PlayerLevelData.AddXp(resourceData.XPReward);
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