using System;
using _Scripts.Controller;
using _Scripts.Managers;
using _Scripts.MechaParts.SO;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _Scripts.Enemies
{
    public class BasicEnemy : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform player;
        [SerializeField] private LayerMask groundLayer, playerLayer;
        [SerializeField] private int maxHealth;
        [SerializeField] private Animator animator;

        [Header("Patrolling")] [SerializeField]
        private Vector3 walkPoint;

        [SerializeField] private float walkPointRange;

        [Header("Attacking")] [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private bool isRanged;
        [SerializeField] private GameObject projectile;
        [SerializeField] private int damage;
        [SerializeField] private Transform projectileSpawnPointLeft;
        [SerializeField] private Transform projectileSpawnPointRight;

        [Header("States")] [SerializeField] private float sightRange;
        [SerializeField] private float attackRange;

        [Header("Drop Items")] 
        [SerializeField] private GameObject droppedItemPrefab;

        private float _currentHealth;
        private bool _walkPointSet, _alreadyAttacked, _playerInSightRange, _playerInAttackRange;
        private static readonly int Moving = Animator.StringToHash("Moving");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int AttackLeft = Animator.StringToHash("AttackLeft");
        private static readonly int AttackRight = Animator.StringToHash("AttackRight");

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        private void Start()
        {
            if (player == null) player = GameObject.FindGameObjectWithTag("Mecha").transform;
        }

        private void Update()
        {
            _playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            _playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
            if (!_playerInSightRange && !_playerInAttackRange) Patrolling();
            if (_playerInSightRange && !_playerInAttackRange) ChasePlayer();
            if (_playerInSightRange && _playerInAttackRange) AttackPlayer();
        }

        private void Patrolling()
        {
            if (!_walkPointSet) SearchWalkPoint();
            if (_walkPointSet)
            {
                agent.SetDestination(walkPoint);
                animator.SetBool(Moving, true);
            }
            else animator.SetBool(Moving, false);

            Vector3 dstToWalkPoint = transform.position - walkPoint;
            if (dstToWalkPoint.magnitude < 1f) _walkPointSet = false;
        }

        private void SearchWalkPoint()
        {
            float rndZ = Random.Range(-walkPointRange, walkPointRange);
            float rndX = Random.Range(-walkPointRange, walkPointRange);
            Vector3 position = transform.position;
            walkPoint = new Vector3(position.x + rndX, position.y, position.z + rndZ);
            if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
                _walkPointSet = true;
        }

        private void ChasePlayer()
        {
            agent.SetDestination(player.position);
            animator.SetBool(Moving, true);
        }

        private void AttackPlayer()
        {
            agent.SetDestination(transform.position);
            animator.SetBool(Moving, false);
            transform.LookAt(player);
            float originalYRotation = transform.eulerAngles.y;
            transform.eulerAngles = new Vector3(0, originalYRotation, 0);
            if (!_alreadyAttacked)
            {
                int rnd = Random.Range(0, 2);
                Vector3 spawnPoint = rnd == 0 ? projectileSpawnPointLeft.position : projectileSpawnPointRight.position;
                // Attack
                if (isRanged)
                {
                    Rigidbody rb = Instantiate(projectile, spawnPoint, Quaternion.identity)
                        .GetComponent<Rigidbody>();
                    rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                    rb.AddForce(transform.up * 8f, ForceMode.Impulse);
                }
                else
                {
                    MechaController.Instance.DamagePart(damage);
                }

                animator.SetTrigger(rnd == 0 ? AttackLeft : AttackRight);
                _alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }

        private void ResetAttack()
        {
            _alreadyAttacked = false;
        }

        public void TakeDamage(int damageTaken)
        {
            _currentHealth -= damageTaken;
            if (_currentHealth <= 0)
            {
                animator.SetBool(IsDead, true);
                Invoke(nameof(Die), 2);
            }
        }

        public void Die()
        {
            EnemyDrop drop = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity).GetComponent<EnemyDrop>();
            drop.mechPart = PartsManager.Instance.GetRandomPart();
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }
    }
}