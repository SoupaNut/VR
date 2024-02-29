using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Unity.Game.Utilities;
using Unity.Game.Shared;

namespace Unity.Game.AI
{
    [RequireComponent(typeof(Health), typeof(Actor), typeof(NavMeshAgent))]
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

        [Header("Parameters")]
        [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
        public float PathReachingRadius = 2f;

        [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
        public float DeathDuration = 0f;

        [Tooltip("The speed at which the enemy rotates")]
        public float OrientationSpeed = 10f;

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

        [Header("Debug Display")]
        [Tooltip("Color of the sphere gizmo representing the path reaching range")]
        public Color PathReachingRangeColor = Color.yellow;

        [Tooltip("Color of the sphere gizmo representing the attack range")]
        public Color AttackRangeColor = Color.red;

        [Tooltip("Color of the sphere gizmo representing the detection range")]
        public Color DetectionRangeColor = Color.blue;



        // Actions
        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;
        public UnityAction onDamaged;

        // Movement
        public PatrolPath PatrolPath { get; set; }
        public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
        public bool IsTargetInAttackRange => DetectionModule.IsTargetInAttackRange;
        public bool IsSeeingTarget => DetectionModule.IsSeeingTarget;
        public bool HadKnownTarget => DetectionModule.HadKnownTarget;
        public DetectionModule DetectionModule;
        int m_DestinationPathNodeIndex;

        private Collider[] m_SelfColliders;

        private Health m_Health;
        private Actor m_Actor;
        private WeaponController m_WeaponController;

        private void Start()
        {
            // Get components
            {
                NavMeshAgent = GetComponent<NavMeshAgent>();
                DebugUtility.HandleErrorIfNullGetComponent<NavMeshAgent, EnemyController>(NavMeshAgent, this, gameObject);

                m_WeaponController = GetComponent<WeaponController>();
                DebugUtility.HandleErrorIfNullGetComponent<WeaponController, EnemyController>(m_WeaponController, this, gameObject);

                m_Health = GetComponent<Health>();
                DebugUtility.HandleErrorIfNullGetComponent<Health, EnemyController>(m_Health, this, gameObject);

                m_Actor = GetComponent<Actor>();
                DebugUtility.HandleErrorIfNullGetComponent<Actor, EnemyController>(m_Actor, this, gameObject);

                DetectionModule = GetComponentInChildren<DetectionModule>();
                DebugUtility.HandleErrorIfNullGetComponent<DetectionModule, EnemyController>(DetectionModule, this, gameObject);

                m_SelfColliders = GetComponentsInChildren<Collider>();
            }
            
            // Subscribe to Events
            {
                m_Health.onDamage += OnDamage;
                m_Health.onDie += OnDie;
                DetectionModule.onDetectTarget += OnDetectTarget;
                DetectionModule.onLostTarget += OnLostTarget;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe to events
            {
                if (m_Health != null)
                {
                    m_Health.onDamage -= OnDamage;
                    m_Health.onDie -= OnDie;
                }

                if (DetectionModule != null)
                {
                    DetectionModule.onDetectTarget -= OnDetectTarget;
                    DetectionModule.onLostTarget -= OnLostTarget;
                }
            }
        }

        private void Update()
        {
            DetectionModule.HandleTargetDetection(m_Actor, m_SelfColliders);
            // Check for sight and attack range
            //m_PlayerInSightRange = Physics.CheckSphere(transform.position, SightRange, PlayerLayer);
            //m_PlayerInAttackRange = Physics.CheckSphere(transform.position, AttackRange, PlayerLayer);

            //if (m_PlayerInSightRange && m_PlayerInAttackRange)
            //{
            //    Attacking();
            //}
            //else if (m_PlayerInSightRange && !m_PlayerInAttackRange)
            //{
            //    Following();
            //}
            //else
            //{
            //    // find new patrol walkpoint if timer is 0
            //    if (m_PatrolTimer <= 0f)
            //    {
            //        Patrolling();
            //        m_PatrolTimer = PatrolCooldown;
            //    }
            //    // do nothing
            //    else
            //    {
            //        m_PatrolTimer -= Time.deltaTime;
            //    }
            //}
        }
        /**************************************************************************
         * FUNCTION: IsPathValid
         * 
         * PARAM: NA
         * 
         * PURPOSE: Helper function. Checks if there is a valid path
         * 
         * RETURN: NA
         **************************************************************************/
        private bool IsPathValid()
        {
            return PatrolPath && PatrolPath.PathNodes.Count > 0;
        }

        /**************************************************************************
         * FUNCTION: UpdatePathDestination
         * 
         * PARAM: (bool) inverseOrder - go in reverse order of path nodes
         * 
         * PURPOSE: Go to the next path node
         * 
         * RETURN: NA
         **************************************************************************/
        public void UpdatePathDestination(bool inverseOrder=false)
        {
            if (IsPathValid())
            {
                // Check if reached the path destination
                if((transform.position - GetPositionOfDestination()).magnitude <= PathReachingRadius)
                {
                    // increment path destination index
                    m_DestinationPathNodeIndex = inverseOrder ? (m_DestinationPathNodeIndex - 1) : (m_DestinationPathNodeIndex + 1);
                    if(m_DestinationPathNodeIndex < 0)
                    {
                        m_DestinationPathNodeIndex = PatrolPath.PathNodes.Count - 1;
                    }

                    if (m_DestinationPathNodeIndex >= PatrolPath.PathNodes.Count)
                    {

                        m_DestinationPathNodeIndex = 0;
                    }
                }
            }
        }

        /**************************************************************************
         * FUNCTION: SetDestinationToClosestPathNode
         * 
         * PARAM: NA
         * 
         * PURPOSE: Sets the path node index to the closest node found
         * 
         * RETURN: NA
         **************************************************************************/
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

        /*************************************************************************
         * FUNCTION: GetPositionOfDestination
         * 
         * PARAM: NA
         * 
         * PURPOSE: Gets the position of the destination path node
         * 
         * RETURN: (Vector3) Position of Path Node
         **************************************************************************/
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

        /**************************************************************************
         * FUNCTION: SetPathDestination
         * 
         * PARAM: (Vector3) destination - position of destination
         * 
         * PURPOSE: Sets the destination of the nav mesh agent
         * 
         * RETURN: NA
         **************************************************************************/
        public void SetPathDestination(Vector3 destination)
        {
            if(NavMeshAgent)
            {
                NavMeshAgent.SetDestination(destination);
            }
        }

        public void OrientTowards(Vector3 lookPosition)
        {
            //GameObject lookObject = new GameObject();
            //lookObject.transform.position = lookPosition;
            //transform.LookAt(lookPosition);
            Quaternion targetRotation = Quaternion.LookRotation(lookPosition - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, OrientationSpeed * Time.deltaTime);
        }

        //private void Following()
        //{
        //    NavMeshAgent.stoppingDistance = StoppingDistance;
        //    SetPathDestination(Player.transform.position);
        //}

        //private void Attacking()
        //{
        //    transform.LookAt(Player.transform);
        //    m_WeaponController.SetReadyToFire(true);

        //    Following();
        //}

        private void OnDamage(float damage, GameObject damageSource)
        {
            AudioUtility.CreateSfx(DamagedSfxClip, transform.position, AudioUtility.AudioGroups.DamageTick);

            DetectionModule.OnDamaged(damageSource);
        }

        private void OnDie()
        {
            // spawn a particle system when dying
            var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
            Destroy(vfx, 5f);

            Destroy(gameObject, DeathDuration);

        }

        private void OnDetectTarget()
        {
            onDetectedTarget?.Invoke();
        }

        private void OnLostTarget()
        {
            onLostTarget?.Invoke();
        }

        void OnDrawGizmos()
        {
            // Path reaching range
            Gizmos.color = PathReachingRangeColor;
            Gizmos.DrawWireSphere(transform.position, PathReachingRadius);

            if (DetectionModule != null)
            {
                // Detection range
                Gizmos.color = DetectionRangeColor;
                Gizmos.DrawWireSphere(transform.position, DetectionModule.DetectionRange);

                // Attack range
                Gizmos.color = AttackRangeColor;
                Gizmos.DrawWireSphere(transform.position, DetectionModule.AttackRange);
            }
        }
    }
}

