using System;
using _Scripts.Combat;
using _Scripts.Managers;
using _Scripts.MechaParts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Controller
{
    public class MechaController : MonoBehaviour
    {
        public static MechaController Instance { get; private set; }

        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private float interactDistance = 2f;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private LayerMask interactLayerMask;
        
        private CharacterController _controller;
        private InputManager _inputManager;
        private Vector3 _mechaVelocity, _move;
        private bool _groundedMecha;
        
        public float medianWeight;

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

        [Header("Attack Spawn Points")] 
        public Transform leftArmRangedSpawn;
        public Transform rightArmRangedSpawn;
        public Transform leftArmMeleeSpawn;
        public Transform rightArmMeleeSpawn;

        
        public GameObject projectilePrefab;

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
            _inputManager.OnRightAction += OnRightAction;
            _inputManager.OnJumpAction += OnJumpAction;
            _inputManager.OnJumpActionReleased += OnJumpActionReleased;
            _controller = GetComponent<CharacterController>();
            _maxHp = headPart.HP + torsoPart.HP + leftArmPart.HP + rightArmPart.HP +
                     legsPart.HP;
            _maxHp = bonusPart != null ? _maxHp + bonusPart.HP : _maxHp;
            _currentHp = _maxHp;
            UpdateMech();
        }

        private void OnDestroy()
        {
            _inputManager.OnLeftAction -= OnLeftAction;
            _inputManager.OnRightAction -= OnRightAction;
            _inputManager.OnJumpAction -= OnJumpAction;
            _inputManager.OnJumpActionReleased -= OnJumpActionReleased;
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
            switch (leftArmPart.type)
            {
                case ArmType.PROJECTILE:
                    UseProjectile(leftArmRangedSpawn.position);
                    break;
                case ArmType.HITSCAN:
                    UseHitscan(leftArmRangedSpawn);
                    break;
                case ArmType.MELEE:
                    UseMelee(leftArmMeleeSpawn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnRightAction(object sender, EventArgs e)
        {
            switch (rightArmPart.type)
            {
                case ArmType.PROJECTILE:
                    UseProjectile(rightArmRangedSpawn.position);
                    break;
                case ArmType.HITSCAN:
                    UseHitscan(rightArmRangedSpawn);
                    break;
                case ArmType.MELEE:
                    UseMelee(rightArmMeleeSpawn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UseProjectile(Vector3 spawnPoint)
        {
            Ray ray = gameCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            Vector3 destination = Physics.Raycast(ray, out var hit) ? hit.point : ray.GetPoint(1000);
            GameObject instantiated = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity);
            Projectile projectile = instantiated.GetComponent<Projectile>();
            projectile.body.velocity = (destination - spawnPoint).normalized * -1 * projectile.projectileSpeed;
        }

        private void OnDrawGizmos()
        {
            Ray ray = gameCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray.origin, ray.direction);
            Vector3 destination = Physics.Raycast(ray, out var hit) ? hit.point : ray.GetPoint(1000);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(leftArmRangedSpawn.position, (destination - leftArmRangedSpawn.position).normalized * -1);
        }
        
        private void UseHitscan(Transform spawnPoint)
        {
        }
        
        private void UseMelee(Transform spawnPoint)
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