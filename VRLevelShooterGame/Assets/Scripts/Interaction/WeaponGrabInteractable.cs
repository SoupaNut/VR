using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Game.Shared;

public class WeaponGrabInteractable : XRGrabInteractableTwoAttach
{
    public WeaponController controller;
    private InteractorManager m_InteractorManager;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable weapon = GetComponent<XRGrabInteractable>();

        // Handle when an interactor selects weapon
        weapon.selectEntered.AddListener(GetInteractorManager);
        weapon.selectExited.AddListener(ClearInteractorManager);

        if (controller.ShootType == WeaponController.WeaponShootType.Manual)
        {
            weapon.activated.AddListener(ShootProjectile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_InteractorManager != null && controller.ShootType == WeaponController.WeaponShootType.Automatic)
        {
            controller.SetReadyToFire(m_InteractorManager.isActivated);
        }
    }

    private void ShootProjectile(ActivateEventArgs args)
    {
        controller.Fire();
    }

    private void GetInteractorManager(SelectEnterEventArgs args)
    {
        var interactorManager = args.interactorObject.transform.parent.gameObject.GetComponent<InteractorManager>();

        if (interactorManager != null)
        {
            m_InteractorManager = interactorManager;
        }
    }

    private void ClearInteractorManager(SelectExitEventArgs args)
    {
        // clear reference
        m_InteractorManager = new InteractorManager();
    }
}
