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
    public float walkPointRange;
    public float sightRange;
    public float attackRange;
    private bool playerInAttackRange, playerInSightRange;

    [Header("Enemy Parameters")]
    public GameObject projectile;
    public float projectileSpeed = 20f;
    public float projectileDespawnTime = 3f;
    public Transform weaponAttachPoint;
    public float timeBetweenAttacks = 1f;
    public AudioSource weaponSound;

    // Patrol
    private bool walkPointSet;
    private Vector3 walkPoint;

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
            Patrolling();
        }
    }

    private void Patrolling()
    {
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
