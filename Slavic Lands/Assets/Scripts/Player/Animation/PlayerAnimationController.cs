using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Centralized point for triggering animations, removing Animator dependencies from other scripts.
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {
        private static readonly int MovementParam = Animator.StringToHash("Movement");
        private static readonly int CutTrigger = Animator.StringToHash("Cut");
        private static readonly int MineTrigger = Animator.StringToHash("Mine");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int ShieldTrigger = Animator.StringToHash("Shield");
        private static readonly int DeathTrigger = Animator.StringToHash("Death");

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Updates the movement blend tree parameter.
        /// </summary>
        public void UpdateMovement(float blend)
        {
            _animator.SetFloat(MovementParam, blend, 0.1f, Time.deltaTime);
        }

        public void TriggerCut() => _animator.SetTrigger(CutTrigger);
        public void TriggerMine() => _animator.SetTrigger(MineTrigger);
        public void TriggerAttack() => _animator.SetTrigger(AttackTrigger);
        public void TriggerShield() => _animator.SetTrigger(ShieldTrigger);
        public void TriggerDeath() => _animator.SetTrigger(DeathTrigger);
    }
}
