using UnityEngine;
using Unity.Game.Utilities;

namespace Unity.Game.Audio
{
    public class ScaleOnMicInput : MonoBehaviour
    {
        public Vector3 MinScale;
        public Vector3 MaxScale;
        public float LoudnessSensibility = 100f;
        public float Threshold = 0.1f;

        AudioManager AudioManager;

        // Start is called before the first frame update
        void Start()
        {
            AudioManager = FindObjectOfType<AudioManager>();
            DebugUtility.HandleErrorIfNullFindObject<AudioManager, ScaleOnMicInput>(AudioManager, this);
        }

        // Update is called once per frame
        void Update()
        {
            float loudness = AudioManager.GetLoudnessFromMicrophone() * LoudnessSensibility;

            if(loudness < Threshold)
            {
                loudness = 0;
            }

            // lerp value from minscale to maxscale
            transform.localScale = Vector3.Lerp(MinScale, MaxScale, loudness);
        }
    }
}


