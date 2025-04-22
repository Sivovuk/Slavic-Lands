using System;
using UnityEngine;

namespace Gameplay.NPC
{
    public class NPCAttackCollider : MonoBehaviour
    {
        private NPCAttack _attack;

        private void Start()
        {
            _attack = GetComponentInParent<NPCAttack>();
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GetComponentInParent<Collider2D>());
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _attack.CheckAttackCollider(other, true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _attack.CheckAttackCollider(other, false);
        }
    }
}