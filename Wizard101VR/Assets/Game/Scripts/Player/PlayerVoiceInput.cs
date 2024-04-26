using UnityEngine;
using UnityEngine.InputSystem;
using Oculus.Voice;
using Unity.Game.Shared;
using Unity.Game.Entity.NPC;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.Game.Player
{
    public class PlayerVoiceInput : MonoBehaviour
    {
        [Header("General")]
        [Tooltip("Object containing Oculus Voice SDK")]
        public AppVoiceExperience VoiceManager;

        [Tooltip("Gaze Interactor object for looking at NPC")]
        public XRGazeInteractor GazeInteractor;

        [Tooltip("Button to press when talking")]
        public InputActionProperty TalkButton;

        [Header("SFX")]
        [Tooltip("Sound to play when microphone starts recording")]
        public AudioClip MicrophoneStartClip;

        [Tooltip("Sound to play when microphone stops recording")]
        public AudioClip MicrophoneStopClip;

        [Tooltip("SFX to play when NPC is active")]
        public AudioClip NPCActiveClip;

        [Header("VFX")]
        [Tooltip("Border to display around oculus lens when player presses talk button")]
        public GameObject MicrophoneUI;

        [Tooltip("VFX marker to display when talking to active NPC. Displays at NPC's feet.")]
        public GameObject NPCActiveMarker;

        [Tooltip("Range indicating how far away the player can be before NPC becomes inactive")]
        public GameObject RangeIndicator;

        public bool IsTalking { get; private set; }
        public float TalkRange { get; private set; }

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
            GazeInteractor.selectEntered.AddListener(GazeSelectEnteredHandler);

            TalkRange = GazeInteractor.maxRaycastDistance;

            // - 0.5 so that we don't accidentally go over the talk limit
            m_MaxTalkDuration = VoiceManager.RuntimeConfiguration.maxRecordingTime - 0.2f;
            VoiceManager.VoiceEvents.OnFullTranscription.AddListener(AskChatGpt);

            if(RangeIndicator)
            {
                RangeIndicator.transform.localScale = new Vector3(TalkRange, RangeIndicator.transform.localScale.y, TalkRange);
            }

        }

        void AskChatGpt(string text)
        {
            if(m_ChatGPTManager)
            {
                Debug.Log("Player: " + text);

                m_ChatGPTManager.AskChatGPT(text);
            }
        }

        // Update is called once per frame
        void Update()
        {
            HandlePlayerTalking();

            CheckNPCWithinTalkRange();
        }

        void HandlePlayerTalking()
        {
            if (IsTalking)
            {
                m_CurrentTalkTime += Time.deltaTime;

                // Stop Talking if we reached our max limit
                if (m_CurrentTalkTime > m_MaxTalkDuration)
                {
                    StoppedTalking();
                }
                else
                {
                    VoiceManager.Activate();
                }
            }
        }

        void CheckNPCWithinTalkRange()
        {
            if(m_ChatGPTManager)
            {
                float sqrTalkRange = TalkRange * TalkRange;
                float sqrDistance = (m_ChatGPTManager.transform.position - transform.position).sqrMagnitude;

                // if we go out of the talk range
                if(sqrDistance > sqrTalkRange)
                {
                    SetNPCActiveUI(false, null);
                    m_ChatGPTManager.TurnNPCAwayFromTarget();
                    m_ChatGPTManager = null;
                }
            }
        }

        void SetNPCActiveUI(bool active, Transform parent)
        {
            // Display VFX
            if(NPCActiveMarker)
            {
                NPCActiveMarker.transform.parent = active ? parent : null;
                NPCActiveMarker.transform.position = active ? parent.position : Vector3.zero;
                NPCActiveMarker.SetActive(active);
            }

            if (RangeIndicator)
            {
                RangeIndicator.transform.parent = active ? parent : null;
                RangeIndicator.transform.position = active ? parent.position : Vector3.zero;
                RangeIndicator.SetActive(active);
            }

            // Play SFX
            if (NPCActiveClip && active)
            {
                AudioUtility.CreateSfx(NPCActiveClip, NPCActiveMarker.transform.position, AudioUtility.AudioGroups.UserInterface, 1f, 3f);
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

        void GazeSelectEnteredHandler(SelectEnterEventArgs args)
        {
            var chatgptManager = args.interactableObject.transform.GetComponent<ChatGPTManager>();

            // If we are looking at a different npc
            if (chatgptManager && chatgptManager != m_ChatGPTManager)
            {
                // Deselect the previous NPC
                if(m_ChatGPTManager != null)
                {
                    m_ChatGPTManager.TurnNPCAwayFromTarget();
                    SetNPCActiveUI(false, null);
                }

                // Select the current NPC
                m_ChatGPTManager = chatgptManager;
                m_ChatGPTManager.TurnNPCTowardsTarget(transform);
                SetNPCActiveUI(true, m_ChatGPTManager.transform);
            }
        }
    }
}


