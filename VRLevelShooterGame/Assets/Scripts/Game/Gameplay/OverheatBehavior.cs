using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Shared;
using Unity.Game.Utilities;

namespace Unity.Game.Gameplay
{
    [RequireComponent(typeof(WeaponController))]
    public class OverheatBehavior : MonoBehaviour
    {
        [System.Serializable]
        public struct RendererIndexData
        {
            public Renderer Renderer;
            public int MaterialIndex;

            public RendererIndexData(Renderer renderer, int index)
            {
                this.Renderer = renderer;
                this.MaterialIndex = index;
            }
        }

        [Header("Visual")]
        [Tooltip("The VFX to scale the spawn rate based on the ammo ratio")]
        public ParticleSystem SteamVfx;

        [Tooltip("The emission rate for the effect when fully overheated")]
        public float SteamVfxEmissionRateMax = 8f;

        //Set gradient field to HDR
        [GradientUsage(true)]
        [Tooltip("Overheat color based on ammo ratio")]
        public Gradient OverheatGradient;

        [Tooltip("The material for overheating color animation")]
        public Material OverheatingMaterial;

        [Header("Sound")]
        [Tooltip("Sound played when a cell are cooling")]
        public AudioClip CoolingCellsSound;

        [Tooltip("Curve for ammo to volume ratio")]
        public AnimationCurve AmmoToVolumeRatioCurve;

        WeaponController m_Weapon;
        AudioSource m_AudioSource;
        List<RendererIndexData> m_OverheatingRenderersData;
        MaterialPropertyBlock m_OverheatMaterialPropertyBlock;
        ParticleSystem.EmissionModule m_SteamVfxEmissionModule;
        float m_LastAmmoRatio;

        void Awake()
        {
            m_OverheatingRenderersData = new List<RendererIndexData>();
            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (renderer.sharedMaterials[i] == OverheatingMaterial)
                        m_OverheatingRenderersData.Add(new RendererIndexData(renderer, i));
                }
            }

            m_OverheatMaterialPropertyBlock = new MaterialPropertyBlock();
            m_SteamVfxEmissionModule = SteamVfx.emission;
            m_SteamVfxEmissionModule.rateOverTimeMultiplier = 0f;

            // Get weapon
            m_Weapon = GetComponent<WeaponController>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, OverheatBehavior>(m_Weapon, this, gameObject);

            // Get Cool Cells Sound
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.clip = CoolingCellsSound;
            m_AudioSource.spatialBlend = 1f;
            m_AudioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.WeaponOverheat);
        }

        void Update()
        {
            // visual smoke coming out of gun
            float currentAmmoRatio = m_Weapon.CurrentAmmoRatio;
            if(currentAmmoRatio != m_LastAmmoRatio)
            {
                m_OverheatMaterialPropertyBlock.SetColor("_EmissionColor",
                    OverheatGradient.Evaluate(1f - currentAmmoRatio));

                foreach (var data in m_OverheatingRenderersData)
                {
                    data.Renderer.SetPropertyBlock(m_OverheatMaterialPropertyBlock, data.MaterialIndex);
                }

                m_SteamVfxEmissionModule.rateOverTimeMultiplier = SteamVfxEmissionRateMax * (1f - currentAmmoRatio);
            }

            // cooling sound
            if(CoolingCellsSound)
            {
                if (!m_AudioSource.isPlaying && m_Weapon.IsReloading && currentAmmoRatio != 1f && m_Weapon.IsWeaponEnabled)
                {
                    m_AudioSource.Play();
                }
                else if (m_AudioSource.isPlaying && (currentAmmoRatio == 1f || !m_Weapon.IsReloading || !m_Weapon.IsWeaponEnabled))
                {
                    m_AudioSource.Stop();
                    //return;
                }

                m_AudioSource.volume = AmmoToVolumeRatioCurve.Evaluate(1 - currentAmmoRatio);
            }

            m_LastAmmoRatio = currentAmmoRatio;
        }
    }
}


