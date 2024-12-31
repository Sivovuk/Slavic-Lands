using System;
using Gameplay.Resources;
using Interfaces;
using UnityEngine;

namespace Gameplay.Player
{

    public enum ActionType
    {
        Attack,
        Shoot,
        Cut,
        Mine
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
        
        private Player _player;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _cutCollider.SetActive(true);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _cutCollider.SetActive(false);
            }
        }


        public void HandleHit(ActionType actionType, IHit hitObject)
        {
            hitObject.TakeDamage(GetActionDamage(actionType), HandleCollection);
        }

        private void HandleCollection(ResourceType resourceType, int amount)
        {
            _player.AddResource(amount, resourceType);
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