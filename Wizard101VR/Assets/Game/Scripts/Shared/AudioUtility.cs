using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Unity.Game.Shared
{
    public class AudioUtility : MonoBehaviour
    {
        static AudioManager s_AudioManager;

        public enum AudioGroups
        {
            General,
            UserInterface,
            DamageTick,
            Impact,
            EnemyDetection,
            EnemyDeath,
            Spells,
            //Pickup,
            WeaponShoot,
            //WeaponOverheat,
            //WeaponChargeBuildup,
            //WeaponChargeLoop,
            //HUDVictory,
            //HUDObjective,
            EnemyAttack
        }

        public static float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip, int sampleWindow = 64)
        {
            int startPosition = clipPosition - sampleWindow;
            float[] waveData = new float[sampleWindow];
            clip.GetData(waveData, startPosition);

            if (startPosition < 0)
                return 0;

            // compute loudness
            float totalLoudness = 0;
            for(int i = 0; i < sampleWindow; i++)
            {
                totalLoudness += Mathf.Abs(waveData[i]);
            }

            return totalLoudness / sampleWindow;
        }

        public static AudioSource CreateSfx(AudioClip clip, GameObject gameObject, AudioGroups audioGroup, float spatialBlend = 0f, float rolloffDistanceMin = 1f)
        {
            AudioSource source = gameObject.GetComponent<AudioSource>();

            // Add an audio source if the gameobject doesn't have one
            if(source == null)
            {
                source = gameObject.AddComponent<AudioSource>();
            }

            SetAudioSource(source, clip, audioGroup, spatialBlend, rolloffDistanceMin);

            return source;
        }

        public static AudioSource CreateSfx(AudioClip clip, Vector3 position, AudioGroups audioGroup, float spatialBlend = 0f, float rolloffDistanceMin = 1f)
        {
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = position;

            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            SetAudioSource(source, clip, audioGroup, spatialBlend, rolloffDistanceMin);

            Destroy(impactSfxInstance, clip.length);

            return source;
        }

        static void SetAudioSource(AudioSource source, AudioClip clip, AudioGroups audioGroup, float spatialBlend = 0f, float rolloffDistanceMin = 1f)
        {
            source.clip = clip;
            source.spatialBlend = spatialBlend;
            source.minDistance = rolloffDistanceMin;
            source.Play();
            source.outputAudioMixerGroup = GetAudioGroup(audioGroup);
        }

        public static AudioMixerGroup GetAudioGroup(AudioGroups group)
        {
            if (s_AudioManager == null)
            {
                s_AudioManager = GameObject.FindObjectOfType<AudioManager>();
            }

            var groups = s_AudioManager.FindMatchingGroups(group.ToString());

            if (groups.Length > 0)
                return groups[0];

            Debug.LogWarning("Didn't find audio group for " + group.ToString());
            return null;
        }

        public static void SetMasterVolume(float value)
        {
            if (s_AudioManager == null)
                s_AudioManager = GameObject.FindObjectOfType<AudioManager>();

            if (value <= 0)
                value = 0.001f;
            float valueInDb = Mathf.Log10(value) * 20;

            s_AudioManager.SetFloat("MasterVolume", valueInDb);
        }

        public static float GetMasterVolume()
        {
            if (s_AudioManager == null)
                s_AudioManager = GameObject.FindObjectOfType<AudioManager>();

            s_AudioManager.GetFloat("MasterVolume", out var valueInDb);
            return Mathf.Pow(10f, valueInDb / 20.0f);
        } 

        public static IEnumerator FadeOut(AudioSource source, float duration)
        {
            float startVolume = source.volume;

            float startTime = Time.time;

            while(Time.time < startTime + duration)
            {
                float progress = (Time.time - startTime) / duration;
                source.volume = Mathf.Lerp(startVolume, 0, progress);

                yield return null;
            }

            source.volume = 0;
            source.Stop();
        }
    }
}

