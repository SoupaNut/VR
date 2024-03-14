using UnityEngine;
using UnityEngine.Audio;

namespace Unity.Game.Utilities
{
    public class AudioUtility : MonoBehaviour
    {
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
    }
}

