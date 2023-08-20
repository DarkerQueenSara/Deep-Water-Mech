using System;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
        
        private InputManager _inputManager;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one PlayerController! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        private void Start()
        {
            _inputManager = InputManager.Instance;
            _inputManager.OnInteractAction += OnInteractAction;
            _inputManager.OnInteractAlternateAction += OnInteractAlternateAction;
        }
        
        private void OnDestroy()
        {
            _inputManager.OnInteractAction -= OnInteractAction;
            _inputManager.OnInteractAlternateAction -= OnInteractAlternateAction;
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            Vector3 inputVector = _inputManager.GetPlayerMovement();
        }

        private void OnInteractAction(object sender, EventArgs e)
        {
        }
        
        private void OnInteractAlternateAction(object sender, EventArgs e)
        {
        }
    }
}