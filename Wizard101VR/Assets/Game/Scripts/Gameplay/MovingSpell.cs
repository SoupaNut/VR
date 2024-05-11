using System.Collections.Generic;
using Unity.Game.Shared;
using UnityEngine;
using static Unity.VisualScripting.Member;

//namespace Unity.Game.Gameplay
//{
//    public class MovingSpell : BasicSpell
//    {
//        //public Vector3 MovingAxis;
//        public float Speed = 5f;
//        public float CollisionRadius = 0.2f;
//        public LayerMask CollisionLayer;

//        public ParticleSystem Bolt;
//        public ParticleSystem Trail;
//        public ParticleSystem Explosion;


//        //Vector3 MovingWorldAxis;
//        Vector3 InitialDirection;
//        bool m_HasExploded;
//        public override void Initialize(Transform wandTip)
//        {
//            base.Initialize(wandTip);
//            //MovingWorldAxis = wandTip.TransformDirection(MovingAxis);
//            InitialDirection = wandTip.forward;
//        }
//        // Update is called once per frame
//        void Update()
//        {
//            transform.position += Time.deltaTime * InitialDirection * Speed;
//        }

//        private void FixedUpdate()
//        {
//            if (!m_HasExploded)
//            {
//                Collider[] results = Physics.OverlapSphere(transform.position, CollisionRadius, CollisionLayer);
//                if (results.Length > 0)
//                {
//                    Explode();
//                }
//            }
//        }

//        public void Explode()
//        {
//            m_HasExploded = true;

//            Explosion.Play();
//            Trail.Stop();
//            Bolt.Stop();

//            Destroy(gameObject, 1f);
//        }
//    }

//}

namespace Unity.Game.Gameplay
{
    public class MovingSpell : BasicSpell
    {
        [Header("General")]
        [Tooltip("Radius of this projectile's collision detection")]
        public float Radius = 0.01f;

        [Tooltip("How fast the projectile travels")]
        public float Speed = 10f;

        [Tooltip("How much damage the projectile deals upon impact")]
        public float Damage = 10f;

        [Tooltip("Transform representing the root of the projectile (used for accurate collision detection)")]
        public Transform Root;

        [Tooltip("Transform representing the center of the projectile (used for accurate collision detection)")]
        public Transform Center;

        [Tooltip("Layers this projectile can collide with")]
        public LayerMask HittableLayers = -1;

        [Header("General SFX")]
        [Tooltip("Clip to play upon cast.")]
        public AudioClip OnCastSfx;

        [Header("Impact")]
        [Tooltip("VFX prefab to spawn upon impact")]
        public GameObject ImpactVfx;

        [Tooltip("LifeTime of the VFX before being destroyed")]
        public float ImpactVfxLifetime = 5f;

        [Tooltip("Offset along the hit normal where the VFX will be spawned")]
        public float ImpactVfxSpawnOffset = 0.1f;

        public List<Collider> IgnoredColliders { get; private set; }

        const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

        public override void Cast(SpellcastManager manager)
        {
            base.Cast(manager);

            IgnoredColliders = manager.IgnoredColliders;

            if(OnCastSfx)
            {
                AudioUtility.CreateSfx(OnCastSfx, gameObject, AudioUtility.AudioGroups.Spells, 1f);
            }
                
        }

        void Update()
        {
            transform.position += transform.forward * Speed * Time.deltaTime;
        }

        private void FixedUpdate()
        {
            HandleHitDetection();
        }

        void HandleHitDetection()
        {
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            bool foundHit = false;

            // Spherecast
            Vector3 displacementSinceLastFrame = Center.position - Root.position;
            RaycastHit[] hits = Physics.SphereCastAll(Center.position, Radius, displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers, k_TriggerInteraction);

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

        //void HandleHitDetection()
        //{
        //    Collider[] hitColliders = Physics.OverlapSphere(Center.position, Radius, HittableLayers);

        //    foreach(var hitCollider in hitColliders)
        //    {
        //        if(IsValidHit(hitCollider))
        //        {
        //            Vector3 point = hitCollider.ClosestPoint(transform.position);
        //            Vector3 normal = MathUtility.CalculateNormal(point, hitCollider);

        //            OnHit(point, normal, hitCollider);
        //        }
        //    }
        //}

        void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {
            // Damage
            Health health = collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(Damage, Owner);
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

            Destroy(gameObject);
        }

        bool IsValidHit(RaycastHit hit)
        {
            // ignore hits with an ignore component
            if (hit.collider.GetComponent<IgnoreHitDetection>())
            {
                return false;
            }

            // ignore hits with specific ignored colliders (self colliders, by default)
            if (IgnoredColliders != null && IgnoredColliders.Contains(hit.collider))
            {
                return false;
            }

            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Center.position, Radius);
        }
    }
}