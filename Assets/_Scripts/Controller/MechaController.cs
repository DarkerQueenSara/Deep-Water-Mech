using System;
using _Scripts.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Controller
{
    public class MechaController : MonoBehaviour
    {
        public static MechaController Instance { get; private set; }

        [SerializeField] private float mechaSpeed = 2.0f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private float interactDistance = 2f;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private LayerMask interactLayerMask;
        
        private CharacterController _controller;
        private InputManager _inputManager;
        private Vector3 _mechaVelocity, _move;
        private bool _groundedMecha;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There's more than one MechaController! {transform} - {Instance}");
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
            _groundedMecha = _controller.isGrounded;
            if (_groundedMecha && _mechaVelocity.y < 0)
                _mechaVelocity.y = 0f;

            Vector3 cameraForward = cameraTransform.forward;
            Vector3 inputVector = _inputManager.GetPlayerMovement();
            _move = new Vector3(inputVector.x, 0f, inputVector.y);
            _move = cameraForward * _move.z + cameraTransform.right * _move.x;
            _move.y = 0;
            transform.forward = new Vector3(cameraForward.x, 0f, cameraForward.z);
            _controller.Move(_move * (Time.deltaTime * mechaSpeed));
            _mechaVelocity.y += gravityValue * Time.deltaTime;
            _controller.Move(_mechaVelocity * Time.deltaTime);
        }

        private void OnInteractAction(object sender, EventArgs e)
        {
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
            if (_groundedMecha)
                _mechaVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }
        
        private void OnJumpActionReleased(object sender, EventArgs e)
        {
        }

        private void OnCrouchAction(object sender, EventArgs e)
        {
        }
        
        private void OnCrouchActionReleased(object sender, EventArgs e)
        {
        }
    }
}