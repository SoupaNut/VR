using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.Game.Interaction
{
    public class WeaponGrabInteractable : XRGrabInteractableTwoAttach
    {
        public InteractorManager InteractorManager { get; set; }
        public bool IsWeaponEnabled { get; private set; }

        void Start()
        {
            var outline = GetComponent<Outline>();
            if(outline)
            {
                outline.enabled = false;
            }
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
            InteractorManager = new InteractorManager();
            IsWeaponEnabled = false;

            base.OnSelectExited(args);
        }
    }
}