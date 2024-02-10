using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractorManager : MonoBehaviour
{
    public GameObject teleportInteractor;
    public GameObject directInteractor;
    public GameObject grabInteractor;
    public float activationThreshold = 0.1f;


    private InputActionProperty teleportInput;
    private InputActionProperty directGrabInput;
    private bool isDirectGrabSelected;
    private bool isTeleportSelected;
    private bool isGrabRayHovered;
    private bool isGrabRaySelected;

    // Start is called before the first frame update
    void Start()
    {
        teleportInput = teleportInteractor.GetComponent<ActionBasedController>().selectAction;
        directGrabInput = directInteractor.GetComponent<ActionBasedController>().selectAction;
    }

    // Update is called once per frame
    void Update()
    {
        isDirectGrabSelected = directInteractor.GetComponent<XRDirectInteractor>().interactablesSelected.Count > 0;
        isTeleportSelected = teleportInput.action.ReadValue<float>() > activationThreshold;
        isGrabRayHovered = grabInteractor.GetComponent<XRRayInteractor>().TryGetHitInfo(out Vector3 position, out Vector3 normal, out int number, out bool isValid);
        isGrabRaySelected = grabInteractor.GetComponent<XRRayInteractor>().interactablesSelected.Count > 0;

        HandleInteractors();
    }

    private void HandleInteractors()
    {
        // Disable Teleport, Disable Grab Ray
        if(isDirectGrabSelected)
        {
            teleportInteractor.SetActive(false);
            grabInteractor.SetActive(false);
        }
        // Enable Teleport, Disable Grab Ray
        else if(isTeleportSelected && !isGrabRayHovered && !isGrabRaySelected)
        {
            teleportInteractor.SetActive(true);
            grabInteractor.SetActive(false);
        }
        // Disable Teleport, Enable Grab Ray
        else
        {
            teleportInteractor.SetActive(false);
            grabInteractor.SetActive(true);
        }
    }
}
