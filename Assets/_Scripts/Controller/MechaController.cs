using System;
using _Scripts.Combat;
using _Scripts.Managers;
using _Scripts.MechaParts;
using Extensions;
using UnityEngine;

namespace _Scripts.Controller
{
    public class MechaController : MonoBehaviour
    {
        public static MechaController Instance { get; private set; }
        public Arm LeftArm { get; set; }
        public Arm RightArm { get; set; }
        public Torso Torso { get; set; }
        public Legs Legs { get; set; }
        public BonusPart BonusPart { get; set; }

        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private LayerMask raycastLayerMask;
        [SerializeField] private LayerMask hittableLayerMask;

        [SerializeField] private float medianWeight;
        [SerializeField] private float meleeAttackRange;
        [SerializeField] private float dashForce;
        [SerializeField] private float dashJumpForce;
        [SerializeField] [Range(0,1)] private float dashControlLoss;
        
        [Header("Mech Parts SO")] 
        [SerializeField] private Inventory inventory;

        [Header("Mech Parts Positions")] 
        [SerializeField] private Transform leftArmTransform;
        [SerializeField] private Transform rightArmTransform;
        [SerializeField] private Transform torsoTransform;
        [SerializeField] private Transform legsTransform;
        
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Animator mechaAnimator;
        
        private CharacterController _controller;
        private InputManager _inputManager;
        private Transform _leftArmSpawnPoint, _rightArmSpawnPoint;
        private Vector3 _mechaVelocity, _move;
        private bool _groundedMecha, _leftFiring, _rightFiring, _dashing;
        private int _maxHp, _currentHp, _currentWeight;
        private static readonly int Moving = Animator.StringToHash("Moving");

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
            _inputManager.OnInteractAction += OnInteractAction;
            _inputManager.OnDashAction += OnDashAction;
            _inputManager.OnDashActionReleased += OnDashActionReleased;
            _controller = GetComponent<CharacterController>();

            LeftArm = inventory.defaultLeftArm;
            RightArm = inventory.defaultRightArm;
            Torso = inventory.defaultTorso;
            Legs = inventory.defaultLegs;
            BonusPart = inventory.defaultBonusPart;
            _maxHp = GetMaxHp();
            _currentHp = _maxHp;
            _currentWeight = GetWeight();
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
            _inputManager.OnInteractAction -= OnInteractAction;
            _inputManager.OnDashAction -= OnDashAction;
            _inputManager.OnDashActionReleased -= OnDashActionReleased;
        }

