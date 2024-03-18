using UnityEngine;
using UnityEngine.Audio;
using Unity.Game.Utilities;

namespace Unity.Game.Audio
{
    public class AudioManager : MonoBehaviour
    {
        string m_Mic;
        AudioClip m_AudioClip;

        // Start is called before the first frame update
        void Start()
        {
            // get first microphone in device list
            m_Mic = Microphone.devices[0];
            m_AudioClip = Microphone.Start(m_Mic, true, 20, AudioSettings.outputSampleRate);
        }

        public string GetSpeechToText()
        {
            return AudioUtility.GetTextFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), m_AudioClip);
        }

        public float GetLoudnessFromMicrophone()
        {
            return AudioUtility.GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), m_AudioClip);
        }
    }
}


