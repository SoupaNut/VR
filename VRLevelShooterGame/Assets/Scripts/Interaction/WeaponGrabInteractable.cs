using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Unity.Game.Shared;
using Unity.Game.Utilities;

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

            // Add Handlers
            {
                m_GrabInteractable.selectEntered.AddListener(OnSelectEnteredHandler);
                m_GrabInteractable.selectExited.AddListener(OnSelectExitedHandler);
            }
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
        }

        private void OnSelectEnteredHandler(SelectEnterEventArgs args)
        {
            var interactorManager = args.interactorObject.transform.parent.GetComponent<InteractorManager>();

            if (interactorManager != null)
            {
                m_InteractorManager = interactorManager;
                m_InteractorManager.ActivateInput.action.performed += OnActivateAction;
                m_InteractorManager.ActivateInput.action.performed += OnDeactivateAction;
                m_WeaponController.IsWeaponEnabled = true;
            }
        }

        private void OnSelectExitedHandler(SelectExitEventArgs args)
        {
            var interactorManager = args.interactorObject.transform.parent.GetComponent<InteractorManager>();

            if (interactorManager != null)
            {
                m_InteractorManager.ActivateInput.action.performed -= OnActivateAction;
                m_InteractorManager.ActivateInput.action.performed -= OnDeactivateAction;
                m_InteractorManager = new InteractorManager();
                m_WeaponController.IsWeaponEnabled = false;
            }
        }

        private void OnActivateAction(InputAction.CallbackContext context)
        {
            m_ActivatePressed = true;
            m_ActivateReleased = false;
        }

        private void OnDeactivateAction(InputAction.CallbackContext context)
        {
            m_ActivateReleased = false;
            m_ActivateReleased = true;
        }
    }
}

