using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandsOnInput : MonoBehaviour
{
    public InputActionProperty pinchInput;
    public InputActionProperty gripInput;
    public Animator handAnimator;

    // Update is called once per frame
    void Update()
    {
        // read values of inputs
        float triggerValue = pinchInput.action.ReadValue<float>();
        float gripValue = gripInput.action.ReadValue<float>();

        // set value of trigger and grip on hand
        handAnimator.SetFloat("Trigger", triggerValue);
        handAnimator.SetFloat("Grip", gripValue);
        
    }
}
