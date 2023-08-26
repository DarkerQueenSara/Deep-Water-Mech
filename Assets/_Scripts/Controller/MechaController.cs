using System;
using System.Collections.Generic;
using _Scripts.Combat;
using _Scripts.Managers;
using _Scripts.MechaParts;
using _Scripts.MechaParts.SO;
using Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Controller
{
    public class MechaController : MonoBehaviour
    {
        public static MechaController Instance { get; private set; }
        public int medianWeight;
        [HideInInspector] public float currentBoost, currentSpeed;
        [HideInInspector] public int maxHp, currentHp, currentWeight, maxBoost;

        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private LayerMask raycastLayerMask;
        [SerializeField] private LayerMask hittableLayerMask;
        [SerializeField] private int meleeAttackRange;
        [SerializeField] private AnimationCurve rotationCurve;
        [SerializeField] private float maxRotationSpeed = 180f;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Animator mechaAnimator;

        [Header("Inventory SO")] [SerializeField]
        private Inventory inventory;

        [Header("Mech Parts")] [SerializeField]
        private InteractablePart[] leftArmsList;
        [SerializeField] private InteractablePart[] rightArmsList;
        [SerializeField] private InteractablePart[] leftLegsList;
        [SerializeField] private InteractablePart[] rightLegsList;
        [SerializeField] private InteractablePart[] torsosList;
        [SerializeField] private InteractablePart[] bonusPartsList;

        [SerializeField] private InteractablePart leftArmPart;
        [SerializeField] private InteractablePart rightArmPart;
        [SerializeField] private InteractablePart leftLegPart;
        [SerializeField] private InteractablePart rightLegPart;
        [SerializeField] private InteractablePart torsoPart;
        [SerializeField] private InteractablePart bonusPart;

        [Header("Mech Projectiles Spawns")] [SerializeField]
        private Transform rightArmSpawnPoint;

        [SerializeField] private Transform leftArmSpawnPoint;

        private static readonly int Moving = Animator.StringToHash("Moving");
        private CharacterController _controller;
        private InputManager _inputManager;
        private Vector3 _mechaVelocity, _move, _lastPos;
        private bool _groundedMecha, _leftFiring, _rightFiring, _dashing;
        private float _leftArmCooldownLeft, _rightArmCooldownLeft, _angleToCamera;

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
            int newMaxHp = GetMaxHp();
            maxHp = newMaxHp;
            currentWeight = GetWeight();
            CalculateCurrentHealth();
            Debug.Log("The mech weighs " + currentWeight + "kg, and has " + currentHp + "/" + maxHp + " HP.");

            leftArmPart = FindInteractablePartByMechPart(inventory.equippedLeftArm, leftArmsList);
            rightArmPart = FindInteractablePartByMechPart(inventory.equippedRightArm, rightArmsList);
            leftLegPart = FindInteractablePartByMechPart(inventory.equippedLegs, leftLegsList);
            rightLegPart = FindInteractablePartByMechPart(inventory.equippedLegs, rightLegsList);
            torsoPart = FindInteractablePartByMechPart(inventory.equippedTorso, torsosList);
            bonusPart = FindInteractablePartByMechPart(inventory.equippedBonusPart, bonusPartsList);
        }

        private InteractablePart FindInteractablePartByMechPart(MechPart targetMechPart, IEnumerable<InteractablePart> partsList)
        {
            foreach (InteractablePart interactablePart in partsList)
                if (interactablePart.mechPart == targetMechPart)
                    return interactablePart;

            return null;
        }

        public void DamagePart(int damage)
        {
            InteractablePart[] bodyParts = { leftArmPart, rightArmPart, leftLegPart, rightLegPart, torsoPart };
            if (bonusPart != null)
            {
                System.Array.Resize(ref bodyParts, bodyParts.Length + 1);
                bodyParts[^1] = bonusPart;
            }

            int numComponents = bodyParts.Length;
            int componentsTakingDamage = Random.Range(1, numComponents + 1);
            int damagePerComponent = damage / componentsTakingDamage;
            for (int i = 0; i < componentsTakingDamage; i++)
            {
                int randomIndex = Random.Range(0, numComponents);
                InteractablePart part = bodyParts[randomIndex];
                part.currentHp = Mathf.Max(part.currentHp - damagePerComponent, 0);
            }

            CalculateCurrentHealth();
        }

        public void CalculateCurrentHealth()
        {
            float hpLossLeftArm = 1.0f * leftArmPart.currentHp / leftArmPart.mechPart.hp;
            float hpLossRightArm = 1.0f * rightArmPart.currentHp / rightArmPart.mechPart.hp;
            float hpLossLeftLeg = 1.0f * leftLegPart.currentHp / leftLegPart.mechPart.hp;
            float hpLossRightLeg = 1.0f * rightLegPart.currentHp / rightLegPart.mechPart.hp;
            float hpLossTorso = 1.0f * torsoPart.currentHp / torsoPart.mechPart.hp;

            if (bonusPart != null)
            {
                float hpLossBonusPart = 1.0f * bonusPart.currentHp / bonusPart.mechPart.hp;
                bonusPart.currentHp = Mathf.RoundToInt(inventory.equippedBonusPart.hp * hpLossBonusPart);
            }

            leftArmPart.currentHp = Mathf.RoundToInt(inventory.equippedLeftArm.hp * hpLossLeftArm);
            rightArmPart.currentHp = Mathf.RoundToInt(inventory.equippedRightArm.hp * hpLossRightArm);
            leftLegPart.currentHp = Mathf.RoundToInt(inventory.equippedLegs.hp * hpLossLeftLeg);
            rightLegPart.currentHp = Mathf.RoundToInt(inventory.equippedLegs.hp * hpLossRightLeg);
            torsoPart.currentHp = Mathf.RoundToInt(inventory.equippedTorso.hp * hpLossTorso);

            currentHp = leftArmPart.currentHp + rightArmPart.currentHp + leftLegPart.currentHp + rightLegPart.currentHp +
                        torsoPart.currentHp;
            currentHp += bonusPart != null ? bonusPart.currentHp : 0;
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
            int aux = inventory.equippedTorso.hp + inventory.equippedLeftArm.hp +
                      inventory.equippedRightArm.hp + inventory.equippedLegs.hp + inventory.equippedLegs.hp;
            aux = inventory.equippedBonusPart != null ? aux + inventory.equippedBonusPart.hp : aux;
            return aux;
        }

        public int GetMedianWeight()
        {
            BonusPart part = inventory.equippedBonusPart;
            if (part != null && part is LighteningPart lPart)
            {
                return medianWeight - lPart.weightLimitReduction;
            }

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
            _mechaVelocity.y += gravityValue * Time.deltaTime;
            _controller.Move(_mechaVelocity * Time.deltaTime);

            mechaAnimator.SetBool(Moving, inputVector.magnitude != 0 || _angleToCamera < -5f);
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
                        UseProjectile(leftArmSpawnPoint.position, true);
                        break;
                    case ArmType.HITSCAN:
                        UseHitscan(leftArmSpawnPoint.position, true);
                        break;
                    case ArmType.MELEE:
                        UseMelee(leftArmSpawnPoint.position, true);
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
                        UseProjectile(rightArmSpawnPoint.position, false);
                        break;
                    case ArmType.HITSCAN:
                        UseHitscan(rightArmSpawnPoint.position, false);
                        break;
                    case ArmType.MELEE:
                        UseMelee(rightArmSpawnPoint.position, false);
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
            _angleToCamera = Vector3.Angle(forward, directionToCamera) - 180f;
            float curveTime = Mathf.Clamp01(_angleToCamera / maxRotationSpeed);
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

            float weightModifier = currentWeight <= medianWeight ? 1 : 1.0f * medianWeight / currentWeight;

            if (_dashing)
                _mechaVelocity.y = Mathf.Sqrt(inventory.equippedLegs.jumpPower *
                                              ((BoostPart)inventory.equippedBonusPart).boostJumpForce *
                                              weightModifier * -2f * gravityValue);
            else
                _mechaVelocity.y = Mathf.Sqrt(inventory.equippedLegs.jumpPower * weightModifier * -2f *
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