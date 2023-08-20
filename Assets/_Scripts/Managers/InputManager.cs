using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public event EventHandler OnLeftAction;
        public event EventHandler OnInteractActionReleased;
        public event EventHandler OnRightAction;
        public event EventHandler OnInteractAlternateActionReleased;
        public event EventHandler OnJumpAction;
        public event EventHandler OnJumpActionReleased;
        public event EventHandler OnLeaveAction;
        public event EventHandler OnCrouchActionReleased;
        public event EventHandler OnMenuAction;

        private const string PlayerPrefsBindings = "InputBindings";
        private PlayerControls _playerControls;
        private bool _wasLeftPressed;
        private bool _wasRightPressed;
        private bool _wasJumpPressed;
        private bool _wasLeavePressed;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one InputManager! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _playerControls = new PlayerControls();

            if (PlayerPrefs.HasKey(PlayerPrefsBindings))
                _playerControls.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PlayerPrefsBindings));
        }

        private void Start()
        {
            _playerControls.Enable();
            _playerControls.Player.LeftArm.performed += OnLeftPerformed;
            _playerControls.Player.RightArm.performed += OnRightPerformed;
            _playerControls.Player.Jump.performed += OnJumpPerformed;
            _playerControls.Player.LeaveMech.performed += OnLeavePerformed;
            _playerControls.Player.Menu.performed += OnMenuActionPerformed;
        }

        private void OnDestroy()
        {
            _playerControls.Disable();
            _playerControls.Player.LeftArm.performed -= OnLeftPerformed;
            _playerControls.Player.RightArm.performed -= OnRightPerformed;
            _playerControls.Player.Jump.performed -= OnJumpPerformed;
            _playerControls.Player.LeaveMech.performed -= OnLeavePerformed;
            _playerControls.Player.Menu.performed -= OnMenuActionPerformed;
            _playerControls.Dispose();
        }

        private void Update()
        {
            if (_wasLeftPressed && _playerControls.Player.LeftArm.WasReleasedThisFrame())
            {
                OnInteractActionReleased?.Invoke(this, EventArgs.Empty);
                _wasLeftPressed = false;
            }
            
            if (_wasRightPressed && _playerControls.Player.RightArm.WasReleasedThisFrame())
            {
                OnInteractAlternateActionReleased?.Invoke(this, EventArgs.Empty);
                _wasRightPressed = false;
            }
            
            if (_wasJumpPressed && _playerControls.Player.Jump.WasReleasedThisFrame())
            {
                OnJumpActionReleased?.Invoke(this, EventArgs.Empty);
                _wasJumpPressed = false;
            }
            
            if (_wasLeavePressed && _playerControls.Player.LeaveMech.WasReleasedThisFrame())
            {
                OnCrouchActionReleased?.Invoke(this, EventArgs.Empty);
                _wasLeavePressed = false;
            }
        }

        public Vector2 GetPlayerMovement() => _playerControls.Player.Movement.ReadValue<Vector2>();

        public Vector2 GetLookDelta() => _playerControls.Player.Look.ReadValue<Vector2>();

        private void OnLeftPerformed(InputAction.CallbackContext obj)
        {
            OnLeftAction?.Invoke(this, EventArgs.Empty);
            _wasLeftPressed = true;
        }

        private void OnRightPerformed(InputAction.CallbackContext obj)
        {
            OnRightAction?.Invoke(this, EventArgs.Empty);
            _wasRightPressed = true;
        } 
        
        private void OnJumpPerformed(InputAction.CallbackContext obj)
        {
            OnJumpAction?.Invoke(this, EventArgs.Empty);
            _wasJumpPressed = true;
        }
        
        private void OnLeavePerformed(InputAction.CallbackContext obj)
        {
            OnLeaveAction?.Invoke(this, EventArgs.Empty);
            _wasLeavePressed = true;
        }

        private void OnMenuActionPerformed(InputAction.CallbackContext obj) => OnMenuAction?.Invoke(this, EventArgs.Empty);
    }
}