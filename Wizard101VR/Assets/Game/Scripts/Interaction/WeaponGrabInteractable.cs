using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.Game.Interaction
{
    public class WeaponGrabInteractable : XRGrabInteractableTwoAttach
    {
        public InteractorManager InteractorManager { get; set; }
        public bool IsWeaponEnabled { get; private set; }

        Outline m_Outline;

        void Start()
        {
            m_Outline = GetComponent<Outline>();
            if(m_Outline)
            {
                m_Outline.enabled = false;
            }
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            m_Outline.enabled = !isSelected;

            base.OnHoverEntered(args);
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            m_Outline.enabled = false;
            base.OnHoverExited(args);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            var interactorManager = args.interactorObject.transform.parent.GetComponent<InteractorManager>();

            if (interactorManager != null)
            {
                InteractorManager = interactorManager;
                IsWeaponEnabled = true;
            }

            base.OnSelectEntered(args);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            InteractorManager = null;
            IsWeaponEnabled = false;

            base.OnSelectExited(args);
        }
    }
}