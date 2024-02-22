using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Game.Utilities
{
    public class AudioUtility : MonoBehaviour
    {
        public static void CreateSfx(AudioClip clip, Vector3 position, float volume = 0.5f)
        {
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = position;

            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.loop = false;
            source.Play();

            Destroy(impactSfxInstance, clip.length);
        }
    }
}

