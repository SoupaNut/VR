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
        [System.Serializable]
        public struct RendererIndexData
        {
            public Renderer Renderer;
            public int MaterialIndex;

            public RendererIndexData(Renderer renderer, int index)
            {
                Renderer = renderer;
                MaterialIndex = index;
            }
        }

        [Header("Parameters")]
        [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
        public float SelfDestructYHeight = -20f;

        [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
        public float PathReachingRadius = 2f;

        [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
        public float DeathDuration = 0f;

        [Tooltip("The speed at which the enemy rotates")]
        public float OrientationSpeed = 10f;

        [Header("Audio")]
        [Tooltip("Sound played when receiving damages")]
        public AudioClip DamagedSfxClip;

        [Header("VFX")]
        [Tooltip("The VFX prefab spawned when the enemy dies")]
        public GameObject DeathVfx;

        [Tooltip("The point at which the death VFX is spawned")]
        public Transform DeathVfxSpawnPoint;

        [Header("Eye color")]
        [Tooltip("Material for the eye color")]
        public Material EyeColorMaterial;

        [Tooltip("The default color of the bot's eye")]
        [ColorUsageAttribute(true, true)]
        public Color DefaultEyeColor;

        [Tooltip("The attack color of the bot's eye")]
        [ColorUsageAttribute(true, true)]
        public Color AttackEyeColor;

        [Header("Flash on hit")]
        [Tooltip("The material used for the body of the hoverbot")]
        public Material BodyMaterial;

        [Tooltip("The gradient representing the color of the flash on hit")]
        [GradientUsageAttribute(true)]
        public Gradient OnHitBodyGradient;

        [Tooltip("The duration of the flash on hit")]
        public float FlashOnHitDuration = 0.5f;

        [Header("Debug Display")]
        [Tooltip("Color of the sphere gizmo representing the path reaching range")]
        public Color PathReachingRangeColor = Color.yellow;

        [Tooltip("Color of the sphere gizmo representing the attack range")]
        public Color AttackRangeColor = Color.red;

        [Tooltip("Color of the sphere gizmo representing the detection range")]
        public Color DetectionRangeColor = Color.blue;

        // - - - - - - - - - - A C T I O N S - - - - - - - - - - //
        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;
        public UnityAction onDamaged;
        public UnityAction onAttack;
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        // - - - - - - - - - - P U B L I C - - - - - - - - - - //
        public NavMeshAgent NavMeshAgent { get; set; }
        public PatrolPath PatrolPath { get; set; }
        public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
        public bool IsTargetInAttackRange => DetectionModule.IsTargetInAttackRange;
        public bool IsSeeingTarget => DetectionModule.IsSeeingTarget;
        public bool HadKnownTarget => DetectionModule.HadKnownTarget;
        public DetectionModule DetectionModule { get; set; }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        // - - - - - - - - - - P R I V A T E - - - - - - - - - - //
        int m_DestinationPathNodeIndex;
        Collider[] m_SelfColliders;
        Health m_Health;
        Actor m_Actor;
        WeaponController m_WeaponController;

        List<RendererIndexData> m_BodyRenderers = new List<RendererIndexData>();
        MaterialPropertyBlock m_BodyFlashMaterialPropertyBlock;
        float m_LastTimeDamaged = Mathf.NegativeInfinity;

        RendererIndexData m_EyeRendererData;
        MaterialPropertyBlock m_EyeColorMaterialPropertyBlock;
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

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

                foreach (var renderer in GetComponentsInChildren<Renderer>(true))
                {
                    for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                    {
                        if (renderer.sharedMaterials[i] == EyeColorMaterial)
                        {
                            m_EyeRendererData = new RendererIndexData(renderer, i);
                        }

                        if (renderer.sharedMaterials[i] == BodyMaterial)
                        {
                            m_BodyRenderers.Add(new RendererIndexData(renderer, i));
                        }
                    }
                }

                m_BodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();

                // Check if we have an eye renderer for this enemy
                if (m_EyeRendererData.Renderer != null)
                {
                    m_EyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
                    m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
                    m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock, m_EyeRendererData.MaterialIndex);
                }
            }
            
            // Subscribe to Events
            {
                m_Health.onDamage += OnDamage;
                m_Health.onDie += OnDie;
                DetectionModule.onDetectTarget += OnDetectTarget;
                DetectionModule.onLostTarget += OnLostTarget;
            }

            // Variables
            {
                m_WeaponController.Owner = gameObject;
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
            EnsureIsWithinLevelBounds();

            DetectionModule.HandleTargetDetection(m_Actor, m_SelfColliders);

            // flash body on hit
            Color currentColor = OnHitBodyGradient.Evaluate((Time.time - m_LastTimeDamaged) / FlashOnHitDuration);
            m_BodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);
            foreach (var data in m_BodyRenderers)
            {
                data.Renderer.SetPropertyBlock(m_BodyFlashMaterialPropertyBlock, data.MaterialIndex);
            }
        }

        /**************************************************************************
         * FUNCTION: EnsureIsWithinLevelBounds
         * 
         * PARAM: NA
         * 
         * PURPOSE: at every frame, this tests for conditions to kill the enemy
         * 
         * RETURN: NA
         **************************************************************************/
        private void EnsureIsWithinLevelBounds()
        {
            if (transform.position.y < SelfDestructYHeight)
            {
                Destroy(gameObject);
                return;
            }
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

        /**************************************************************************
         * FUNCTION: OrientTowards
         * 
         * PARAM: (Vector3) lookPosition - position to orient towards
         * 
         * PURPOSE: rotates self towards lookPosition with an angular speed of OrientationSpeed
         * 
         * RETURN: NA
         **************************************************************************/
        public void OrientTowards(Vector3 lookPosition)
        {
            Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - transform.position, Vector3.up).normalized;
            if (lookDirection.sqrMagnitude != 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
            }
        }

        public void TryAttack()
        {
            if (m_WeaponController.HandleShootInputs(false, true, false);)
            {
                onAttack?.Invoke();
            }
        }

        private void OnDamage(float damage, GameObject damageSource)
        {
            // Make sure the damage source is the player
            if(damageSource && !damageSource.GetComponent<EnemyController>()) 
            {
                DetectionModule.OnDamaged(damageSource);

                // play damaged sound
                if (DamagedSfxClip)
                {
                    AudioUtility.CreateSfx(DamagedSfxClip, transform.position, AudioUtility.AudioGroups.DamageTick);
                }

                onDamaged?.Invoke();
                m_LastTimeDamaged = Time.time;
            }
        }

        private void OnDie()
        {
            // spawn a particle system when dying
            if(DeathVfx)
            {
                var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
                Destroy(vfx, 5f);
            }
            
            Destroy(gameObject, DeathDuration);

        }

        private void OnDetectTarget()
        {
            onDetectedTarget?.Invoke();

            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", AttackEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock, m_EyeRendererData.MaterialIndex);
            }
        }

        private void OnLostTarget()
        {
            onLostTarget?.Invoke();

            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock, m_EyeRendererData.MaterialIndex);
            }
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

