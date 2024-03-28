using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Unity.Game.Shared;

namespace Unity.Game.Audio
{
    public class MicrophoneInput : MonoBehaviour
    {
        [Tooltip("Button to press when talking")]
        public InputActionProperty TalkButton;

        [Tooltip("Threshold to know when button is pressed")]
        public float m_ActivationThreshold = 0.1f;

        [Tooltip("How long user can talk for in seconds")]
        [Range(1, 30)]
        public int MaxRecordDuration = 15;

        public UnityAction onStartRecord;
        public UnityAction onStopRecord;

        public bool IsRecording { get; private set; }

        ChatGPTManager m_ChatGPTManager;
        AudioClip m_AudioClip;
        float m_Time;
        int m_StartSamplePosition;
        int m_EndSamplePosition;
        int m_MaxSamples;

        // Start is called before the first frame update
        void Start()
        {
            m_ChatGPTManager = FindObjectOfType<ChatGPTManager>();
            DebugUtility.HandleErrorIfNullFindObject<ChatGPTManager, MicrophoneInput>(m_ChatGPTManager, this);

            // Set up recording to last a max of the recording rate + 1 and loop over and over
            // +1 is to give enough time to read the data before it is overwritten
            m_AudioClip = Microphone.Start(null, true, MaxRecordDuration + 1, AudioSettings.outputSampleRate);

            m_MaxSamples = m_AudioClip.samples * m_AudioClip.channels;
            
        }

        void OnEnable()
        {
            TalkButton.action.performed += TalkButtonPressed;
            TalkButton.action.canceled += TalkButtonReleased;
        }

        void OnDisable()
        {
            TalkButton.action.performed -= TalkButtonPressed;
            TalkButton.action.canceled -= TalkButtonReleased;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsRecording)
            {
                // stop recording if we hit our max time of recording
                m_Time += Time.deltaTime;
                if (m_Time >= MaxRecordDuration)
                {
                    StopRecording();
                }
            }
        }

        public float GetLoudness()
        {
            return AudioUtility.GetLoudnessFromAudioClip(Microphone.GetPosition(null), m_AudioClip);
        }

        void TalkButtonPressed(InputAction.CallbackContext context)
        {
#if UNITY_EDITOR
            Debug.Log("Recording...");
#endif
            onStartRecord?.Invoke();
            m_StartSamplePosition = Microphone.GetPosition(null);
            IsRecording = true;
            m_Time = 0;
        }

        void TalkButtonReleased(InputAction.CallbackContext context)
        {
            StopRecording();
        }

        void StopRecording()
        {
            if (IsRecording)
            {
                onStopRecord?.Invoke();
                m_EndSamplePosition = Microphone.GetPosition(null);
                IsRecording = false;
                TranscribeAudio();
            }
        }

        async void TranscribeAudio()
        {
#if UNITY_EDITOR
            Debug.Log("Transcribing...");
#endif
            // get entire audioclip data
            float[] samples = new float[m_MaxSamples];
            m_AudioClip.GetData(samples, 0);

            // get new samples array based on the given start and end point
            samples = GetSampleDataArray(samples, m_StartSamplePosition, m_EndSamplePosition);

            byte[] data = SaveWav.EncodeAsWAV(samples, m_AudioClip.frequency, m_AudioClip.channels);

            string res = await m_ChatGPTManager.GetAudioTranscription(data);

#if UNITY_EDITOR
            Debug.Log(res);
#endif

            m_ChatGPTManager.AskChatGPT(res);
        }

        float[] GetSampleDataArray(float[] samples, int startIndex, int endIndex)
        {
            // Don't add 1 at the end since we don't need the data at the endIndex
            int range = (endIndex - startIndex + m_MaxSamples) % m_MaxSamples;
            // create array to hold our new samples
            float[] newSamples = new float[range];

            // fill in data
            for (int i = 0; i < range; i++)
            {
                int index = (startIndex + i) % m_MaxSamples;
                newSamples[i] = samples[index];
            }
            return newSamples;
        }
    }
}


