using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Game.Shared;

namespace Unity.Game.Interaction
{
    public class AnimateHandsOnInput : MonoBehaviour
    {
        public InputActionProperty PinchInput;
        public InputActionProperty GripInput;
        public Animator HandAnimator;

        void Start()
        {
            if(HandAnimator == null)
            {
                HandAnimator = GetComponent<Animator>();
                DebugUtility.HandleErrorIfNullGetComponent<Animator, AnimateHandsOnInput>(HandAnimator, this, gameObject);
            }

            if (PinchInput.reference == null)
            {
                PinchInput = GetComponentInParent<ActionBasedController>().activateAction;
                DebugUtility.HandleErrorIfNullInputActionProperty<AnimateHandsOnInput>(PinchInput, this, gameObject);
            }

            if(GripInput.reference == null)
            {
                GripInput = GetComponentInParent<ActionBasedController>().selectAction;
                DebugUtility.HandleErrorIfNullInputActionProperty<AnimateHandsOnInput>(GripInput, this, gameObject);
            }

        }

        // Update is called once per frame
        void Update()
        {
            // read values of inputs
            float triggerValue = PinchInput.action.ReadValue<float>();
            float gripValue = GripInput.action.ReadValue<float>();

            // set value of trigger and grip on hand
            HandAnimator.SetFloat("Trigger", triggerValue);
            HandAnimator.SetFloat("Grip", gripValue);

        }
    }
}


