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
        
        public enum Binding
        {
            MoveUp,
            MoveDown,
            MoveLeft,
            MoveRight,
            LeftArm,
            RightArm,
            Jump,
            Dash,
            Crouch,
            Interact,
            Menu,
            GamepadLeftArm,
            GamepadRightArm,
            GamepadJump,
            GamepadDash,
            GamepadCrouch,
            GamepadInteract,
            GamepadMenu
        }

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
        
        public void RebindBinding(Binding binding, Action onActionRebound)
        {
            InputAction inputAction;
            int bindingIndex;
            switch (binding)
            {
                case Binding.MoveUp:
                    inputAction = _playerControls.Player.Movement;
                    bindingIndex = 1;
                    break;
                case Binding.MoveDown:
                    inputAction = _playerControls.Player.Movement;
                    bindingIndex = 2;
                    break;
                case Binding.MoveLeft:
                    inputAction = _playerControls.Player.Movement;
                    bindingIndex = 3;
                    break;
                case Binding.MoveRight:
                    inputAction = _playerControls.Player.Movement;
                    bindingIndex = 4;
                    break;
                case Binding.LeftArm:
                    inputAction = _playerControls.Player.Interact;
                    bindingIndex = 0;
                    break;
                case Binding.RightArm:
                    inputAction = _playerControls.Player.LeftArm;
                    bindingIndex = 0;
                    break;
                case Binding.Jump:
                    inputAction = _playerControls.Player.Jump;
                    bindingIndex = 0;
                    break;
                case Binding.Dash:
                    inputAction = _playerControls.Player.Dash;
                    bindingIndex = 0;
                    break;
                case Binding.Crouch:
                    inputAction = _playerControls.Player.Crouch;
                    bindingIndex = 0;
                    break;
                case Binding.Interact:
                    inputAction = _playerControls.Player.Interact;
                    bindingIndex = 0;
                    break;
                case Binding.Menu:
                    inputAction = _playerControls.Player.Menu;
                    bindingIndex = 0;
                    break;
                case Binding.GamepadLeftArm:
                    inputAction = _playerControls.Player.LeftArm;
                    bindingIndex = 1;
                    break;
                case Binding.GamepadRightArm:
                    inputAction = _playerControls.Player.RightArm;
                    bindingIndex = 1;
                    break;
                case Binding.GamepadJump:
                    inputAction = _playerControls.Player.Jump;
                    bindingIndex = 1;
                    break;
                case Binding.GamepadDash:
                    inputAction = _playerControls.Player.Dash;
                    bindingIndex = 1;
                    break;
                case Binding.GamepadCrouch:
                    inputAction = _playerControls.Player.Crouch;
                    bindingIndex = 1;
                    break;
                case Binding.GamepadInteract:
                    inputAction = _playerControls.Player.Interact;
                    bindingIndex = 1;
                    break;
                case Binding.GamepadMenu:
                    inputAction = _playerControls.Player.Menu;
                    bindingIndex = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(binding), binding, null);
            }

            _playerControls.Player.Disable();
            inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
            {
                callback.Dispose();
                _playerControls.Player.Enable();
                onActionRebound();
                PlayerPrefs.SetString(PlayerPrefsBindings, _playerControls.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            }).Start();
        }

        public string GetBindingText(Binding binding)
        {
            return binding switch
            {
                Binding.MoveUp => _playerControls.Player.Movement.bindings[1].ToDisplayString(),
                Binding.MoveDown => _playerControls.Player.Movement.bindings[2].ToDisplayString(),
                Binding.MoveLeft => _playerControls.Player.Movement.bindings[3].ToDisplayString(),
                Binding.MoveRight => _playerControls.Player.Movement.bindings[4].ToDisplayString(),
                Binding.LeftArm => _playerControls.Player.LeftArm.bindings[0].ToDisplayString(),
                Binding.RightArm => _playerControls.Player.RightArm.bindings[0].ToDisplayString(),
                Binding.Jump => _playerControls.Player.Jump.bindings[0].ToDisplayString(),
                Binding.Dash => _playerControls.Player.Dash.bindings[0].ToDisplayString(),
                Binding.Crouch => _playerControls.Player.Crouch.bindings[0].ToDisplayString(),
                Binding.Interact => _playerControls.Player.Interact.bindings[0].ToDisplayString(),
                Binding.Menu => _playerControls.Player.Menu.bindings[0].ToDisplayString(),
                Binding.GamepadLeftArm => _playerControls.Player.LeftArm.bindings[1].ToDisplayString(),
                Binding.GamepadRightArm => _playerControls.Player.RightArm.bindings[1].ToDisplayString(),
                Binding.GamepadMenu => _playerControls.Player.Menu.bindings[1].ToDisplayString(),
                Binding.GamepadJump => _playerControls.Player.Jump.bindings[1].ToDisplayString(),
                Binding.GamepadDash => _playerControls.Player.Dash.bindings[1].ToDisplayString(),
                Binding.GamepadCrouch => _playerControls.Player.Crouch.bindings[1].ToDisplayString(),
                Binding.GamepadInteract => _playerControls.Player.Interact.bindings[1].ToDisplayString(),
                _ => throw new ArgumentOutOfRangeException(nameof(binding), binding, null)
            };
        }
    }
}