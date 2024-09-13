using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CatInput
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/InputReader", order = 1)]
    public class InputReader : ScriptableObject, GameInput.IGameplayActions
    {
        private GameInput _gameInput;

        void OnEnable() 
        {
            if(_gameInput == null)
            {
                _gameInput = new GameInput();

                _gameInput.Gameplay.SetCallbacks(this);
            }

            EnableAllInput();
        }

        void OnDisable() 
        {
            DisableAllInput();
        }

        private void DisableAllInput()
        {
            _gameInput.Gameplay.Disable();
        }

        public void EnableAllInput()
        {
            _gameInput.Gameplay.Enable();
        }

        public event Action<Vector2> MoveEvent;
        public event Action JumpEvent;
        public event Action FireEvent;


        public void OnFire(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                FireEvent?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                JumpEvent?.Invoke();
            }

            // if(context.phase == InputActionPhase.Canceled)
            // {
            //     Debug.Log($"Jump Canceled");
            // }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            
        }
    }

}
