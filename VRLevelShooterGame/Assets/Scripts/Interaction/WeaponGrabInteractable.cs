using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Game.Shared;

namespace Unity.Game.Interaction
{
    [RequireComponent(typeof(WeaponController))]
    public class WeaponGrabInteractable : XRGrabInteractableTwoAttach
    {
        private WeaponController m_WeaponController;
        private InteractorManager m_InteractorManager;

        // Start is called before the first frame update
        void Start()
        {
            // Get Components
            {
                XRGrabInteractable weapon = GetComponent<XRGrabInteractable>(); // TODO: Change this?

                m_WeaponController = GetComponent<WeaponController>();
                DebugUtility.HandleErrorIfNullGetComponent<WeaponController, WeaponGrabInteractable>(m_WeaponController, this, gameObject);
            }
            

            // Handle when an interactor selects weapon
            weapon.selectEntered.AddListener(OnSelectEnteredHandler);
            weapon.selectExited.AddListener(OnSelectExitedHandler);

            if (m_WeaponController.ShootType == WeaponController.WeaponShootType.Manual)
            {
                weapon.activated.AddListener(ShootProjectile);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (m_InteractorManager != null && m_WeaponController.ShootType == WeaponController.WeaponShootType.Automatic)
            {
                m_WeaponController.SetReadyToFire(m_InteractorManager.IsActivated);
            }
        }

        private void ShootProjectile(ActivateEventArgs args)
        {
            m_WeaponController.Fire();
        }

        private void OnSelectEnteredHandler(SelectEnterEventArgs args)
        {
            var interactorManager = args.interactorObject.transform.parent.gameObject.GetComponent<InteractorManager>();

            if (interactorManager != null)
            {
                m_InteractorManager = interactorManager;
            }
        }

        private void OnSelectExitedHandler(SelectExitEventArgs args)
        {
            // clear reference
            m_InteractorManager = new InteractorManager();
        }
    }
}

