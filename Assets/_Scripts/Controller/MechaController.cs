using System;
using _Scripts.Combat;
using _Scripts.Managers;
using _Scripts.MechaParts;
using _Scripts.MechaParts.SO;
using Extensions;
using UnityEngine;

namespace _Scripts.Controller
{
    public class MechaController : MonoBehaviour
    {
        public static MechaController Instance { get; private set; }

        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private LayerMask raycastLayerMask;
        [SerializeField] private LayerMask hittableLayerMask;

        [SerializeField] private int medianWeight;
        [SerializeField] private int meleeAttackRange;

        [SerializeField] private AnimationCurve rotationCurve;
        [SerializeField] private float maxRotationSpeed = 180f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Animator mechaAnimator;

        [Header("Inventory SO")] [SerializeField]
        private Inventory inventory;

        [Header("Mech Parts Positions")] [SerializeField]
        private Transform leftArmTransform;

        [SerializeField] private Transform rightArmTransform;
        [SerializeField] private Transform torsoTransform;
        [SerializeField] private Transform legsTransform;
        [SerializeField] private Transform bonusPartTransform;

        private CharacterController _controller;
        private InputManager _inputManager;
        private Transform _leftArmSpawnPoint, _rightArmSpawnPoint;
        private Vector3 _mechaVelocity, _move, _lastPos;
        private bool _groundedMecha, _leftFiring, _rightFiring, _dashing;
        [HideInInspector] public int maxHp, currentHp, currentWeight, maxBoost;
        [HideInInspector] public float currentBoost, currentSpeed;
        private float _leftArmCooldownLeft, _rightArmCooldownLeft;
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

            //TODO remove this later as it should happen in the game manager
            inventory.InitiateInventory();

            _leftArmCooldownLeft = 0;
            _rightArmCooldownLeft = 0;

            _lastPos = transform.position;

            maxHp = GetMaxHp();
            currentHp = maxHp;
            maxBoost = 100;
            currentBoost = maxBoost;
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
            if (!GameManager.Instance.IsInsideMecha) return;
            RotateTowardsCamera();
            HandleMovement();
            HandleAttack();

            currentSpeed = Vector3.Distance(_lastPos, transform.position) / Time.deltaTime * 3.6f;
            _lastPos = transform.position;
            
            if (_dashing && currentSpeed > 0)
                currentBoost =
                    Math.Clamp(currentBoost - ((BoostPart)inventory.equippedBonusPart).boostConsumption * Time.deltaTime, 0,
                        maxBoost);
            else
                currentBoost = Math.Clamp(currentBoost + ((BoostPart)inventory.equippedBonusPart).boostRecovery * Time.deltaTime,
                    0, maxBoost);

        }

        public void UpdateMech()
        {
            //Assemble the mech
            /*
            Instantiate(LeftArm.prefab, leftArmTransform.position, Quaternion.identity);
            Instantiate(RightArm.prefab, rightArmTransform.position, Quaternion.identity);
            Instantiate(Torso.prefab, torsoTransform.position, Quaternion.identity);
            Instantiate(Legs.prefab, legsTransform.position, Quaternion.identity);
            if (BonusPart != null)
               Instantiate(BonusPart.prefab, bonusPartTransform.position, Quaternion.identity);
            */

            _leftArmSpawnPoint = inventory.equippedLeftArm.prefab.GetComponent<ArmBehaviour>().spawnPoint;
            _rightArmSpawnPoint = inventory.equippedRightArm.prefab.GetComponent<ArmBehaviour>().spawnPoint;
            float hpLoss = 1.0f * currentHp / maxHp;
            int newMaxHp = GetMaxHp();
            currentHp = Mathf.RoundToInt(newMaxHp * hpLoss);
            maxHp = newMaxHp;
            currentWeight = GetWeight();
            Debug.Log("The mech weighs " + currentWeight + "kg, and has " + currentHp + "/" + maxHp + " HP.");
        }

        private int GetWeight()
        {
            int aux = inventory.equippedHead.weight + inventory.equippedTorso.weight + inventory.equippedLeftArm.weight +
                      inventory.equippedRightArm.weight + inventory.equippedLegs.weight;
            aux = inventory.equippedBonusPart != null ? aux + inventory.equippedBonusPart.weight : aux;
            return aux;
        }

        private int GetMaxHp()
        {
            int aux = inventory.equippedHead.hp + inventory.equippedTorso.hp + inventory.equippedLeftArm.hp +
                      inventory.equippedRightArm.hp +
                      inventory.equippedLegs.hp;
            aux = inventory.equippedBonusPart != null ? aux + inventory.equippedBonusPart.hp : aux;
            return aux;
        }

        public int GetMedianWeight()
        {
            //TODO have a bonus part that increases this value and return value accordinglys
            return medianWeight;
        }

        private void HandleMovement()
        {
            _groundedMecha = _controller.isGrounded;
            if (_groundedMecha && _mechaVelocity.y < 0 && !_dashing)
                _mechaVelocity.y = 0f;

            Vector3 currentRotation = transform.forward;
            Vector3 inputVector = _inputManager.GetPlayerMovement();
            _move = new Vector3(inputVector.x, 0f, inputVector.y);
            _move = currentRotation * _move.z + gameCamera.transform.right * _move.x;
            _move.y = 0;
            transform.forward = new Vector3(currentRotation.x, 0f, currentRotation.z);
            float weightModifier = currentWeight <= medianWeight ? 1 : 1.0f * medianWeight / currentWeight;
            float moveSpeed = inventory.equippedLegs.speed / 3.6f * weightModifier * Time.deltaTime;
            if (_dashing) moveSpeed *= ((BoostPart)inventory.equippedBonusPart).boostForce;
            _controller.Move(_move * moveSpeed);

            mechaAnimator.SetBool(Moving, inputVector.magnitude != 0);
        }

