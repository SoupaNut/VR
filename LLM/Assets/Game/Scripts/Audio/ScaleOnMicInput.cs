using UnityEngine;
using Unity.Game.Shared;

namespace Unity.Game.Audio
{
    public class ScaleOnMicInput : MonoBehaviour
    {
        public Vector3 MinScale;
        public Vector3 MaxScale;
        public float LoudnessSensibility = 100f;
        public float Threshold = 0.1f;

        MicrophoneInput MicrophoneInput;

        // Start is called before the first frame update
        void Start()
        {
            MicrophoneInput = FindObjectOfType<MicrophoneInput>();
            DebugUtility.HandleErrorIfNullFindObject<MicrophoneInput, ScaleOnMicInput>(MicrophoneInput, this);
        }

        // Update is called once per frame
        void Update()
        {
            float loudness = MicrophoneInput.GetLoudness() * LoudnessSensibility;

            if (loudness < Threshold)
            {
                loudness = 0;
            }

            // lerp value from minscale to maxscale
            transform.localScale = Vector3.Lerp(MinScale, MaxScale, loudness);

            //AudioManager.GetSpeechToText();
        }
    }
}


