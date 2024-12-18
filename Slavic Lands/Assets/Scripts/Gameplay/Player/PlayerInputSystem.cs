using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    public class PlayerInputSystem : MonoBehaviour, Controls.IPlayerActions
    {
        [field:SerializeField] public Vector2 MovementValue { get; private set; }
    	[field:SerializeField] public Vector2 Look { get; private set; }

        public UnityEvent OnLMBClick;
        public UnityEvent OnRMBClick;
        public UnityEvent OnJumpClick;
        public UnityEvent<bool> OnSprintClick;

        private Controls _controls;
    
        private void Start()
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
            _controls.Enable();
        
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
                OnLMBClick?.Invoke();
            }
        }

        public void OnRMB(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnRMBClick?.Invoke();
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
            if (context.performed)
            {
                OnJumpClick?.Invoke();
            }
        }
    }
}