using System.Collections.Generic;
using Unity.Game.Shared;
using UnityEngine;

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

        [Tooltip("Transform representing the center of the projectile (used for accurate collision detection)")]
        public Transform Center;

        [Tooltip("Layers this projectile can collide with")]
        public LayerMask HittableLayers = -1;

        public List<Collider> IgnoredColliders { get; private set; }
        bool m_HasExploded = false;

        public override void Cast(SpellcastManager manager)
        {
            base.Cast(manager);

            IgnoredColliders = manager.IgnoredColliders;
        }

        void Update()
        {
            transform.position += transform.forward * Speed * Time.deltaTime;
        }

        void FixedUpdate()
        {
            HandleHitDetection();
        }

        void HandleHitDetection()
        {
            if (!m_HasExploded)
            {
                Collider[] results = Physics.OverlapSphere(transform.position, Radius, HittableLayers);
                
                foreach(Collider result in results)
                {
                    if (IsValidHit(result))
                    {
                        Debug.Log(result.gameObject.name);
                        OnHit();
                    }
                }
            }
        }

        void OnHit()
        {
            Destroy(gameObject);
        }

        bool IsValidHit(Collider hit)
        {
            // ignore hits with an ignore component
            if (hit.GetComponent<Collider>().GetComponent<IgnoreHitDetection>())
            {
                return false;
            }

            // ignore hits with specific ignored colliders (self colliders, by default)
            if (IgnoredColliders != null && IgnoredColliders.Contains(hit.GetComponent<Collider>()))
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