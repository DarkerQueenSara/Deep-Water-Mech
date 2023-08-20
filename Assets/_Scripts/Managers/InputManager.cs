using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public event EventHandler OnLeftAction;
        public event EventHandler OnLeftActionReleased;
        public event EventHandler OnRightAction;
        public event EventHandler OnRightActionReleased;
        public event EventHandler OnJumpAction;
        public event EventHandler OnJumpActionReleased;
        public event EventHandler OnCrouchAction;
        public event EventHandler OnCrouchActionReleased;
        public event EventHandler OnDashAction;
        public event EventHandler OnDashActionReleased;
        public event EventHandler OnInteractAction;
        public event EventHandler OnMenuAction;

        private const string PlayerPrefsBindings = "InputBindings";
        private PlayerControls _playerControls;
        private bool _wasLeftPressed, _wasRightPressed, _wasJumpPressed, _wasDashPressed, _wasCrouchPressed;

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
            _playerControls.Player.Dash.performed += OnDashPerformed;
            _playerControls.Player.Crouch.performed += OnCrouchPerformed;
            _playerControls.Player.Interact.performed += OnInteractPerformed;
            _playerControls.Player.Menu.performed += OnMenuActionPerformed;
        }

        private void OnDestroy()
        {
            _playerControls.Disable();
            _playerControls.Player.LeftArm.performed -= OnLeftPerformed;
            _playerControls.Player.RightArm.performed -= OnRightPerformed;
            _playerControls.Player.Jump.performed -= OnJumpPerformed;
            _playerControls.Player.Dash.performed -= OnDashPerformed;
            _playerControls.Player.Menu.performed -= OnMenuActionPerformed;
            _playerControls.Dispose();
        }

        private void Update()
        {
            if (_wasLeftPressed && _playerControls.Player.LeftArm.WasReleasedThisFrame())
            {
                OnLeftActionReleased?.Invoke(this, EventArgs.Empty);
                _wasLeftPressed = false;
            }
            
            if (_wasRightPressed && _playerControls.Player.RightArm.WasReleasedThisFrame())
            {
                OnRightActionReleased?.Invoke(this, EventArgs.Empty);
                _wasRightPressed = false;
            }
            
            if (_wasJumpPressed && _playerControls.Player.Jump.WasReleasedThisFrame())
            {
                OnJumpActionReleased?.Invoke(this, EventArgs.Empty);
                _wasJumpPressed = false;
            }
            
            if (_wasDashPressed && _playerControls.Player.Dash.WasReleasedThisFrame())
            {
                OnDashActionReleased?.Invoke(this, EventArgs.Empty);
                _wasDashPressed = false;
            }
            
            if (_wasCrouchPressed && _playerControls.Player.Crouch.WasReleasedThisFrame())
            {
                OnCrouchActionReleased?.Invoke(this, EventArgs.Empty);
                _wasCrouchPressed = false;
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
        
        private void OnDashPerformed(InputAction.CallbackContext obj)
        {
            OnDashAction?.Invoke(this, EventArgs.Empty);
            _wasDashPressed = true;
        }

        private void OnCrouchPerformed(InputAction.CallbackContext obj)
        {
            OnCrouchAction?.Invoke(this, EventArgs.Empty);
            _wasCrouchPressed = true;
        }

        private void OnInteractPerformed(InputAction.CallbackContext obj) => OnInteractAction?.Invoke(this, EventArgs.Empty);

        private void OnMenuActionPerformed(InputAction.CallbackContext obj) => OnMenuAction?.Invoke(this, EventArgs.Empty);
    }
}