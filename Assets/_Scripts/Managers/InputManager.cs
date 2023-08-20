using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        public event EventHandler OnInteractAction;
        public event EventHandler OnInteractAlternateAction;
        public event EventHandler OnMenuAction;

        private const string PlayerPrefsBindings = "InputBindings";
        private PlayerControls _playerControls;

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
            _playerControls.Player.Menu.performed += OnMenuActionPerformed;
        }

        private void OnDestroy()
        {
            _playerControls.Disable();
            _playerControls.Player.Interact.performed -= OnInteractPerformed;
            _playerControls.Player.InteractAlternate.performed -= OnInteractAlternatePerformed;
            _playerControls.Player.Menu.performed -= OnMenuActionPerformed;
            _playerControls.Dispose();
        }

        public Vector2 GetPlayerMovement() => _playerControls.Player.Movement.ReadValue<Vector2>();

        public Vector2 GetLookDelta() => _playerControls.Player.Look.ReadValue<Vector2>();

        private void OnInteractPerformed(InputAction.CallbackContext obj) => OnInteractAction?.Invoke(this, EventArgs.Empty);

        private void OnInteractAlternatePerformed(InputAction.CallbackContext obj) => OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);

        private void OnMenuActionPerformed(InputAction.CallbackContext obj) => OnMenuAction?.Invoke(this, EventArgs.Empty);
    }
}