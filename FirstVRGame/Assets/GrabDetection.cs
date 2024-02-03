using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class GrabDetection : MonoBehaviour
{
    public XRBaseController controller;
    public XRBaseInteractor interactor;
    private bool isGrabbing;
    private XRInteractorLineVisual lineVisual;
    private InputActionProperty select;
    List<IXRSelectInteractable> currentlyGrabbedObjects;

    // Start is called before the first frame update
    private void Start()
    {
        select = controller.GetComponent<ActionBasedController>().selectAction;
        lineVisual = interactor.GetComponent<XRInteractorLineVisual>();
    }

    private void Update()
    {
        CheckGrabbing();

        // activate/deactivate teleport ray
        lineVisual.enabled = !isGrabbing && select.action.ReadValue<float>() > 0.1f;
    }

    private void CheckGrabbing()
    {
        // get list of all currently interacted objects
        currentlyGrabbedObjects = interactor.interactablesSelected;

        // if list is not null
        if (currentlyGrabbedObjects != null && currentlyGrabbedObjects.Count > 0)
        {
            // check if the object is a grabbable
            isGrabbing = currentlyGrabbedObjects[0] is XRGrabInteractable;
        }
        else
        {
            isGrabbing = false;
        }
    }
}
