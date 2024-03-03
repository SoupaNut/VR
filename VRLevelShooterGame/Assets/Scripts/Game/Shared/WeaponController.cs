using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Utilities;

namespace Unity.Game.Shared
{
    public class WeaponController : MonoBehaviour
    {
        public enum WeaponShootType
        {
            Manual,
            Automatic,
            Charge
        }

        [Header("Internal References")]
        public Transform WeaponMuzzle;
        
        [Header("Shoot Parameters")]
        [Tooltip("The type of weapon wil affect how it shoots")]
        public WeaponShootType ShootType;

        [Tooltip("The projectile prefab")]
        public ProjectileBase ProjectilePrefab;

        [Tooltip("How fast the projectile prefab travels")]
        public float ProjectileSpeed = 20f;

        [Tooltip("How fast the weapon shoots projectiles")]
        public float FireRate = 1f;

        [Tooltip("Amount of damage the projectile deals upon hit")]
        public float Damage = 10f;
        
        [Header("Audio")]
        public AudioClip WeaponSound;

        [Header("Visual")]
        [Tooltip("Prefab of the muzzle flash")]
        public GameObject MuzzleFlashPrefab;

        public GameObject Owner { get; set; }

        private bool m_ReadyToFire;
        private float m_NextFireTime = 0f;

        // ------------------- TEMPORARY (REMOVE LATER)
        private void Start()
        {
            Owner = gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            if(Time.time >= m_NextFireTime && m_ReadyToFire)
            {
                Fire();
                m_NextFireTime = Time.time + 1f / FireRate;
                m_ReadyToFire = false;
            }
        }

        public void SetReadyToFire(bool fire)
        {
            m_ReadyToFire = fire;
        }

        public void Fire()
        {
            // spawn bullet
            ProjectileBase spawnedProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, WeaponMuzzle.rotation);
            spawnedProjectile.Shoot(this);

            // Spawn Muzzle Flash
            if(MuzzleFlashPrefab != null)
            {
                GameObject spawnedMuzzleFlash = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);


                Destroy(spawnedMuzzleFlash, 2f);
            }
            
            // play weapon bullet sound
            float volume = 0.3f;
            float spatialBlend = 1f;
            float minDistance = 3f;
            AudioUtility.CreateSfx(WeaponSound, WeaponMuzzle.position, AudioUtility.AudioGroups.WeaponShoot, spatialBlend, minDistance, volume);
        }
    }
}


