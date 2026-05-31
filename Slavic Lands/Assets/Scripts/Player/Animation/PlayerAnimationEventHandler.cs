using System;
using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Receives Animation Events from the Unity Animator and forwards them via C# events.
    /// This should be attached to the same GameObject as the Animator component.
    /// </summary>
    public class PlayerAnimationEventHandler : MonoBehaviour
    {
        public event Action OnMeleeHit;

        /// <summary>
        /// Called via Unity Animation Event on the exact frame the weapon swing hits.
        /// </summary>
        public void ExecuteMeleeHit()
        {
            OnMeleeHit?.Invoke();
        }
    }
}
