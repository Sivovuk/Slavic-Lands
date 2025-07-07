using System;
using Gameplay.Resources;
using UnityEngine;

namespace Gameplay.NPC
{
    public class NPC : Resource
    {
        private NPCMovement _npcMovement;
        private NPCAttack _npcAttack;

        private void Awake()
        {
            _npcMovement = GetComponent<NPCMovement>();
            _npcAttack = GetComponent<NPCAttack>();
        }

        public override void ApplyKnockback(Vector2 direction, float force)
        {
            if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rg))
            {
                _npcMovement.IsStunned = true;
                Invoke("Unstunned", 1f);
                rg.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }
        
        private void Unstunned()
        {
            _npcMovement.IsStunned = false;
        }
    }
}