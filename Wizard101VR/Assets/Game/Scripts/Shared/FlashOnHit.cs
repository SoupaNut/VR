using System.Collections.Generic;
using UnityEngine;

namespace Unity.Game.Shared
{
    using RendererData = TypesUtility.RendererIndexData;

    [RequireComponent(typeof(Health))]
    public class FlashOnHit : MonoBehaviour
    {
        [Tooltip("List of materials to flash. Material needs to enable Emission and have Global Illumination set to 'None'")]
        public List<Material> FlashMaterials;

        [Tooltip("The gradient representing the color of the flash on hit")]
        [GradientUsage(true)]
        public Gradient FlashGradient;

        [Tooltip("The duration of the flash on hit")]
        public float FlashDuration = 0.5f;

        Health m_Health;
        float m_LastTimeDamaged;
        List<RendererData> m_Renderers = new List<RendererData>();
        MaterialPropertyBlock m_FlashMaterialPropertyBlock;

        // Start is called before the first frame update
        void Start()
        {
            m_Health = GetComponent<Health>();

            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {

                    if (FlashMaterials.Contains(renderer.sharedMaterials[i]))
                    {
                        m_Renderers.Add(new RendererData(renderer, i));
                    }
                }
            }

            m_FlashMaterialPropertyBlock = new MaterialPropertyBlock();

            m_Health.onDamage += OnDamaged;
        }

        void OnDisable()
        {
            if(m_Health != null)
            {
                m_Health.onDamage -= OnDamaged;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // flash body on hit
            Color currentColor = FlashGradient.Evaluate((Time.time - m_LastTimeDamaged) / FlashDuration);
            m_FlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);
            foreach (var data in m_Renderers)
            {
                data.Renderer.SetPropertyBlock(m_FlashMaterialPropertyBlock, data.MaterialIndex);
            }
        }

        void OnDamaged(float damage, GameObject source)
        {
            m_LastTimeDamaged = Time.time;
        }


    }
}


