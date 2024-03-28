using UnityEngine;
using UnityEngine.UI;
using Unity.Game.Audio;

namespace Unity.Game.UI
{
    public class MicrophoneUI : MonoBehaviour
    {
        public MicrophoneInput MicrophoneInput;
        public AudioSource EnableMicrophoneSound;

        Slider m_Slider;

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

        }

        void OnStartRecord()
        {

        }

        void OnStopRecord()
        {

        }
    }
}


