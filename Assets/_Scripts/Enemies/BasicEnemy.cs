using System;
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
        
        [Header("Patrolling")]
        [SerializeField] private Vector3 walkPoint;
        [SerializeField] private float walkPointRange;

        [Header("Attacking")] 
        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private GameObject projectile;

        [Header("States")] 
        [SerializeField] private float sightRange;
        [SerializeField] private float attackRange;

        private float _currentHealth;
        private bool walkPointSet, alreadyAttacked, playerInSightRange, playerInAttackRange;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        private void Update()
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
            if (!playerInSightRange && !playerInAttackRange) Patrolling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInSightRange && playerInAttackRange) AttackPlayer();
        }

        private void Patrolling()
        {
            if (!walkPointSet) SearchWalkPoint();
            if (walkPointSet) agent.SetDestination(walkPoint);

            Vector3 dstToWalkPoint = transform.position - walkPoint;
            if (dstToWalkPoint.magnitude < 1f) walkPointSet = false;
        }

        private void SearchWalkPoint()
        {
            float rndZ = Random.Range(-walkPointRange, walkPointRange);
            float rndX = Random.Range(-walkPointRange, walkPointRange);
            Vector3 position = transform.position;
            walkPoint = new Vector3(position.x + rndX, position.y, position.z + rndZ);
            if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
                walkPointSet = true;
        }

        private void ChasePlayer()
        {
            agent.SetDestination(player.position);
        }

        private void AttackPlayer()
        {
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            if (!alreadyAttacked)
            {
                // Attack
                Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                rb.AddForce(transform.up * 8f, ForceMode.Impulse);
                
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }

        private void ResetAttack()
        {
            alreadyAttacked = false;
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0) Destroy(gameObject);
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