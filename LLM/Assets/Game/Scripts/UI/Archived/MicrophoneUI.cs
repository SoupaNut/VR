using UnityEngine;
using UnityEngine.UI;
using Unity.Game.Audio;
using Unity.Game.Shared;
//using UnityEditor;

namespace Unity.Game.UI
{
    public class MicrophoneUI : MonoBehaviour
    {
        public MicrophoneInput PlayerMicrophone;
        public float LoudnessSensibility = 100f;
        public Color VignetteColor;
        public Color VignetteColorBlend;

        [Range(0,1)]
        public float ActivationAlpha = 0.5f;

        Material m_VignetteMaterial;

        // Start is called before the first frame update
        void Start()
        {
            m_VignetteMaterial = GetComponent<Renderer>().materials[0];

            PlayerMicrophone.onStartRecord += OnStartRecord;
            PlayerMicrophone.onStopRecord += OnStopRecord;

            // Make sure border is not showing at the start
            OnStopRecord();

            // The below code prints out all of the material's properties
            // ------------------------------------------------------------------------------
            //int propertyCount = m_VignetteMaterial.shader.GetPropertyCount();

            //for (int i = 0; i < propertyCount; i++)
            //{
            //    string propertyName = m_VignetteMaterial.shader.GetPropertyName(i);
            //    var propertyType = m_VignetteMaterial.shader.GetPropertyType(i);

            //    Debug.Log("Property Name: " + propertyName + ", Type: " + propertyType);
            //}
        }

        void OnStartRecord()
        {
            m_VignetteMaterial.SetColor("_VignetteColor", new Color(VignetteColor.r, VignetteColor.g, VignetteColor.b, ActivationAlpha));
            m_VignetteMaterial.SetColor("_VignetteColorBlend", new Color(VignetteColorBlend.r, VignetteColorBlend.g, VignetteColorBlend.b, ActivationAlpha));
        }

        void OnStopRecord()
        {
            m_VignetteMaterial.SetColor("_VignetteColor", new Color(VignetteColor.r, VignetteColor.g, VignetteColor.b, 0f));
            m_VignetteMaterial.SetColor("_VignetteColorBlend", new Color(VignetteColorBlend.r, VignetteColorBlend.g, VignetteColorBlend.b, 0f));
        }
    }
}


