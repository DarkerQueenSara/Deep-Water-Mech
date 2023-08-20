using System;
using _Scripts.Managers;
using _Scripts.MechaParts;
using UnityEngine;

namespace _Scripts.Controller
{
    public class MechaController : MonoBehaviour
    {
        public static MechaController Instance { get; private set; }
        
        private InputManager _inputManager;

        private CharacterController _characterController;
        
        public const float MedianWeight = 200.0f;
        
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
            _characterController = GetComponent<CharacterController>();
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
            Vector2 inputVector = _inputManager.GetPlayerMovement();
            Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y).normalized;

            if (direction.magnitude >= 0.01f)
            {
                
            }

        }

        private int GetWeight()
        {
            int weight = headPart.weight + torsoPart.weight + leftArmPart.weight + rightArmPart.weight +
                         legsPart.weight;
            return bonusPart != null ? weight + bonusPart.weight : weight;
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