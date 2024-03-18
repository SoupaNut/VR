using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using OpenAI;

namespace Unity.Game.Audio
{
    public class MicrophoneInput : MonoBehaviour
    {
        public InputActionProperty TalkButton;
        public float m_ActivationThreshold = 0.1f;
        public int MaxRecordDuration = 30;

        public string CurrentMicrophone { get; private set; }
        public bool IsRecording { get; private set; }

        OpenAIApi openai = new OpenAIApi();
        AudioSource m_AudioSource;
        int ResizeRecordingRate = 1;
        float m_Time;
        string m_FileName = "output.wav";

        // Temporary audio vector we write to every second while recording is enabled
        List<float> tempRecording = new List<float>();

        // Start is called before the first frame update
        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();
            //CurrentMicrophone = Microphone.devices[0];

            // Set up recording to last a max of the recording rate and loop over and over
            m_AudioSource.clip = Microphone.Start(null, true, ResizeRecordingRate, AudioSettings.outputSampleRate);

            // resize our temp vector every second
            //Invoke("ResizeRecording", ResizeRecordingRate);
        }

        void ResizeRecording()
        {
            if(IsRecording)
            {
                // Add next second of recorded audio to temp vector
                float[] clipData = new float[AudioSettings.outputSampleRate];
                m_AudioSource.clip.GetData(clipData, 0);
                tempRecording.AddRange(clipData);
            }
        }

        void OnEnable()
        {
            TalkButton.action.performed += StartRecording;
            TalkButton.action.canceled += StopRecording;
        }

        void OnDisable()
        {
            TalkButton.action.performed -= StartRecording;
            TalkButton.action.canceled -= StopRecording;
        }

        // Update is called once per frame
        void Update()
        {
            m_Time += Time.deltaTime;

            if (m_Time >= ResizeRecordingRate)
            {
                ResizeRecording();
                m_Time = 0;
            }

            //if (IsRecording)
            //{
            //    //m_Time += Time.deltaTime;

            //    //if(m_Time >= ResizeRecordingRate)
            //    //{
            //    //    m_Time = 0;
            //    //}

            //    //if(m_Time >= MaxRecordDuration)
            //    //{
            //    //    IsRecording = false;
            //    //    m_Time = 0f;
            //    //    TranscribeAudio();
            //    //}
            //    //StopRecording();
            //    // TODO: Might use this loop later for UI elements or something
            //}

        }

        void StartRecording(InputAction.CallbackContext context)
        {
            Debug.Log("Recording...");
            IsRecording = true;
            //AudioClip = Microphone.Start(null, false, MaxRecordDuration, 44100);
        }

        void StopRecording(InputAction.CallbackContext context)
        {
            if(IsRecording)
            {
                IsRecording = false;
                Microphone.End(null);
                TranscribeAudio();
            }
        }

        async void TranscribeAudio()
        {
            Debug.Log("Transcribing...");

            byte[] data = SaveWav.Save(m_FileName, m_AudioSource.clip);
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() { Data = data, Name = "audio.wav" },
                // File = Application.persistentDataPath + "/" + fileName,
                Model = "whisper-1",
                Language = "en"
            };

            var res = await openai.CreateAudioTranscription(req);

            Debug.Log(res.Text);
        }
    }
}


