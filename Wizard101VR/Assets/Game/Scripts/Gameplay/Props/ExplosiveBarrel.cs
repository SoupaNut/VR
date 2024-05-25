using UnityEngine;
using Unity.Game.Shared;
using System.Linq;
using Unity.VisualScripting;

namespace Unity.Game.Gameplay.Props
{
    [RequireComponent(typeof(Health))]
    public class ExplosiveBarrel : MonoBehaviour
    {

        [Header("Game Objects")]
        [Tooltip("Barrel object")]
        public GameObject Barrel;

        [Tooltip("VFX of setting on fire")]
        public GameObject FireVfx;

        [Tooltip("Explosion VFX")]
        public GameObject ExplosionVfx;

        [Header("Parameters")]
        [Tooltip("Max amount of time the object stays active in the scene after it explodes")]
        public float MaxLifeTime = 5f;

        [Tooltip("Radius of the explosion")]
        public float ExplosionRadius = 2f;

        [Tooltip("Minimum time that the barrel must be set on fire before exploding")]
        public float MinTimeOnFire = 1f;

        [Tooltip("Layers the explosion can collide with")]
        public LayerMask HittableLayers = -1;

        public Transform ExplosionOrigin;

        [Header("Debug")]
        public bool DisplayRadius;

        public Color RadiusColor = Color.red;

        public float Damage = 20f;

        bool m_isOnFire = false;
        bool m_ReadyToExplode = false;
        bool m_HasExploded = false;
        float timer = 0f;
        Health m_Health;
        Collider[] m_SelfColliders;

        // Start is called before the first frame update
        void Start()
        {
            FireVfx.SetActive(false);
            ExplosionVfx.SetActive(false);

            m_SelfColliders = GetComponentsInChildren<Collider>();

            m_Health = GetComponent<Health>();
            m_Health.onDamage += OnDamaged;
            m_Health.onDie += OnDie;
        }

        // Update is called once per frame
        void Update()
        {
            if(m_isOnFire && !m_HasExploded)
            {
                timer += Time.deltaTime;

                if(timer >= MinTimeOnFire && m_ReadyToExplode)
                {
                    Explode();
                }
            }
        }

        void OnDie()
        {
            m_ReadyToExplode = true;
        }

        void OnDamaged(float damage, GameObject source)
        {
            m_isOnFire = true;

            if(FireVfx)
            {
                FireVfx.SetActive(true);
            }
        }

        void Explode()
        {
            Barrel.SetActive(false);
            FireVfx.SetActive(false);
            ExplosionVfx.SetActive(true);

            // Get colliders within the explosion radius
            Collider[] colliders = Physics.OverlapSphere(ExplosionOrigin.position, ExplosionRadius, HittableLayers);

            foreach (Collider collider in colliders)
            {
                // Skip to next collider if current one doesn't have health or is a self collider
                Health health = collider.GetComponentInParent<Health>();
                if (health == null || m_SelfColliders.Contains(collider)) continue;


                // Check for obstructions
                Vector3 targetPosition = collider.transform.position;
                Vector3 diffVector = targetPosition - ExplosionOrigin.position;
                Vector3 direction = diffVector.normalized;
                float sqrDistance = diffVector.sqrMagnitude;

                RaycastHit[] hits = Physics.RaycastAll(ExplosionOrigin.position, direction, ExplosionRadius, HittableLayers, QueryTriggerInteraction.Ignore);

                bool isObstructed = false;

                foreach(var hit in hits)
                {
                    // Skip if the hit collider is part of self colliders, or has Health component
                    if (m_SelfColliders.Contains(hit.collider) || hit.collider == collider || hit.collider.GetComponentInParent<Health>()) continue;

                    float sqrHitDistance = (hit.point - ExplosionOrigin.position).sqrMagnitude;

                    // if an obstruction is found before the target collider, mark as obstructed and break
                    if(sqrHitDistance < sqrDistance)
                    {
                        isObstructed = true;
                        break;
                    }
                }

                if(!isObstructed)
                {
                    health.TakeDamage(Damage, gameObject);
                }
            }
            m_HasExploded = true;

            Destroy(gameObject, MaxLifeTime);
        }

        private void OnDrawGizmos()
        {
            if(DisplayRadius)
            {
                Gizmos.color = RadiusColor;
                Gizmos.DrawWireSphere(ExplosionOrigin.position, ExplosionRadius);
            }
        }
    }
}


