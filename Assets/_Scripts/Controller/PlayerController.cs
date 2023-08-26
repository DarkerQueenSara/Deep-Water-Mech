using System;
using System.Collections;
using _Scripts.Managers;
using _Scripts.MechaParts;
using _Scripts.UI;
using Cinemachine;
using UnityEngine;

namespace _Scripts.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Values")] [SerializeField] private float playerSpeed = 2.0f;
        [SerializeField] private float jumpHeight = 2.0f;
        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private float interactDistance = 2f;
        [SerializeField] private float heightLerpSpeed = 0.1f;
        [SerializeField] private float moveLerpSpeed = 0.1f;
        [SerializeField] private float swimRate = 1.0f;

        [Header("References")] [SerializeField]
        private Transform cameraTransform;

        [SerializeField] private LayerMask interactLayerMask;
        [SerializeField] private LayerMask mechaLayerMask;
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private Inventory inventory;
        [SerializeField] private MechSwapHUD mechSwapHUD;
        [SerializeField] private CinemachineInputProvider cinemachineInputProvider;

        private CharacterController _controller;
        private InputManager _inputManager;
        private Vector3 _playerVelocity, _move;
        private float _currentSwimHeight;
        private float _swimHeightChangeRate;
        private bool _groundedPlayer, _menuOpen, _leftPressed;
        private InteractablePart _interactablePartSelected;
        private static readonly int Moving = Animator.StringToHash("Moving");

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
            _inputManager.OnLeftAction += OnLeftAction;
            _inputManager.OnLeftActionReleased += OnLeftActionReleased;
            _inputManager.OnRightAction += OnRightAction;
            _inputManager.OnRightActionReleased += OnRightActionReleased;
            _inputManager.OnJumpAction += OnJumpAction;
            _inputManager.OnJumpActionReleased += OnJumpActionReleased;
            _inputManager.OnCrouchAction += OnCrouchAction;
            _inputManager.OnCrouchActionReleased += OnCrouchActionReleased;
            _inputManager.OnInteractAction += OnInteractAction;
        }

        private void OnDestroy()
        {
            _inputManager.OnLeftAction -= OnLeftAction;
            _inputManager.OnRightAction -= OnRightAction;
            _inputManager.OnJumpAction -= OnJumpAction;
            _inputManager.OnJumpActionReleased -= OnJumpActionReleased;
            _inputManager.OnCrouchAction -= OnCrouchAction;
            _inputManager.OnCrouchActionReleased -= OnCrouchActionReleased;
            _inputManager.OnInteractAction -= OnInteractAction;
        }

        private void Update()
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            HandleMovement();
            HandleAim();
            HandleFixPart();
        }

        private void HandleMovement()
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 inputVector = _inputManager.GetPlayerMovement();
            Vector3 targetMove = cameraForward * inputVector.y + cameraTransform.right * inputVector.x;
            _move = Vector3.Lerp(_move, targetMove, moveLerpSpeed * Time.deltaTime);
            //_move.y = 0;
            transform.forward = new Vector3(cameraForward.x, 0f, cameraForward.z);
            _currentSwimHeight = Mathf.Lerp(_currentSwimHeight, _swimHeightChangeRate, heightLerpSpeed);
            Vector3 swimMovement = (_move * playerSpeed + Vector3.up * _currentSwimHeight) * Time.deltaTime;
            _controller.Move(swimMovement);

            playerAnimator.SetBool(Moving, inputVector.magnitude != 0);
        }

        private void HandleAim()
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hitInfo,
                    interactDistance, interactLayerMask))
            {
                if (hitInfo.collider.TryGetComponent(out InteractablePart interactablePart))
                {
                    _interactablePartSelected = interactablePart;
                    interactablePart.SetSelected(true);
                }
                else
                {
                    if (_interactablePartSelected != null) _interactablePartSelected.SetSelected(false);
                    _interactablePartSelected = null;
                }
            }
            else
            {
                if (_interactablePartSelected != null) _interactablePartSelected.SetSelected(false);
                _interactablePartSelected = null;
            }
        }

        private void HandleFixPart()
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            if (_leftPressed && _interactablePartSelected != null) _interactablePartSelected.RepairPart();
        }
        
        public void HUDClosed()
        {
            _menuOpen = false;
            cinemachineInputProvider.enabled = true;
        }

        private void OnLeftAction(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            if (_interactablePartSelected != null && _interactablePartSelected.isDrop)
            {
                inventory.AddToInventory(_interactablePartSelected.mechPart);
                Destroy(_interactablePartSelected.gameObject);
                _interactablePartSelected = null;
            }
            else
            {
                _leftPressed = true;
            }
        }

        private void OnLeftActionReleased(object sender, EventArgs e)
        {
            _leftPressed = false;
        }

        private void OnRightAction(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hitInfo,
                    interactDistance, mechaLayerMask))
            {
                _menuOpen = true;
                cinemachineInputProvider.enabled = false;
                mechSwapHUD.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void OnRightActionReleased(object sender, EventArgs e)
        {
        }

        private void OnJumpAction(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            _swimHeightChangeRate = swimRate;
        }

        private void OnJumpActionReleased(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            _swimHeightChangeRate = 0.0f;
        }

        private void OnCrouchAction(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            _swimHeightChangeRate = -swimRate;
        }

        private void OnCrouchActionReleased(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            _swimHeightChangeRate = 0.0f;
        }

        private void OnInteractAction(object sender, EventArgs e)
        {
            if (GameManager.Instance.IsInsideMecha || _menuOpen) return;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hitInfo,
                    interactDistance, interactLayerMask))
            {
                GameManager.Instance.EnterMecha();
            }
        }

    }
}