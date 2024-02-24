using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Utilities;

namespace Unity.Game.Shared
{
    public class WeaponController : MonoBehaviour
    {
        [Header("Internal References")]
        public Transform WeaponMuzzle;

        [Header("Shoot Parameters")]
        public ProjectileBase ProjectilePrefab;
        public float ProjectileSpeed = 20f;
        public float FireRate = 1f;
        public float Damage = 10f;
        public WeaponModes WeaponMode;
        public enum WeaponModes
        {
            Auto,
            SemiAuto
        }

        [Header("Audio")]
        public AudioClip WeaponSound;

        private bool m_ReadyToFire;
        private float m_NextFireTime = 0f;

        // Update is called once per frame
        void Update()
        {
            if(Time.time >= m_NextFireTime && m_ReadyToFire)
            {
                Fire();
                m_NextFireTime = Time.time + 1f / FireRate;
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

            // play weapon bullet sound
            float volume = 0.3f;
            float spatialBlend = 1f;
            float minDistance = 3f;
            AudioUtility.CreateSfx(WeaponSound, WeaponMuzzle.position, AudioUtility.AudioGroups.WeaponShoot, spatialBlend, minDistance, volume);
        }
    }
}


