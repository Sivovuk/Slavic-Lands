using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    public class PlayerInputSystem : MonoBehaviour, Controls.IPlayerActions
    {
        [field: SerializeField] public Vector2 MovementValue { get; private set; }
        [field: SerializeField] public Vector2 Look { get; private set; }

        public Action<bool> OnLMBClick;
        public Action<bool> OnLMBRelease;
        public Action<bool> OnRMBClick;
        public Action OnInteractionClick;
        public Action OnJumpClick;
        public Action<bool> OnActionChanged;
        public Action<bool> OnSprintClick;
        public Action<bool> OnTabClicked;
        public Action OnDashClicked;
        public Action<int> OnAbilitySelect;

        private Controls _controls;

        private void Start()
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
            {
                OnLMBClick?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnLMBRelease?.Invoke(false);
            }
        }

        public void OnRMB(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnRMBClick?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnRMBClick?.Invoke(false);
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnSprintClick?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnSprintClick?.Invoke(false);
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            var shift = Keyboard.current.leftShiftKey.isPressed;
            var moveKey = Keyboard.current.dKey.isPressed || Keyboard.current.aKey.isPressed;
            
            if ( shift && moveKey) return;

            if (context.performed)
                OnJumpClick?.Invoke();
        }

        public void OnSelectAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnActionChanged?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnActionChanged?.Invoke(false);
            }
        }

        public void OnPlayerUI(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnTabClicked?.Invoke(true);
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnDashClicked?.Invoke();
        }

        public void OnAbility1(InputAction.CallbackContext context)
        {
            OnAbilitySelect?.Invoke((int)ActionType.AbilitySlash);
        }

        public void OnAbility2(InputAction.CallbackContext context)
        {
            OnAbilitySelect?.Invoke((int)ActionType.AbilityShieldBash);
        }

        public void OnAbility3(InputAction.CallbackContext context)
        {
            OnAbilitySelect?.Invoke((int)ActionType.AbilityPiercedArrow);
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            OnInteractionClick?.Invoke();
        }
    }
}