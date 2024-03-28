using UnityEngine;
using UnityEngine.UI;
using Unity.Game.Shared;

namespace Unity.Game.UI
{
    public class MicrophoneUI : MonoBehaviour
    {
        public MicrophoneInput MicrophoneInput;
        public AudioClip EnableMicrophoneClip;
        public AudioClip DisableMicrophoneClip;

        Slider m_Slider;
        //bool m_IsTalking;

        // Start is called before the first frame update
        void Start()
        {
            m_Slider = GetComponentInChildren<Slider>();

            MicrophoneInput.onStartRecord += OnStartRecord;
            MicrophoneInput.onStopRecord += OnStopRecord;
        }

        // Update is called once per frame
        void Update()
        {
            if(gameObject.activeSelf)
            {
                m_Slider.value = MicrophoneInput.GetLoudness();
            }
        }

        void OnStartRecord()
        {
            //m_IsTalking = true;
            gameObject.SetActive(true);

            if(EnableMicrophoneClip)
            {
                AudioUtility.CreateSfx(EnableMicrophoneClip, AudioGroups.UserInterface);
            }
        }

        void OnStopRecord()
        {
            //m_IsTalking = false;
            gameObject.SetActive(false);

            if (DisableMicrophoneClip)
            {
                AudioUtility.CreateSfx(DisableMicrophoneClip, AudioGroups.UserInterface);
            }
        }
    }
}


