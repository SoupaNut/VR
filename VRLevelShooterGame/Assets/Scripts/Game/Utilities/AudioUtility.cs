using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Game.Utilities
{
    public class AudioUtility : MonoBehaviour
    {

        public enum AudioGroups
        {
            DamageTick,
            Impact,
            EnemyDetection,
            Pickup,
            WeaponShoot,
            WeaponOverheat,
            WeaponChargeBuildup,
            WeaponChargeLoop,
            HUDVictory,
            HUDObjective,
            EnemyAttack
        }

        public static void CreateSfx(AudioClip clip, Vector3 position, AudioGroups audioGroup, float spatialBlend = 0f, float rolloffDistanceMin = 1f, float volume = 0.5f)
        {
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = position;

            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = spatialBlend;
            source.minDistance = rolloffDistanceMin;
            source.volume = volume;
            source.loop = false;
            source.Play();

            Destroy(impactSfxInstance, clip.length);
        }
    }
}

