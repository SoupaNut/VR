using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Game.Utilities;
using Unity.Game.Shared;

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

    [Header("Audio")]
    public AudioClip DamagedSfxClip;
    

    // Patrol
    private bool walkPointSet;
    private Vector3 walkPoint;
    private float patrolTimer;

    private Health m_Health;
    private WeaponController m_WeaponController;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        m_WeaponController = GetComponent<WeaponController>();
    }

    private void OnEnable()
    {
        m_Health = GetComponent<Health>();
        if(m_Health != null )
        {
            m_Health.OnDamage += OnDamage;
            m_Health.OnDie += OnDie;
        }
    }

    private void OnDisable()
    {
        m_Health.OnDamage -= OnDamage;
        m_Health.OnDie -= OnDie;
    }

    private void Start()
    {
        walkPointSet = false;
    }

    private void Update()
    {

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

        m_WeaponController.SetReadyToFire(false);
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
        m_WeaponController.SetReadyToFire(true);

        Following();
    }

    private void OnDamage()
    {
        AudioUtility.CreateSfx(DamagedSfxClip, transform.position, AudioUtility.AudioGroups.DamageTick);
    }

    private void OnDie()
    {
        gameObject.SetActive(false);
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