        private void HandleAttack()
        {
            _leftArmCooldownLeft -= Time.deltaTime;
            _rightArmCooldownLeft -= Time.deltaTime;
            if (_leftFiring)
            {
                switch (inventory.equippedLeftArm.type)
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

            if (_rightFiring)
            {
                switch (inventory.equippedRightArm.type)
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
        }

        private void RotateTowardsCamera()
        {
            Vector3 forward = transform.forward;
            Vector3 directionToCamera = gameCamera.transform.forward - forward;
            directionToCamera.y = 0.0f;
            Quaternion targetRotation = Quaternion.LookRotation(gameCamera.transform.forward, Vector3.up);
            float angleToCamera = Vector3.Angle(forward, directionToCamera) - 180f;
            float curveTime = Mathf.Clamp01(angleToCamera / maxRotationSpeed);
            float rotationSpeed = maxRotationSpeed * rotationCurve.Evaluate(curveTime);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        private void OnLeftAction(object sender, EventArgs e)
        {
            //if (!GameManager.Instance.IsInsideMecha) return;
            _leftFiring = true;
        }

        private void OnLeftActionReleased(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsInsideMecha) return;
            _leftFiring = false;
        }

        private void OnRightAction(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsInsideMecha) return;
            _rightFiring = true;
        }

        private void OnRightActionReleased(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsInsideMecha) return;
            _rightFiring = false;
        }

        private void UseProjectile(Vector3 spawnPoint, bool left)
        {
            switch (left)
            {
                case true when _leftArmCooldownLeft > 0:
                case false when _rightArmCooldownLeft > 0:
                    return;
            }

            Ray ray = gameCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, raycastLayerMask) ? hit.point : ray.GetPoint(75);
            Vector3 direction = targetPoint - spawnPoint;
            Projectile projectile = Instantiate(projectilePrefab, spawnPoint, Quaternion.identity).GetComponent<Projectile>();
            projectile.gameObject.transform.forward = direction.normalized;
            projectile.body.AddForce(direction.normalized * projectile.projectileSpeed, ForceMode.Impulse);
            projectile.projectileDamage = left ? inventory.equippedLeftArm.damage : inventory.equippedRightArm.damage;

            if (left) _leftArmCooldownLeft = inventory.equippedLeftArm.cooldown;
            else _rightArmCooldownLeft = inventory.equippedRightArm.cooldown;
        }

        private void UseHitscan(Vector3 spawnPoint, bool left)
        {
            switch (left)
            {
                case true when _leftArmCooldownLeft > 0:
                case false when _rightArmCooldownLeft > 0:
                    return;
            }

            Ray ray = gameCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            if (Physics.Raycast(ray, out RaycastHit hit, hittableLayerMask))
            {
                int damage = left ? inventory.equippedLeftArm.damage : inventory.equippedRightArm.damage;
                hit.collider.gameObject.GetComponent<Hittable>().DoDamage(damage);
            }
            //display muzzle at spawnPoint

            if (left) _leftArmCooldownLeft = inventory.equippedLeftArm.cooldown;
            else _rightArmCooldownLeft = inventory.equippedRightArm.cooldown;
        }

        private void UseMelee(Vector3 spawnPoint, bool left)
        {
            switch (left)
            {
                case true when _leftArmCooldownLeft > 0:
                case false when _rightArmCooldownLeft > 0:
                    return;
            }

            Collider[] cols = Physics.OverlapSphere(spawnPoint, meleeAttackRange, raycastLayerMask);

            foreach (Collider col in cols)
            {
                if (hittableLayerMask.HasLayer(col.gameObject.layer))
                {
                    int damage = left ? inventory.equippedLeftArm.damage : inventory.equippedRightArm.damage;
                    col.gameObject.GetComponent<Hittable>().DoDamage(damage);
                }
            }

            if (left) _leftArmCooldownLeft = inventory.equippedLeftArm.cooldown;
            else _rightArmCooldownLeft = inventory.equippedRightArm.cooldown;
        }

        private void OnJumpAction(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsInsideMecha) return;
            if (!_groundedMecha) return;
            if (_dashing)
                _mechaVelocity.y = Mathf.Sqrt(inventory.equippedLegs.jumpPower *
                                              ((BoostPart)inventory.equippedBonusPart).boostJumpForce *
                                              (medianWeight / currentWeight) * -2f * gravityValue);
            else
                _mechaVelocity.y = Mathf.Sqrt(inventory.equippedLegs.jumpPower * (medianWeight / currentWeight) * -2f *
                                              gravityValue);
        }

        private void OnJumpActionReleased(object sender, EventArgs e)
        {
        }

        private void OnInteractAction(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsInsideMecha) return;
            GameManager.Instance.ExitMecha();
            mechaAnimator.SetBool(Moving, false);
        }

        private void OnDashAction(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsInsideMecha) return;
            if (inventory.equippedBonusPart != null && inventory.equippedBonusPart is BoostPart && currentBoost > 0)
                _dashing = true;
        }

        private void OnDashActionReleased(object sender, EventArgs e)
        {
            if (!GameManager.Instance.IsInsideMecha) return;
            _dashing = false;
        }
    }
}