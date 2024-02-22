using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Shared;
using Unity.Game.Utilities;

namespace Unity.Game.Gameplay
{
    public class ProjectileStandard : MonoBehaviour
    {
        [Header("General")]
        [Tooltip("Radius of this projectile's collision detection")]
        public float Radius = 0.01f;

        [Tooltip("Transform representing the tip of the projectile (used for accurate collision detection)")]
        public Transform Tip;

        [Tooltip("LifeTime of the projectile")]
        public float MaxLifeTime = 5f;


        [Tooltip("Layers this projectile can collide with")]
        public LayerMask HittableLayers = -1;

        [Tooltip("Clip to play on impact")]
        public AudioClip ImpactSfxClip;

        [Header("Damage")]
        [Tooltip("Damage of the projectile")]
        private float m_Damage = 20f;
        public float Damage { get => m_Damage; set => m_Damage = value; }

        private void OnEnable()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // hit detection
            RaycastHit hit;
            if (Physics.SphereCast(Tip.position, Radius, Tip.forward, out hit, Mathf.Infinity, HittableLayers))
            {
                HandleHit(hit);
            }
        }

        private void HandleHit(RaycastHit hitInfo)
        {
            var health = hitInfo.collider.gameObject.GetComponentInParent<Health>();
            Debug.Log(hitInfo.collider.name);
            if (health != null)
            {
                Debug.Log("Health");
                health.TakeDamage(m_Damage);
                AudioUtility.CreateSfx(ImpactSfxClip, hitInfo.transform.position);

                Destroy(gameObject, 0.1f);

            }
        }
    }

}
