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
            Vector2 inputVector = _inputManager.GetPlayerMovement();
            Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y).normalized;

            if (direction.magnitude >= 0.01f)
            {
                float moveSpeed = legsPart.speed * medianWeight / _currentWeight * Time.deltaTime;
            }

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