using UnityEngine;
using UnityEngine.Events;
using Unity.Game.Shared;

namespace Unity.Game.AI.Enemy
{
    using RendererData = TypesUtility.RendererIndexData;

    [RequireComponent(typeof(Health), typeof(Actor))]
    public class EnemyController : BaseEntityController
    {
        [Header("General")]
        [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
        public float SelfDestructYHeight = -20f;

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

        [Header("Debug Display")]
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
        public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
        public bool IsTargetInAttackRange => DetectionModule.IsTargetInAttackRange;
        public bool IsSeeingTarget => DetectionModule.IsSeeingTarget;
        public bool HadKnownTarget => DetectionModule.HadKnownTarget;
        public DetectionModule DetectionModule { get; set; }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        // - - - - - - - - - - P R I V A T E - - - - - - - - - - //
        Collider[] m_SelfColliders;
        Health m_Health;
        Actor m_Actor;
        WeaponController m_WeaponController;

        RendererData m_EyeRendererData;
        MaterialPropertyBlock m_EyeColorMaterialPropertyBlock;
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        protected override void Start()
        {
            base.Start();

            // ************** 
            // Get Components 
            // ************** 
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
                        m_EyeRendererData = new RendererData(renderer, i);
                    }
                }
            }

            // Check if we have an eye renderer for this enemy
            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock, m_EyeRendererData.MaterialIndex);
            }


            // ************** 
            // Subscribe to Events
            // ************** 
            m_Health.onDamage += OnDamage;
            m_Health.onDie += OnDie;
            DetectionModule.onDetectTarget += OnDetectTarget;
            DetectionModule.onLostTarget += OnLostTarget;


            // ************** 
            // Variables
            // ************** 
            m_WeaponController.Owner = gameObject;
        }

        void OnDisable()
        {
            // Unsubscribe to events
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

        void Update()
        {
            EnsureIsWithinLevelBounds();

            DetectionModule.HandleTargetDetection(m_Actor, m_SelfColliders);
        }

        void EnsureIsWithinLevelBounds()
        {
            if (transform.position.y < SelfDestructYHeight)
            {
                Destroy(gameObject);
                return;
            }
        }

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
            if (m_WeaponController.HandleShootInputs(false, true, false))
            {
                onAttack?.Invoke();
            }
        }

        void OnDamage(float damage, GameObject damageSource)
        {
            // Make sure the damage source is the player
            if (damageSource && !damageSource.GetComponent<EnemyController>())
            {
                DetectionModule.OnDamaged(damageSource);

                // play damaged sound
                if (DamagedSfxClip)
                {
                    AudioUtility.CreateSfx(DamagedSfxClip, transform.position, AudioUtility.AudioGroups.DamageTick);
                }

                onDamaged?.Invoke();
            }
        }

        void OnDie()
        {
            // spawn a particle system when dying
            if (DeathVfx)
            {
                var vfx = Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
                Destroy(vfx, 5f);
            }

            Destroy(gameObject, DeathDuration);

        }

        void OnDetectTarget()
        {
            onDetectedTarget?.Invoke();

            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", AttackEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock, m_EyeRendererData.MaterialIndex);
            }
        }

        void OnLostTarget()
        {
            onLostTarget?.Invoke();

            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock, m_EyeRendererData.MaterialIndex);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

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