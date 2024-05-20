using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Shared;

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

        [Header("Debug")]
        public bool DisplayRadius;

        public Color RadiusColor = Color.red;

        public float Damage = 20f;

        bool m_isOnFire = false;
        bool m_ReadyToExplode = false;
        bool m_HasExploded = false;
        float timer = 0f;
        Health m_Health;

        // Start is called before the first frame update
        void Start()
        {
            FireVfx.SetActive(false);
            ExplosionVfx.SetActive(false);

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

            Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius, HittableLayers);
            Debug.Log("Length: " + colliders.Length);
            foreach (Collider collider in colliders)
            {
                Debug.Log(collider.gameObject.name);
                // Calculate direction from the explosion to the hit point
                Vector3 direction = collider.transform.position - transform.position;

                // check if there is a direct line of sight
                if (!Physics.Raycast(transform.position, direction, direction.magnitude, HittableLayers))
                {
                    // Damage
                    Health health = collider.GetComponentInParent<Health>();
                    if (health != null)
                    {
                        health.TakeDamage(Damage, null);
                    }
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
                Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
            }
        }
    }
}


