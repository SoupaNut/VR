using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AnchorControlOnSelect : MonoBehaviour
{
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    void Start()
    {
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(DisableAnchor);
        GetComponent<XRGrabInteractable>().selectExited.AddListener(EnableAnchor);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DisableAnchor(SelectEnterEventArgs args)
    {
        if (args.interactorObject.transform.CompareTag("Left Hand"))
        {
            leftRayInteractor.allowAnchorControl = false;
        }
        else if (args.interactorObject.transform.CompareTag("Right Hand"))
        {
            rightRayInteractor.allowAnchorControl = false;
        }
    }

    private void EnableAnchor(SelectExitEventArgs args)
    {
        leftRayInteractor.allowAnchorControl = true;
        rightRayInteractor.allowAnchorControl = true;
    }
}
