using System;
using _Scripts.Managers;
using _Scripts.MechaParts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Controller
{
    public class MechaController : MonoBehaviour
    {
        public static MechaController Instance { get; private set; }

        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private float interactDistance = 2f;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private LayerMask interactLayerMask;
        
        private CharacterController _controller;
        private InputManager _inputManager;
        private Vector3 _mechaVelocity, _move;
        private bool _groundedMecha;
        
        public float medianWeight = 200.0f;

        private int _maxHp;
        private int _currentHp;
        private int _currentWeight;
        
        [Header("Mech Parts SO")] 
        public Head headPart;
        public Torso torsoPart;
        public Arm leftArmPart;
        public Arm rightArmPart;
        public Legs legsPart;
        public BonusPart bonusPart;

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
            _controller = GetComponent<CharacterController>();
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

        //call when mech parts are changed out
        public void UpdateMech()
        {
            _currentWeight = headPart.weight + torsoPart.weight + leftArmPart.weight + rightArmPart.weight +
                                          legsPart.weight;
            _currentWeight = bonusPart != null ? _currentWeight + bonusPart.weight : _currentWeight;

            float hpLoss = 1.0f * _currentHp / _maxHp;
            
            _maxHp = headPart.HP + torsoPart.HP + leftArmPart.HP + rightArmPart.HP +
                                      legsPart.HP;
            _maxHp = bonusPart != null ? _maxHp + bonusPart.HP : _maxHp;

            _currentHp = Mathf.RoundToInt(_maxHp * hpLoss);
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
            Debug.Log(inputVector);
            _move = new Vector3(inputVector.x, 0f, inputVector.y);
            _move = cameraForward * _move.z + cameraTransform.right * _move.x;
            _move.y = 0;
            transform.forward = new Vector3(cameraForward.x, 0f, cameraForward.z);
            float moveSpeed = legsPart.speed * medianWeight / _currentWeight * Time.deltaTime;
            _controller.Move(_move * (Time.deltaTime * moveSpeed));
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