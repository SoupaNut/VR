using UnityEngine;
using UnityEngine.UI;
using Unity.Game.Utilities;
using Unity.Game.Shared;

namespace Unity.Game.UI
{
    public class AmmoBar : MonoBehaviour
    {
        [Tooltip("Game Object of Ammo Bar to toggle")]
        public GameObject AmmoBarObject;

        [Tooltip("Amount of time to wait before starting the fade out")]
        public float FadeOutDelay = 2f;

        [Tooltip("Amount of time to fade out")]
        public float FadeOutDuration = 1f;

        [Tooltip("Amount of time to wait before starting the fade int")]
        public float FadeInDelay = 2f;

        [Tooltip("Amount of time to fade in")]
        public float FadeInDuration = 1f;

        Animator m_Animator;
        WeaponController m_WeaponController;
        Slider m_Slider;
        float m_LastActiveTime = Mathf.NegativeInfinity;
        float m_LastAmmoRatio = 1f;
        bool m_AnimationDone = true;
        bool m_AnimationSetThisFrame;

        void Start()
        {
            // Get Components
            {
                m_Animator = GetComponent<Animator>();
                DebugUtility.HandleErrorIfNullGetComponent<Animator, AmmoBar>(m_Animator, this, gameObject);

                m_WeaponController = GetComponentInParent<WeaponController>();
                DebugUtility.HandleErrorIfNullGetComponent<WeaponController, AmmoBar>(m_WeaponController, this, gameObject);

                m_Slider = GetComponentInChildren<Slider>();
                DebugUtility.HandleErrorIfNullGetComponent<Slider, AmmoBar>(m_Slider, this, gameObject);
            }

            //// Subscribe to Events
            //{
            //    m_WeaponController.onShoot += OnShoot;
            //}
            
        }

        void Update()
        {
            // Show Ammo Bar if we are either reloading or shooting
            //float currentAmmoRatio = m_WeaponController.CurrentAmmoRatio;

            //if(currentAmmoRatio != m_LastAmmoRatio)
            //{
            //    m_LastActiveTime = Time.time;
            //}

            //if (currentAmmoRatio != 1f)
            //{

            //}

            //m_LastAmmoRatio = currentAmmoRatio;

            if (m_WeaponController.IsWeaponEnabled)
            {
                if (m_Slider.value == m_Slider.maxValue)
                {
                    // check if this is a different trigger
                    if(!m_Animator.GetBool("FadeOut") && !m_AnimationSetThisFrame)
                    {
                        m_Animator.SetTrigger("FadeOut");
                        //m_AnimationSetThisFrame = true;
                        Debug.Log("Fade Out");
                    }
                }
                else
                {
                    if (!m_Animator.GetBool("FadeIn") && !m_AnimationSetThisFrame)
                    {
                        m_Animator.SetTrigger("FadeIn");
                        //m_AnimationSetThisFrame = true;
                        Debug.Log("Fade In");
                    }
                }

                SetAmmoRatio(m_WeaponController.CurrentAmmoRatio);

                m_LastAmmoRatio = m_WeaponController.CurrentAmmoRatio;
                //Debug.Log(m_Slider.value);
            }
        }
        public void AnimationStart()
        {
            m_AnimationSetThisFrame = false;
        }

        public void AnimationDone()
        {
            m_AnimationSetThisFrame = false;
        }

        public void SetAmmoRatio(float ammoRatio)
        {
            m_Slider.value = ammoRatio;
        }

        //private void OnShoot()
        //{
        //    SetAmmoRatio(m_WeaponController.CurrentAmmoRatio);
        //}
    }
}


