using System;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    public class PlayerInputSystem : MonoBehaviour, Controls.IPlayerActions
    {
        [field: SerializeField] public Vector2 MovementValue { get; private set; }
        [field: SerializeField] public Vector2 Look { get; private set; }

        public event Action OnLmbClick;
        public event  Action OnLmbRelease;
        public event  Action<bool> OnRmbClick;
        public event  Action OnInteractionClick;
        public event  Action OnJumpClick;
        public event  Action<bool> OnActionChanged;
        public event  Action<bool> OnSprintClick;
        public event  Action<bool> OnTabClicked;
        public event  Action OnDashClicked;
        public event  Action<int> OnAbilitySelect;

        private Controls _controls;

        private void Awake()
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
            _controls.Enable();
        }

        private void OnDestroy()
        {
            _controls.Player.Disable();
        }


        public void OnMove(InputAction.CallbackContext context)
        {
            MovementValue = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }

        public void OnLMB(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnLmbClick?.Invoke();
            else if (context.canceled)
                OnLmbRelease?.Invoke();
        }

        public void OnRMB(InputAction.CallbackContext context)
        {
            OnRmbClick?.Invoke(context.performed);
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            OnSprintClick?.Invoke(context.started || context.performed);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var keyboard = Keyboard.current;
                if (keyboard != null && keyboard.leftShiftKey.isPressed &&
                    (keyboard.dKey.isPressed || keyboard.aKey.isPressed))
                    return;

                OnJumpClick?.Invoke();
            }
        }

        public void OnSelectAction(InputAction.CallbackContext context)
        {
            OnActionChanged?.Invoke(context.performed);
        }

        public void OnPlayerUI(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnTabClicked?.Invoke(true);
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnDashClicked?.Invoke();
        }

        public void OnAbility1(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAbilitySelect?.Invoke((int)ToolType.Slashed);
        }

        public void OnAbility2(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAbilitySelect?.Invoke((int)ToolType.ShieldBash);
        }

        public void OnAbility3(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAbilitySelect?.Invoke((int)ToolType.PiercingArrow);
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnInteractionClick?.Invoke();
        }
    }
}