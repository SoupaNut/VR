using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Shared;

namespace Unity.Game.Gameplay
{
    public class ExplosiveSpell : MovingSpell
    {
        [Header("Explosion Settings")]
        [Tooltip("The radius of the explosion")]
        public float ExplosionRadius = 2f;

        [Tooltip("The spell will explode regardless if it collides with an object or not.")]
        public bool ExplodeAtEndOfLife = false;

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
            Explode(point);
        }

        public void Explode(Vector3 origin)
        {
            Collider[] colliders = Physics.OverlapSphere(origin, ExplosionRadius, m_MovingSpell.HittableLayers);

            foreach(Collider collider in colliders)
            {
                // Calculate direction from the explosion to the hit point
                Vector3 direction = collider.transform.position - origin;

                // check if there is a direct line of sight
                if(!Physics.Raycast(origin, direction, direction.magnitude, m_MovingSpell.HittableLayers))
                {
                    // Damage
                    Health health = collider.GetComponentInParent<Health>();
                    if (health != null)
                    {
                        health.TakeDamage(m_MovingSpell.Damage, m_MovingSpell.Owner);
                    }
                }
            }

            Destroy(gameObject, m_MovingSpell.OnHitLifetime);
        }
    }
}

