using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public event EventHandler OnInteractAction;
        public event EventHandler OnInteractActionReleased;
        public event EventHandler OnInteractAlternateAction;
        public event EventHandler OnInteractAlternateActionReleased;
        public event EventHandler OnJumpAction;
        public event EventHandler OnJumpActionReleased;
        public event EventHandler OnCrouchAction;
        public event EventHandler OnCrouchActionReleased;
        public event EventHandler OnMenuAction;

        private const string PlayerPrefsBindings = "InputBindings";
        private PlayerControls _playerControls;
        private bool _wasInteractPressed;
        private bool _wasInteractAlternatePressed;
        private bool _wasJumpPressed;
        private bool _wasCrouchPressed;

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
            _playerControls.Player.Interact.performed += OnInteractPerformed;
            _playerControls.Player.InteractAlternate.performed += OnInteractAlternatePerformed;
            _playerControls.Player.Jump.performed += OnJumpPerformed;
            _playerControls.Player.Crouch.performed += OnCrouchPerformed;
            _playerControls.Player.Menu.performed += OnMenuActionPerformed;
        }

        private void OnDestroy()
        {
            _playerControls.Disable();
            _playerControls.Player.Interact.performed -= OnInteractPerformed;
            _playerControls.Player.InteractAlternate.performed -= OnInteractAlternatePerformed;
            _playerControls.Player.Jump.performed -= OnJumpPerformed;
            _playerControls.Player.Crouch.performed -= OnCrouchPerformed;
            _playerControls.Player.Menu.performed -= OnMenuActionPerformed;
            _playerControls.Dispose();
        }

        private void Update()
        {
            if (_wasInteractPressed && _playerControls.Player.Interact.WasReleasedThisFrame())
            {
                OnInteractActionReleased?.Invoke(this, EventArgs.Empty);
                _wasInteractPressed = false;
            }
            
            if (_wasInteractAlternatePressed && _playerControls.Player.InteractAlternate.WasReleasedThisFrame())
            {
                OnInteractAlternateActionReleased?.Invoke(this, EventArgs.Empty);
                _wasInteractAlternatePressed = false;
            }
            
            if (_wasJumpPressed && _playerControls.Player.Jump.WasReleasedThisFrame())
            {
                OnJumpActionReleased?.Invoke(this, EventArgs.Empty);
                _wasJumpPressed = false;
            }
            
            if (_wasCrouchPressed && _playerControls.Player.Crouch.WasReleasedThisFrame())
            {
                OnCrouchActionReleased?.Invoke(this, EventArgs.Empty);
                _wasCrouchPressed = false;
            }
        }

        public Vector2 GetPlayerMovement() => _playerControls.Player.Movement.ReadValue<Vector2>();

        public Vector2 GetLookDelta() => _playerControls.Player.Look.ReadValue<Vector2>();

        private void OnInteractPerformed(InputAction.CallbackContext obj)
        {
            OnInteractAction?.Invoke(this, EventArgs.Empty);
            _wasInteractPressed = true;
        }

        private void OnInteractAlternatePerformed(InputAction.CallbackContext obj)
        {
            OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
            _wasInteractAlternatePressed = true;
        } 
        
        private void OnJumpPerformed(InputAction.CallbackContext obj)
        {
            OnJumpAction?.Invoke(this, EventArgs.Empty);
            _wasJumpPressed = true;
        }
        
        private void OnCrouchPerformed(InputAction.CallbackContext obj)
        {
            OnCrouchAction?.Invoke(this, EventArgs.Empty);
            _wasCrouchPressed = true;
        }

        private void OnMenuActionPerformed(InputAction.CallbackContext obj) => OnMenuAction?.Invoke(this, EventArgs.Empty);
    }
}