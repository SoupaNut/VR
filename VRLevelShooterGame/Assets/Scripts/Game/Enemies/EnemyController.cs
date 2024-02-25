using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Unity.Game.Utilities;
using Unity.Game.Shared;

namespace Unity.Game.AI
{
    public class EnemyController : MonoBehaviour
    {
        public enum AIState
        {
            Patrol,
            Follow,
            Attack
        }
        public AIState AiState;

        [Header("References")]
        public NavMeshAgent NavMeshAgent;
        public GameObject Player;
        public LayerMask PlayerLayer, GroundLayer;

        [Header("Ranges")]
        public float PatrolRange = 10f;
        public float SightRange = 20f;
        public float AttackRange = 10f;
        private bool m_PlayerInAttackRange, m_PlayerInSightRange;

        [Header("Movement")]
        public bool PatrolWhenIdle = true;
        public float StoppingDistance = 5f;
        public float PatrolCooldown = 1f;

        [Header("Audio")]
        [Tooltip("Sound played when receiving damages")]
        public AudioClip DamagedSfxClip;

        [Header("VFX")]
        [Tooltip("The VFX prefab spawned when the enemy dies")]
        public GameObject DeathVfx;

        [Tooltip("The point at which the death VFX is spawned")]
        public Transform DeathVfxSpawnPoint;

        [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
        public float DeathDuration = 0f;

        // Actions
        public UnityAction OnDetectedTarget;
        public UnityAction OnLostTarget;
        public UnityAction OnDamaged;

        // Movement
        public PatrolPath PatrolPath { get; set; }
        public bool IsTargetInAttackRange => m_DetectionModule.IsTargetInAttackRange;
        public bool IsSeeingTarget => m_DetectionModule.IsSeeingTarget;
        public bool HadKnownTarget => m_DetectionModule.HadKnownTarget;
        private DetectionModule m_DetectionModule;
        private int m_DestinationPathNodeIndex;

        private Health m_Health;
        private WeaponController m_WeaponController;

        // Patrol
        private bool m_WalkPointSet;
        private Vector3 m_WalkPoint;
        private float m_PatrolTimer;
        private Vector3 m_InitialPosition;

        private void Start()
        {
            // Get required components
            {
                NavMeshAgent = GetComponent<NavMeshAgent>();
                m_WeaponController = GetComponent<WeaponController>();
            }
            
            // Suscribe to Events
            {
                m_Health = GetComponent<Health>();
                if (m_Health != null)
                {
                    m_Health.OnDamage += OnDamage;
                    m_Health.OnDie += OnDie;
                }

                m_DetectionModule = GetComponentInChildren<DetectionModule>();
                if (m_DetectionModule != null)
                {
                    m_DetectionModule.OnDetectTarget += OnDetect;
                    m_DetectionModule.OnLostTarget += OnLost;
                }
            }

            // Set starting defaults
            {
                m_WalkPointSet = false;
                m_InitialPosition = transform.position;
                AiState = AIState.Patrol;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe to events
            {
                if (m_Health != null)
                {
                    m_Health.OnDamage -= OnDamage;
                    m_Health.OnDie -= OnDie;
                }

                if (m_DetectionModule != null)
                {
                    m_DetectionModule.OnDetectTarget -= OnDetect;
                    m_DetectionModule.OnLostTarget -= OnLost;
                }
            }
        }

        private void Update()
        {

            // Check for sight and attack range
            m_PlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, PlayerLayer);
            m_PlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, PlayerLayer);

            if (m_PlayerInSightRange && m_PlayerInAttackRange)
            {
                Attacking();
            }
            else if (m_PlayerInSightRange && !m_PlayerInAttackRange)
            {
                Following();
            }
            else
            {
                // find new patrol walkpoint if timer is 0
                if (m_PatrolTimer <= 0f)
                {
                    Patrolling();
                    m_PatrolTimer = PatrolCooldown;
                }
                // do nothing
                else
                {
                    m_PatrolTimer -= Time.deltaTime;
                }
            }
        }

        private bool IsPathValid()
        {
            return PatrolPath && PatrolPath.PathNodes.Count > 0;
        }

        private void UpdatePathDestination()
        {
            //// calculate random point in Patrol Range
            //float randomX = Random.Range(-PatrolRange, PatrolRange);
            //float randomZ = Random.Range(-PatrolRange, PatrolRange);

            //m_WalkPoint = new Vector3(m_InitialPosition.x + randomX, m_InitialPosition.y, m_InitialPosition.z + randomZ);

            //// Check if destination is on the ground
            //if(Physics.Raycast(m_WalkPoint, -transform.up, 2f, GroundLayer))
            //{
            //    m_WalkPointSet = true;
            //}

            float randomX = Random.Range(-PatrolRange, PatrolRange);
            float randomZ = Random.Range(-PatrolRange, PatrolRange);

            m_WalkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            // check if the walkpoint is on the ground
            if (Physics.Raycast(m_WalkPoint, -transform.up, 2f, GroundLayer))
            {
                m_WalkPointSet = true;
            }
        }
        //
        // Sets the path node index to the closest node found
        //
        public void SetDestinationToClosestPathNode()
        {
            if(IsPathValid())
            {
                int closestPathNodeIndex = 0;
                for(int i = 0; i < PatrolPath.PathNodes.Count; i++)
                {
                    float distanceToNode = PatrolPath.GetDistanceToNode(transform.position, i);
                    if(distanceToNode < PatrolPath.GetDistanceToNode(transform.position, closestPathNodeIndex))
                    {
                        closestPathNodeIndex = i;
                    }
                }

                m_DestinationPathNodeIndex = closestPathNodeIndex;
            }
            else
            {
                m_DestinationPathNodeIndex = 0;
            }
        }
        //
        // Returns the position vector of the destination 
        //
        public Vector3 GetPositionOfDestination()
        {
            if(IsPathValid())
            {
                return PatrolPath.GetPositionOfPathNode(m_DestinationPathNodeIndex);
            }
            else
            {
                return transform.position;
            }
        }
        //
        // Sets the destination of the nav mesh agent
        //
        private void SetPathDestination(Vector3 destination)
        {
            if(NavMeshAgent)
            {
                NavMeshAgent.SetDestination(destination);
            }
        }

        private void Patrolling()
        {
            NavMeshAgent.stoppingDistance = 0f;
            if (!m_WalkPointSet)
            {
                UpdatePathDestination();
            }

            if (m_WalkPointSet)
            {
                SetPathDestination(m_WalkPoint);
            }

            Vector3 distanceToWalkPoint = transform.position - m_WalkPoint;

            if (distanceToWalkPoint.magnitude < 1f)
            {
                m_WalkPointSet = false;
            }

            m_WeaponController.SetReadyToFire(false);
        }

        private void Following()
        {
            NavMeshAgent.stoppingDistance = StoppingDistance;
            SetPathDestination(Player.transform.position);
        }

        private void Attacking()
        {
            transform.LookAt(Player.transform);
            m_WeaponController.SetReadyToFire(true);

            Following();
        }

        private void OnDamage()
        {
            AudioUtility.CreateSfx(DamagedSfxClip, transform.position, AudioUtility.AudioGroups.DamageTick);
        }

        private void OnDie()
        {
            // spawn a particle system when dying
            var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
            Destroy(vfx, 5f);

            Destroy(gameObject, DeathDuration);

        }

        private void OnDetect()
        {

        }

        private void OnLost()
        {

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, PatrolRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, SightRange);
        }
    }
}

