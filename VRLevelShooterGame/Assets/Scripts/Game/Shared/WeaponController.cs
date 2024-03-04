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

        [Header("Weapon Parameters")]
        [Tooltip("How many projectiles the weapon fires per second")]
        public float FireRate = 1f;

        [Tooltip("Amount of damage the projectile deals upon hit")]
        public float Damage = 10f;

        [Tooltip("Total weapon ammo (-1 = no ammo limit")]
        public float MaxAmmo = -1f;

        [Tooltip("Weapon ammo per clip (-1 = no ammo limit)")]
        public float AmmoPerClip = -1f;

        [Header("Shoot Parameters")]
        [Tooltip("The type of weapon will affect how it shoots")]
        public WeaponShootType ShootType;

        [Tooltip("The projectile prefab")]
        public ProjectileBase ProjectilePrefab;

        [Tooltip("How fast the projectile prefab travels")]
        public float ProjectileSpeed = 20f;

        [Header("Audio")]
        public AudioClip WeaponSound;

        [Header("Visual")]
        [Tooltip("Prefab of the muzzle flash")]
        public GameObject MuzzleFlashPrefab;

        public GameObject Owner { get; set; }

        bool m_ReadyToFire;
        float m_NextFireTime = Mathf.NegativeInfinity;
        float m_CurrentAmmo;
        float m_CurrentClipAmmo;
        
        private void Start()
        {
            m_CurrentAmmo = MaxAmmo;
            m_CurrentClipAmmo = AmmoPerClip;
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

        public void TryShoot()
        {
            if (Time.time >= m_NextFireTime)
            {
                HandleShoot();
                m_NextFireTime = Time.time + 1f / FireRate;
            }
        }

        private void HandleShoot()
        {
            if(HasAmmo())
            {
                m_CurrentClipAmmo -= 1;
                m_CurrentAmmo -= 1;
            }
        }

        private bool HasAmmo()
        {
            if(m_CurrentAmmo > 0 && m_CurrentClipAmmo > 0)
            {
                return true;
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


