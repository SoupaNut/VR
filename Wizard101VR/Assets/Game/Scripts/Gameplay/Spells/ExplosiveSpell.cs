using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Shared;

namespace Unity.Game.Gameplay.Spells
{
    public class ExplosiveSpell : MovingSpell
    {
        [Header("Explosion Settings")]
        [Tooltip("The radius of the explosion")]
        public float ExplosionRadius = 2f;

        [Tooltip("The spell will explode regardless if it collides with an object or not.")]
        public bool ExplodeAtEndOfLife = false;

        [Tooltip("Time to wait before dealing damage. Used to let animation play")]
        public float ExplosionDelay;

        MovingSpell m_MovingSpell;

        void OnDestroy()
        {
            if (ExplodeAtEndOfLife) Explode(transform.position);
        }

        protected override void Awake()
        {
            base.Awake();

            m_MovingSpell = GetComponent<MovingSpell>();
            DebugUtility.HandleErrorIfNullGetComponent<MovingSpell, ExplosiveSpell>(m_MovingSpell, this, gameObject);
        }

        public override void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {
            StartCoroutine(Explode(point));
        }

        IEnumerator Explode(Vector3 origin)
        {
            // Impact Vfx
            if (ImpactVfx)
            {
                GameObject impactVfxInstance = Instantiate(ImpactVfx, origin, Quaternion.identity);
                if (ImpactVfxLifetime > 0)
                {
                    Destroy(impactVfxInstance.gameObject, ImpactVfxLifetime);
                }
            }

            yield return new WaitForSeconds(ExplosionDelay);


            var ignoredColliders = m_MovingSpell.IgnoredColliders;
            // Get colliders within the explosion radius
            Collider[] colliders = Physics.OverlapSphere(origin, ExplosionRadius, HittableLayers);

            foreach (Collider collider in colliders)
            {
                // Skip to next collider if current one doesn't have health or is a self collider
                Health health = collider.GetComponentInParent<Health>();
                if (health == null || ignoredColliders.Contains(collider)) continue;


                // Check for obstructions
                Vector3 targetPosition = collider.transform.position;
                Vector3 diffVector = targetPosition - origin;
                Vector3 direction = diffVector.normalized;
                float sqrDistance = diffVector.sqrMagnitude;

                RaycastHit[] hits = Physics.RaycastAll(origin, direction, ExplosionRadius, HittableLayers, QueryTriggerInteraction.Ignore);

                bool isObstructed = false;

                foreach (var hit in hits)
                {
                    // Skip if the hit collider is part of self colliders, or has Health component
                    if (ignoredColliders.Contains(hit.collider) || hit.collider == collider || hit.collider.GetComponentInParent<Health>()) continue;

                    float sqrHitDistance = (hit.point - origin).sqrMagnitude;

                    // if an obstruction is found before the target collider, mark as obstructed and break
                    if (sqrHitDistance < sqrDistance)
                    {
                        isObstructed = true;
                        break;
                    }
                }

                if (!isObstructed)
                {
                    health.TakeDamage(Damage, gameObject);
                }
            }

            Destroy(gameObject, m_MovingSpell.OnHitLifetime);
        }
    }
}

