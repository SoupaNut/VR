using Unity.Game.Shared;
using UnityEngine;

namespace Unity.Game.Gameplay
{
    public class MovingSpell : BasicSpell
    {
        [Header("Movement")]
        [Tooltip("How fast the projectile travels")]
        public float Speed = 10f;

        [Header("Hit Detection")]
        [Tooltip("Radius of this projectile's collision detection")]
        public float DetectionRadius = 0.01f;

        [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
        public Transform Root;

        [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
        public Transform Tip;

        [Tooltip("Layers this projectile can collide with")]
        public LayerMask HittableLayers = -1;

        [Tooltip("How long the projectile will remain in the scene once it hits something. Having this non-zero will help play any extra animations.")]
        public float OnHitLifetime;

        [Header("Impact")]
        [Tooltip("VFX prefab to spawn upon impact")]
        public GameObject ImpactVfx;

        [Tooltip("LifeTime of the VFX before being destroyed")]
        public float ImpactVfxLifetime = 5f;

        [Tooltip("Offset along the hit normal where the VFX will be spawned")]
        public float ImpactVfxSpawnOffset = 0.1f;

        [Header("Debug")]
        [Tooltip("Visually show the sphere collider")]
        public bool DisplayColliderRadius;

        [Tooltip("Color of the collider shown")]
        public Color ColliderColor = Color.green;

        public bool HasHit { get; set; }

        BasicSpell m_BasicSpell;

        const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

        protected virtual void Awake()
        {
            m_BasicSpell = GetComponent<BasicSpell>();
            DebugUtility.HandleErrorIfNullGetComponent<BasicSpell, MovingSpell>(m_BasicSpell, this, gameObject);
        }

        public override void Cast(SpellcastManager manager)
        {
            base.Cast(manager);
            HasHit = false;
        }

        protected virtual void Update()
        {
            // keep moving if we haven't hit
            if (!HasHit)
            {
                transform.position += transform.forward * Speed * Time.deltaTime;
                HandleHitDetection();
            }
        }

        public virtual void HandleHitDetection()
        {
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            // Spherecast
            Vector3 displacementSinceLastFrame = Tip.position - Root.position;
            RaycastHit[] hits = Physics.SphereCastAll(Root.position, DetectionRadius, displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers, k_TriggerInteraction);

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
                HasHit = true;
                // Handle case of casting while already inside a collider
                if (closestHit.distance <= 0f)
                {
                    closestHit.point = Root.position;
                    closestHit.normal = -transform.forward;
                }

                OnHit(closestHit.point, closestHit.normal, closestHit.collider);
            }
        }

        public virtual void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {
            // Damage
            Health health = collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(m_BasicSpell.Damage, m_BasicSpell.Owner);
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

            Destroy(gameObject, OnHitLifetime);
        }

        public virtual bool IsValidHit(RaycastHit hit)
        {
            // ignore hits with an ignore component
            if (hit.collider.GetComponent<IgnoreHitDetection>())
            {
                return false;
            }

            // ignore hits with specific ignored colliders (self colliders, by default)
            if (m_BasicSpell.IgnoredColliders != null && m_BasicSpell.IgnoredColliders.Contains(hit.collider))
            {
                return false;
            }

            return true;
        }

        protected virtual void OnDrawGizmos()
        {
            if (DisplayColliderRadius)
            {
                Gizmos.color = ColliderColor;
                Gizmos.DrawWireSphere(Tip.position, DetectionRadius);
            }
        }
    }
}