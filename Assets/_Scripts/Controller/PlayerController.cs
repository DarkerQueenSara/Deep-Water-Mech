using System;
using System.Collections;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
        
        [SerializeField] private float playerSpeed = 2.0f;
        [SerializeField] private float jumpHeight = 2.0f;
        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private float interactDistance = 2f;
        [SerializeField] private float heightLerpSpeed = 0.1f;
        [SerializeField] private float moveLerpSpeed = 0.1f;
        [SerializeField] private float swimRate = 1.0f; 
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private LayerMask interactLayerMask;
        
        private CharacterController _controller;
        private InputManager _inputManager;
        private Vector3 _playerVelocity, _move;
        private float _currentSwimHeight;
        private float _swimHeightChangeRate;
        private bool _groundedPlayer;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one PlayerController! {transform} - {Instance}");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _controller = GetComponent<CharacterController>();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        private void Start()
        {
            _inputManager = InputManager.Instance;
            _inputManager.OnInteractAction += OnInteractAction;
            _inputManager.OnInteractActionReleased += OnInteractActionReleased;
            _inputManager.OnInteractAlternateAction += OnInteractAlternateAction;
            _inputManager.OnInteractAlternateActionReleased += OnInteractAlternateActionReleased;
            _inputManager.OnJumpAction += OnJumpAction;
            _inputManager.OnJumpActionReleased += OnJumpActionReleased;
            _inputManager.OnCrouchAction += OnCrouchAction;
            _inputManager.OnCrouchActionReleased += OnCrouchActionReleased;
        }

        private void OnDestroy()
        {
            _inputManager.OnInteractAction -= OnInteractAction;
            _inputManager.OnInteractAlternateAction -= OnInteractAlternateAction;
            _inputManager.OnJumpAction -= OnJumpAction;
            _inputManager.OnJumpActionReleased -= OnJumpActionReleased;
            _inputManager.OnCrouchAction -= OnCrouchAction;
            _inputManager.OnCrouchActionReleased -= OnCrouchActionReleased;
        }

        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            // _groundedPlayer = _controller.isGrounded;
            // if (_groundedPlayer && _playerVelocity.y < 0)
            //     _playerVelocity.y = 0f;

            Vector3 cameraForward = cameraTransform.forward;
            Vector3 inputVector = _inputManager.GetPlayerMovement();
            Vector3 targetMove = cameraForward * inputVector.y + cameraTransform.right * inputVector.x;
            _move = Vector3.Lerp(_move, targetMove, moveLerpSpeed * Time.deltaTime);
            _move.y = 0;
            transform.forward = new Vector3(cameraForward.x, 0f, cameraForward.z);
            _currentSwimHeight = Mathf.Lerp(_currentSwimHeight, _swimHeightChangeRate, heightLerpSpeed);
            Vector3 swimMovement = (_move * playerSpeed + Vector3.up * _currentSwimHeight) * Time.deltaTime;
            _controller.Move(swimMovement);
        }

        private void OnInteractAction(object sender, EventArgs e)
        {
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hitInfo,
                    interactDistance, interactLayerMask))
            {
            }
        }
        
        private void OnInteractActionReleased(object sender, EventArgs e)
        {
        }
        
        private void OnInteractAlternateAction(object sender, EventArgs e)
        {
        }
        
        private void OnInteractAlternateActionReleased(object sender, EventArgs e)
        {
        }
        
        private void OnJumpAction(object sender, EventArgs e)
        {
            _swimHeightChangeRate = swimRate;
        }

        private void OnJumpActionReleased(object sender, EventArgs e)
        {
            _swimHeightChangeRate = 0.0f;
        }

        private void OnCrouchAction(object sender, EventArgs e)
        {
            _swimHeightChangeRate = -swimRate;
        }

        private void OnCrouchActionReleased(object sender, EventArgs e)
        {
            _swimHeightChangeRate = 0.0f;
        }
    }
}