        //call when mech parts are changed out
        private void Update()
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            HandleMovement();
        }

        public void UpdateMech()
        {
            _leftArmSpawnPoint = LeftArm.prefab.GetComponent<ArmBehaviour>().spawnPoint;
            _rightArmSpawnPoint = RightArm.prefab.GetComponent<ArmBehaviour>().spawnPoint;
            float hpLoss = 1.0f * _currentHp / _maxHp;
            _currentHp = Mathf.RoundToInt(_maxHp * hpLoss);
            Debug.Log("The mech weighs " + GetWeight() + "kg, and has " + _currentHp + "/" + _maxHp + " HP.");
        }

        private int GetWeight()
        {
            int currentWeight = Torso.weight + LeftArm.weight + RightArm.weight + Legs.weight;
            currentWeight = BonusPart != null ? currentWeight + BonusPart.weight : currentWeight;
            return currentWeight;
        }

        private int GetMaxHp()
        {
            int maxHp = Torso.HP + LeftArm.HP + RightArm.HP + Legs.HP;
            maxHp = BonusPart != null ? maxHp + BonusPart.HP : maxHp;
            return maxHp;
        }

        private void HandleMovement()
        {
            _groundedMecha = _controller.isGrounded;
            if (_groundedMecha && _mechaVelocity.y < 0 && !_dashing)
                _mechaVelocity.y = 0f;

            Vector3 cameraForward = gameCamera.transform.forward;
            Vector3 inputVector = _inputManager.GetPlayerMovement();
            _move = new Vector3(inputVector.x, 0f, inputVector.y);
            _move = cameraForward * _move.z + gameCamera.transform.right * _move.x;
            _move.y = 0;
            transform.forward = new Vector3(cameraForward.x, 0f, cameraForward.z);
            float moveSpeed = Legs.speed * (medianWeight / _currentWeight) * Time.deltaTime;
            if (_dashing) moveSpeed *= dashForce;
            _controller.Move(_move * moveSpeed);
            _mechaVelocity.y += gravityValue * Time.deltaTime;
            _controller.Move(_mechaVelocity * Time.deltaTime);
            
            mechaAnimator.SetBool(Moving, inputVector.magnitude != 0);
        }

        
        private void OnLeftAction(object sender, EventArgs e)
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            _leftFiring = true;
            switch (LeftArm.type)
            {
                case ArmType.PROJECTILE:
                    UseProjectile(_leftArmSpawnPoint.position, true);
                    break;
                case ArmType.HITSCAN:
                    UseHitscan(_leftArmSpawnPoint.position, true);
                    break;
                case ArmType.MELEE:
                    UseMelee(_leftArmSpawnPoint.position, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnLeftActionReleased(object sender, EventArgs e)
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            _leftFiring = false;
        }
        
        private void OnRightAction(object sender, EventArgs e)
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            _rightFiring = true;
            switch (RightArm.type)
            {
                case ArmType.PROJECTILE:
                    UseProjectile(_rightArmSpawnPoint.position, false);
                    break;
                case ArmType.HITSCAN:
                    UseHitscan(_rightArmSpawnPoint.position, false);
                    break;
                case ArmType.MELEE:
                    UseMelee(_rightArmSpawnPoint.position, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void OnRightActionReleased(object sender, EventArgs e)
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            _rightFiring = false;
        }

        private void UseProjectile(Vector3 spawnPoint, bool left)
        {
            Ray ray = gameCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            Vector3 targetPoint = Physics.Raycast(ray, out var hit, raycastLayerMask) ? hit.point : ray.GetPoint(75);
            Vector3 direction = targetPoint - spawnPoint;
            Projectile projectile = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity).GetComponent<Projectile>();
            projectile.gameObject.transform.forward = direction.normalized;
            projectile.body.AddForce(direction.normalized * projectile.projectileSpeed, ForceMode.Impulse);
            projectile.projectileDamage = left ? LeftArm.damage : RightArm.damage;
        }

        private void UseHitscan(Vector3 spawnPoint, bool left)
        {
            Ray ray = gameCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            if (Physics.Raycast(ray, out var hit, hittableLayerMask))
            {
                int damage = left ? LeftArm.damage : RightArm.damage;
                hit.collider.gameObject.GetComponent<Hittable>().DoDamage(damage);
            }
            //display muzzle at spawnPoint
        }
        
        private void UseMelee(Vector3 spawnPoint, bool left)
        {
            Collider[] cols = Physics.OverlapSphere(spawnPoint, meleeAttackRange, raycastLayerMask);

            foreach (Collider col in cols)
            {
                if (hittableLayerMask.HasLayer(col.gameObject.layer))
                {
                    int damage = left ? LeftArm.damage : RightArm.damage;
                    col.gameObject.GetComponent<Hittable>().DoDamage(damage);
                }
            }
            
        }
        
        private void OnJumpAction(object sender, EventArgs e)
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            if (_groundedMecha)
                if (_dashing)
                   _mechaVelocity.y = Mathf.Sqrt(Legs.jumpPower * dashJumpForce * (medianWeight / _currentWeight) * -2f * gravityValue);
                else
                    _mechaVelocity.y = Mathf.Sqrt(Legs.jumpPower * (medianWeight / _currentWeight) * -2f * gravityValue);
        }
        
        private void OnJumpActionReleased(object sender, EventArgs e)
        {
        }

        private void OnInteractAction(object sender, EventArgs e)
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            //GameManager.Instance.ExitMecha();
        }

        private void OnDashAction(object sender, EventArgs e)
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            _dashing = true;
        } 
        
        private void OnDashActionReleased(object sender, EventArgs e)
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            _dashing = false;
        } 
    }
}