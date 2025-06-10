using System;
using Interfaces;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerActionHandler : MonoBehaviour
    {
        [SerializeField] private ActionType _actionType;
        [SerializeField] private PlayerAttack _playerAttack;
        [SerializeField] private float _pushForce;

        public void LoadActionHandler(ActionType actionType, PlayerAttack playerAttack, float pushForce)
        {
            _actionType = actionType;
            _playerAttack = playerAttack;
            _pushForce = pushForce;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IHit>(out IHit hit))
            {
                Vector2 direction = other.transform.position - _playerAttack.transform.position;
                direction.x = direction.x > 0 ? 1 : -1;
                direction.y = direction.y > 0 ? 1 : -1;
                _playerAttack.HandleHit(_actionType, hit, true, _pushForce, direction);
            }
        }
    }
}