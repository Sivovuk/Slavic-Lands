using System;
using Interfaces;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerActionHandler : MonoBehaviour
    {
        [SerializeField] private ActionType _actionType;
        [SerializeField] private PlayerAttack _playerAttack;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IHit>(out IHit hit))
            {
                //Debug.LogError(other.name + " is hit");
                _playerAttack.HandleHit(_actionType, hit);
            }
        }
    }
}