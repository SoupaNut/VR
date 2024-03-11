using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

        [Header("Shoot Parameters")]
        [Tooltip("The type of weapon will affect how it shoots")]
        public WeaponShootType ShootType;

        [Tooltip("The projectile prefab")]
        public ProjectileBase ProjectilePrefab;

        [Tooltip("How fast the projectile prefab travels")]
        public float ProjectileSpeed = 20f;

        [Header("Ammo Parameters")]
        [Tooltip("Weapon has infinite ammo")]
        public bool HasInfiniteAmmo = true;

        [Tooltip("Total number of bullets in a clip")]
        public int MaxAmmo = 30;

        [Tooltip("Amount of time it takes to start the reloading process")]
        public float AmmoReloadDelay = 1f;

        [Tooltip("Amount of ammo reloaded per second")]
        public float AmmoReloadRate = 10f;

        [Header("Audio")]
        public AudioClip WeaponSound;

        [Header("Visual")]
        [Tooltip("Prefab of the muzzle flash")]
        public GameObject MuzzleFlashPrefab;

        public GameObject Owner { get; set; }
        public bool IsWeaponEnabled { get; set; }
        public bool IsReadyToFire { get; private set; }
        public bool IsReloading { get; private set; }
        public float CurrentAmmoRatio { get; private set; }

        public UnityAction onShoot;

        float m_NextFireTime = Mathf.NegativeInfinity;
        float m_LastShotFired = Mathf.NegativeInfinity;
        float m_CurrentAmmo;
        bool m_Overheated;

        
        private void Awake()
        {
            Owner = gameObject;// TODO: Remove

            m_CurrentAmmo = MaxAmmo;
            IsReadyToFire = true;
            IsWeaponEnabled = false;
            IsReloading = false;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAmmo();
        }

        void UpdateAmmo()
        {
            // return if infinite ammo or ammo is full
            if(HasInfiniteAmmo || m_CurrentAmmo >= MaxAmmo)
            {
                m_Overheated = false;
                IsReadyToFire = true;
                IsReloading = false;
                CurrentAmmoRatio = 1f;
                return;
            }

            // Overheated --> wait until full reload is done to fire
            if(m_CurrentAmmo <= 0)
            {
                m_CurrentAmmo = 0;
                IsReadyToFire = false;
                IsReloading = false;
                m_Overheated = true;
            }

            // Wait until ammo reload delay is done
            if(Time.time - m_LastShotFired >= AmmoReloadDelay)
            {
                // if we haven't overheated, then weapon can fire any time
                if(!m_Overheated)
                {
                    IsReadyToFire = true;
                }

                m_CurrentAmmo += AmmoReloadRate * Time.deltaTime;
                m_CurrentAmmo = Mathf.Clamp(m_CurrentAmmo, 0, MaxAmmo);
                IsReloading = true;
            }
            else
            {
                IsReloading = false;
            }

            CurrentAmmoRatio = m_CurrentAmmo / MaxAmmo;
        }

        public bool HandleShootInputs(bool inputDown,bool inputHeld, bool inputUp)
        {
            switch(ShootType)
            {
                case WeaponShootType.Manual:
                    if(inputDown)
                    {
                        return TryShoot();
                    }
                    return false;

                case WeaponShootType.Automatic:
                    if(inputHeld)
                    {
                        return TryShoot();
                    }
                    return false;

                case WeaponShootType.Charge:
                    if(inputHeld)
                    {
                        // TODO: begin charging
                    }
                    return false;

                default:
                    return false;
            }
        }

        public bool TryShoot()
        {
            if (HasAmmo() && Time.time >= m_NextFireTime && IsReadyToFire)
            {
                HandleShoot();

                m_NextFireTime = Time.time + 1f / FireRate;
                m_CurrentAmmo -= 1;

                return true;
            }

            return false;
        }

        private bool HasAmmo()
        {
            return HasInfiniteAmmo || m_CurrentAmmo > 0;
        }

        private void HandleShoot()
        {
            // spawn bullet
            ProjectileBase spawnedProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, WeaponMuzzle.rotation);
            spawnedProjectile.Shoot(this);

            onShoot?.Invoke();

            // Spawn Muzzle Flash
            if(MuzzleFlashPrefab != null)
            {
                GameObject spawnedMuzzleFlash = Instantiate(MuzzleFlashPrefab, WeaponMuzzle.position, WeaponMuzzle.rotation, WeaponMuzzle.transform);


                Destroy(spawnedMuzzleFlash, 2f);
            }
            
            // play weapon bullet sound
            float spatialBlend = 1f;
            float minDistance = 3f;
            AudioUtility.CreateSfx(WeaponSound, WeaponMuzzle.position, AudioUtility.AudioGroups.WeaponShoot, spatialBlend, minDistance);

            m_LastShotFired = Time.time;
        }
    }
}


