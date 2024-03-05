using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Game.Shared;
using System.Diagnostics;

namespace Unity.Game.Interaction
{
    [RequireComponent(typeof(WeaponController))]
    public class WeaponGrabInteractable : XRGrabInteractableTwoAttach
    {
        XRGrabInteractable m_GrabInteractable;
        WeaponController m_WeaponController;
        InteractorManager m_InteractorManager = new InteractorManager();
        bool m_ActivatePressed = false;
        bool m_ActivateReleased = false;

        // Start is called before the first frame update
        void Start()
        {
            // Get Components
            {
                m_GrabInteractable = GetComponent<XRGrabInteractable>();
                DebugUtility.HandleErrorIfNullGetComponent<XRGrabInteractable, WeaponGrabInteractable>(m_GrabInteractable, this, gameObject);

                m_WeaponController = GetComponent<WeaponController>();
                DebugUtility.HandleErrorIfNullGetComponent<WeaponController, WeaponGrabInteractable>(m_WeaponController, this, gameObject);
            }


            // Handle when an interactor selects weapon
            m_GrabInteractable.selectEntered.AddListener(OnSelectEnteredHandler);
            m_GrabInteractable.selectExited.AddListener(OnSelectExitedHandler);

            //if (m_WeaponController.ShootType == WeaponController.WeaponShootType.Manual)
            //{
            //    weapon.activated.AddListener(ShootProjectile);
            //}
        }

        // Update is called once per frame
        void Update()
        {
            if(m_InteractorManager != null)
            {
                m_WeaponController.HandleShootInputs(m_ActivatePressed, m_InteractorManager.IsActivated, m_ActivateReleased);

                if(m_ActivatePressed)
                {
                    m_ActivatePressed = false;
                }

                if(m_ActivateReleased)
                {
                    m_ActivateReleased = false;
                }
            }
            //if (m_InteractorManager != null && m_WeaponController.ShootType == WeaponController.WeaponShootType.Automatic)
            //{
            //    //m_WeaponController.SetReadyToFire(m_InteractorManager.IsActivated);
            //    m_WeaponController.HandleShootInputs(false, m_InteractorManager.IsActivated, false);
            //}
        }

        //private void ShootProjectile()
        //{
        //    //m_WeaponController.Fire();
        //    m_WeaponController.TryShoot();
        //}

        private void OnSelectEnteredHandler(SelectEnterEventArgs args)
        {
            var interactorManager = args.interactorObject.transform.parent.GetComponent<InteractorManager>();

            if (interactorManager != null)
            {
                m_InteractorManager = interactorManager;
                m_InteractorManager.ActivateInput.action.performed += OnActivateAction;
                m_InteractorManager.ActivateInput.action.performed += OnDeactivateAction;
            }
        }

        private void OnSelectExitedHandler(SelectExitEventArgs args)
        {
            m_InteractorManager.ActivateInput.action.performed -= OnActivateAction;
            m_InteractorManager.ActivateInput.action.performed -= OnDeactivateAction;
            m_InteractorManager = new InteractorManager();
        }

        private void OnActivateAction(InputAction.CallbackContext context)
        {
            Debug.Log("Activate Pressed");
            m_ActivatePressed = true;
            m_ActivateReleased = false;
        }

        private void OnDeactivateAction(InputAction.CallbackContext context)
        {
            Debug.Log("Activate Released");
            m_ActivateReleased = false;
            m_ActivateReleased = true;
        }
    }
}

