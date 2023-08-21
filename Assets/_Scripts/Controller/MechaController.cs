using System;
using _Scripts.Combat;
using _Scripts.Managers;
using _Scripts.MechaParts;
using UnityEngine;

namespace _Scripts.Controller
{
    public class MechaController : MonoBehaviour
    {
        public static MechaController Instance { get; private set; }

        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private float interactDistance = 2f;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private Transform raycastOrigin;
        [SerializeField] private LayerMask interactLayerMask;
        [SerializeField] private LayerMask raycastLayerMask;

        [SerializeField] private float medianWeight;

        [Header("Mech Parts SO")] 
        [SerializeField] private Head headPart;
        [SerializeField] private Torso torsoPart;
        [SerializeField] private Arm leftArmPart;
        [SerializeField] private Arm rightArmPart;
        [SerializeField] private Legs legsPart;
        [SerializeField] private BonusPart bonusPart;

        [Header("Attack Spawn Points")] 
        [SerializeField] private Transform leftArmRangedSpawn;
        [SerializeField] private Transform rightArmRangedSpawn;
        [SerializeField] private Transform leftArmMeleeSpawn;
        [SerializeField] private Transform rightArmMeleeSpawn;
        
        [SerializeField] private GameObject projectilePrefab;
        
        private CharacterController _controller;
        private InputManager _inputManager;
        private Vector3 _mechaVelocity, _move;
        private bool _groundedMecha, _leftFiring, _rightFiring;
        private int _maxHp;
        private int _currentHp;
        private int _currentWeight;

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
            _inputManager.OnLeftAction += OnLeftAction;
            _inputManager.OnLeftActionReleased += OnLeftActionReleased;
            _inputManager.OnRightAction += OnRightAction;
            _inputManager.OnRightActionReleased += OnRightActionReleased;
            _inputManager.OnJumpAction += OnJumpAction;
            _inputManager.OnJumpActionReleased += OnJumpActionReleased;
            _controller = GetComponent<CharacterController>();
            _maxHp = headPart.HP + torsoPart.HP + leftArmPart.HP + rightArmPart.HP + legsPart.HP;
            _maxHp = bonusPart != null ? _maxHp + bonusPart.HP : _maxHp;
            _currentHp = _maxHp;
            UpdateMech();
        }

        private void OnDestroy()
        {
            _inputManager.OnLeftAction -= OnLeftAction;
            _inputManager.OnLeftActionReleased -= OnLeftActionReleased;
            _inputManager.OnRightAction -= OnRightAction;
            _inputManager.OnRightActionReleased -= OnRightActionReleased;
            _inputManager.OnJumpAction -= OnJumpAction;
            _inputManager.OnJumpActionReleased -= OnJumpActionReleased;
        }

        //call when mech parts are changed out
        public void UpdateMech()
        {
            _currentWeight = headPart.weight + torsoPart.weight + leftArmPart.weight + rightArmPart.weight + legsPart.weight;
            _currentWeight = bonusPart != null ? _currentWeight + bonusPart.weight : _currentWeight;

            float hpLoss = 1.0f * _currentHp / _maxHp;
            
            _maxHp = headPart.HP + torsoPart.HP + leftArmPart.HP + rightArmPart.HP + legsPart.HP;
            _maxHp = bonusPart != null ? _maxHp + bonusPart.HP : _maxHp;

            _currentHp = Mathf.RoundToInt(_maxHp * hpLoss);
            Debug.Log("The mech weighs " + _currentWeight + "kg, and has " + _currentHp + "/" + _maxHp + " HP.");
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

            Vector3 cameraForward = gameCamera.transform.forward;
            Vector3 inputVector = _inputManager.GetPlayerMovement();
            _move = new Vector3(inputVector.x, 0f, inputVector.y);
            _move = cameraForward * _move.z + gameCamera.transform.right * _move.x;
            _move.y = 0;
            transform.forward = new Vector3(cameraForward.x, 0f, cameraForward.z);
            float moveSpeed = legsPart.speed * (medianWeight / _currentWeight) * Time.deltaTime;
            _controller.Move(_move * moveSpeed);
            _mechaVelocity.y += gravityValue * Time.deltaTime;
            _controller.Move(_mechaVelocity * Time.deltaTime);
        }

        
        private void OnLeftAction(object sender, EventArgs e)
        {
            _leftFiring = true;
            switch (leftArmPart.type)
            {
                case ArmType.PROJECTILE:
                    UseProjectile(leftArmRangedSpawn.position);
                    break;
                case ArmType.HITSCAN:
                    UseHitscan(leftArmRangedSpawn.position);
                    break;
                case ArmType.MELEE:
                    UseMelee(leftArmMeleeSpawn.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnLeftActionReleased(object sender, EventArgs e)
        {
            _leftFiring = false;
        }
        
        private void OnRightAction(object sender, EventArgs e)
        {
            _rightFiring = true;
            switch (rightArmPart.type)
            {
                case ArmType.PROJECTILE:
                    UseProjectile(rightArmRangedSpawn.position);
                    break;
                case ArmType.HITSCAN:
                    UseHitscan(rightArmRangedSpawn.position);
                    break;
                case ArmType.MELEE:
                    UseMelee(rightArmMeleeSpawn.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnRightActionReleased(object sender, EventArgs e)
        {
            _rightFiring = false;
        }

        private void UseProjectile(Vector3 spawnPoint)
        {
            Ray ray = gameCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            Vector3 targetPoint = Physics.Raycast(ray, out var hit, raycastLayerMask) ? hit.point : ray.GetPoint(75);
            Vector3 direction = targetPoint - spawnPoint;
            Projectile projectile = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity).GetComponent<Projectile>();
            projectile.gameObject.transform.forward = direction.normalized;
            projectile.body.AddForce(direction.normalized * projectile.projectileSpeed, ForceMode.Impulse);
        }

        private void UseHitscan(Vector3 spawnPoint)
        {
            Ray ray = gameCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            if (Physics.Raycast(ray, out var hit, raycastLayerMask))
            {
                //do damage according to what was hit
            }
            //display muzzle at spawnPoint
        }
        
        private void UseMelee(Vector3 spawnPoint)
        {
            
        }
        
        private void OnJumpAction(object sender, EventArgs e)
        {
            if (_groundedMecha)
                _mechaVelocity.y = Mathf.Sqrt(legsPart.jumpPower * (medianWeight / _currentWeight) * -2f * gravityValue);
        }
        
        private void OnJumpActionReleased(object sender, EventArgs e)
        {
        }

        
    }
}