using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Attach this to states in the Animator (e.g. Cut, Mine, Attack) to prevent the player from moving
    /// while the animation is playing.
    /// </summary>
    public class LockMovementSMB : StateMachineBehaviour
    {
        private PlayerController _playerController;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_playerController == null)
                _playerController = animator.GetComponent<PlayerController>();

            if (_playerController != null && _playerController.PlayerMovement != null)
            {
                _playerController.PlayerMovement.SetMovementLock(true);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_playerController != null && _playerController.PlayerMovement != null)
            {
                _playerController.PlayerMovement.SetMovementLock(false);
            }
        }
    }
}
