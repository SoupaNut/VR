using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Oculus.Voice;
using Unity.Game.Shared;
using Unity.Game.NPC;

namespace Unity.Game.Player
{
    public class PlayerVoiceInput : MonoBehaviour
    {
        [Header("General")]
        [Tooltip("Object containing Oculus Voice SDK")]
        public AppVoiceExperience VoiceManager;

        [Tooltip("Button to press when talking")]
        public InputActionProperty TalkButton;

        [Header("SFX")]
        [Tooltip("Sound to play when microphone starts recording")]
        public AudioClip MicrophoneStartClip;

        [Tooltip("Sound to play when microphone stops recording")]
        public AudioClip MicrophoneStopClip;

        [Header("VFX")]
        [Tooltip("Border to display around oculus lens when player presses talk button")]
        public GameObject MicrophoneUI;

        public bool IsTalking { get; private set; }

        ChatGPTManager m_ChatGPTManager;
        float m_MaxTalkDuration;
        float m_CurrentTalkTime;


        void OnEnable()
        {
            TalkButton.action.performed += OnTalkButtonPerformed;
            TalkButton.action.canceled += OnTalkButtonCanceled;
        }

        void OnDisable()
        {
            TalkButton.action.performed -= OnTalkButtonPerformed;
            TalkButton.action.canceled -= OnTalkButtonCanceled;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_ChatGPTManager = FindObjectOfType<ChatGPTManager>();
            DebugUtility.HandleErrorIfNullFindObject<ChatGPTManager, PlayerVoiceInput>(m_ChatGPTManager, this);

            // - 0.5 so that we don't accidentally go over the talk limit
            m_MaxTalkDuration = VoiceManager.RuntimeConfiguration.maxRecordingTime - 0.5f;
            VoiceManager.VoiceEvents.OnFullTranscription.AddListener(AskChatGpt);

        }

        void AskChatGpt(string text)
        {
            Debug.Log("Player: " + text);

            m_ChatGPTManager.AskChatGPT(text);
        }

        // Update is called once per frame
        void Update()
        {
            if (IsTalking)
            {
                m_CurrentTalkTime += Time.deltaTime;

                // Stop Talking if we reached our max limit
                if(m_CurrentTalkTime > m_MaxTalkDuration)
                {
                    StoppedTalking();
                }
                else
                {
                    VoiceManager.Activate();
                }
            }
        }

        void OnTalkButtonPerformed(InputAction.CallbackContext context)
        {
            IsTalking = true;
            m_CurrentTalkTime = 0;

            // Display VFX
            if(MicrophoneUI)
            {
                MicrophoneUI.SetActive(true);
            }


            // Play SFX
            if (MicrophoneStartClip)
            {
                AudioUtility.CreateSfx(MicrophoneStartClip, transform.position, AudioUtility.AudioGroups.UserInterface);
            }
        }

        void OnTalkButtonCanceled(InputAction.CallbackContext context)
        {
            if(IsTalking)
            {
                StoppedTalking();
            }
        }

        void StoppedTalking()
        {
            IsTalking = false;

            // Display VFX
            if (MicrophoneUI)
            {
                MicrophoneUI.SetActive(false);
            }

            // Play SFX
            if (MicrophoneStopClip)
            {
                AudioUtility.CreateSfx(MicrophoneStopClip, transform.position, AudioUtility.AudioGroups.UserInterface);
            }
        }
    }
}


