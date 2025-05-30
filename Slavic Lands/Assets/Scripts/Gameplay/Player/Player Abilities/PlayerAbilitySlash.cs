using Interfaces;
using UnityEngine;

namespace Gameplay.Player.Player_Abilities
{
    public class PlayerAbilitySlash : MonoBehaviour
    {
        [SerializeField] private ActionType _actionType;
        [SerializeField] private PlayerAttack _playerAttack;
        [SerializeField] private float _pushForce;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IHit>(out IHit hit))
            {
                _playerAttack.HandleHit(_actionType, hit);
                Vector2 direction = (other.transform.position - transform.position).normalized;
                other.TryGetComponent<Rigidbody2D>(out Rigidbody2D rg);
                rg.AddForce(direction *  _pushForce, ForceMode2D.Force);
            }
        }
    }
}