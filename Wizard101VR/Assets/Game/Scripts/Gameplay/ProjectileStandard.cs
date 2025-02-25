using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Shared;

namespace Unity.Game.Gameplay
{
    public class ProjectileStandard : ProjectileBase
    {
        [Header("General")]
        [Tooltip("Radius of this projectile's collision detection")]
        public float Radius = 0.01f;

        [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
        public Transform Root;

        [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
        public Transform Tip;

        [Tooltip("LifeTime of the projectile")]
        public float MaxLifeTime = 3f;

        [Tooltip("Layers this projectile can collide with")]
        public LayerMask HittableLayers = -1;

        [Header("Audio & Visual")]
        [Tooltip("VFX prefab to spawn upon impact")]
        public GameObject ImpactVfx;

        [Tooltip("LifeTime of the VFX before being destroyed")]
        public float ImpactVfxLifetime = 5f;

        [Tooltip("Offset along the hit normal where the VFX will be spawned")]
        public float ImpactVfxSpawnOffset = 0.1f;

        [Tooltip("Clip to play on impact")]
        public AudioClip ImpactSfxClip;

        Vector3 m_Velocity;
        Vector3 m_CurrentPosition;
        Vector3 m_LastRootPosition;
        float m_Damage;
        ProjectileBase m_ProjectileBase;
        List<Collider> m_IgnoredColliders;

        const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

        void OnEnable()
        {
            m_ProjectileBase = GetComponent<ProjectileBase>();
            DebugUtility.HandleErrorIfNullGetComponent<ProjectileBase, ProjectileStandard>(m_ProjectileBase, this, gameObject);

            m_ProjectileBase.onShoot += OnShoot;

            Destroy(gameObject, MaxLifeTime);
        }

        void OnDisable()
        {
            m_ProjectileBase.onShoot -= OnShoot;
        }

        // Update is called once per frame
        void Update()
        {
            // Move
            {
                m_CurrentPosition += m_Velocity * Time.deltaTime;
                transform.position = m_CurrentPosition;
            }


            // Hit Detection
            {
                RaycastHit closestHit = new RaycastHit();
                closestHit.distance = Mathf.Infinity;
                bool foundHit = false;

                // Spherecast
                Vector3 displacementSinceLastFrame = Tip.position - m_LastRootPosition;
                RaycastHit[] hits = Physics.SphereCastAll(m_LastRootPosition, Radius, displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers, k_TriggerInteraction);

                foreach (var hit in hits)
                {
                    if (IsValidHit(hit) && hit.distance < closestHit.distance)
                    {
                        foundHit = true;
                        closestHit = hit;
                    }
                }

                if (foundHit)
                {
                    // Handle case of casting while already inside a collider
                    if (closestHit.distance <= 0f)
                    {
                        closestHit.point = Root.position;
                        closestHit.normal = -transform.forward;
                    }

                    OnHit(closestHit.point, closestHit.normal, closestHit.collider);
                }
            }

            m_LastRootPosition = Root.position;
        }

        bool IsValidHit(RaycastHit hit)
        {
            // ignore hits with an ignore component
            if (hit.collider.GetComponent<IgnoreHitDetection>())
            {
                return false;
            }

            // ignore hits with specific ignored colliders (self colliders, by default)
            if (m_IgnoredColliders != null && m_IgnoredColliders.Contains(hit.collider))
            {
                return false;
            }

            return true;
        }

        void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {
            // Damage
            Health health = collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(m_Damage, m_ProjectileBase.Owner);
            }

            // Impact Vfx
            if (ImpactVfx)
            {
                GameObject impactVfxInstance = Instantiate(ImpactVfx, point + (normal * ImpactVfxSpawnOffset), Quaternion.LookRotation(normal));
                if (ImpactVfxLifetime > 0)
                {
                    Destroy(impactVfxInstance.gameObject, ImpactVfxLifetime);
                }
            }


            // Impact Sfx
            if (ImpactSfxClip)
            {
                float spatialBlend = 1f;
                AudioUtility.CreateSfx(ImpactSfxClip, point, AudioUtility.AudioGroups.Impact, spatialBlend);
            }

            Destroy(this.gameObject);
        }

        void OnShoot()
        {
            m_LastRootPosition = Root.position;
            m_CurrentPosition = m_ProjectileBase.InitialPosition;
            m_Velocity = m_ProjectileBase.InitialDirection * m_ProjectileBase.Speed;
            m_Damage = m_ProjectileBase.Damage;
            m_IgnoredColliders = new List<Collider>();

            // Get List of owner colliders and add them to ignored colliders list
            Collider[] ownerColliders = m_ProjectileBase.Owner.GetComponentsInChildren<Collider>();
            m_IgnoredColliders.AddRange(ownerColliders);
        }
    }

}
