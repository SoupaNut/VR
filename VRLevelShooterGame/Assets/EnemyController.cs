using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public GameObject player;
    public LayerMask playerLayer, groundLayer;

    [Header("Ranges")]
    public float walkPointRange = 10f;
    public float sightRange = 20f;
    public float attackRange = 10f;
    private bool playerInAttackRange, playerInSightRange;

    [Header("Movement")]
    public float stoppingDistance = 5f;
    public float patrolCooldown = 1f;

    [Header("Weapon Parameters")]
    public GameObject projectile;
    public Transform weaponAttachPoint;
    public AudioSource weaponSound;
    public float projectileSpeed = 20f;
    public float projectileDespawnTime = 3f;
    public float timeBetweenAttacks = 1f;
    

    // Patrol
    private bool walkPointSet;
    private Vector3 walkPoint;
    private float patrolTimer;

    // Attack
    private bool alreadyAttacked;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        walkPointSet = false;
        alreadyAttacked = false;
    }

    private void Update()
    {
        //UpdateAiStateTransitions();

        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if(playerInSightRange && playerInAttackRange)
        {
            Attacking();
        }
        else if(playerInSightRange && !playerInAttackRange)
        {
            Following();
        }
        else
        {
            // find new patrol walkpoint if timer is 0
            if(patrolTimer <= 0f)
            {
                Patrolling();
                patrolTimer = patrolCooldown;
            }
            // do nothing
            else
            {
                patrolTimer -= Time.deltaTime;
            }
            
        }
    }

    private void Patrolling()
    {
        agent.stoppingDistance = 0f;
        if(!walkPointSet)
        {
            SearchWalkPoint();
        }

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        // calculate random point in range
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        float randomZ = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // check if the walkpoint is on the ground
        if(Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    private void Following()
    {
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(player.transform.position);
    }

    private void Attacking()
    {
        transform.LookAt(player.transform);

        if(!alreadyAttacked)
        {
            Fire();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        Following();
    }

    private void Fire()
    {
        // Shoot projectile at player
        GameObject spawnedProjectile = Instantiate(projectile, weaponAttachPoint.position, weaponAttachPoint.rotation);
        spawnedProjectile.GetComponent<Rigidbody>().velocity = weaponAttachPoint.forward * projectileSpeed;

        // play weapon sound
        weaponSound.Play();

        Destroy(spawnedProjectile, projectileDespawnTime);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
