using System;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerActionHandler : MonoBehaviour
    {
        // [SerializeField] private ActionType _actionType;
        // [SerializeField] private PlayerCombat _playerCombat;
        // [SerializeField] private float _pushForce;
        //
        // public void LoadActionHandler(ActionType actionType, PlayerCombat playerCombat, float pushForce)
        // {
        //     _actionType = actionType;
        //     _playerCombat = playerCombat;
        //     _pushForce = pushForce;
        // }
        //
        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (other.TryGetComponent<IHit>(out IHit hit))
        //     {
        //         Vector2 direction = other.transform.position - _playerCombat.transform.position;
        //         direction.x = direction.x > 0 ? 1 : -1;
        //         direction.y = direction.y > 0 ? 1 : -1;
        //         _playerCombat.HandleHit(_actionType, hit, true, _pushForce, direction);
        //     }
        // }
    }
}