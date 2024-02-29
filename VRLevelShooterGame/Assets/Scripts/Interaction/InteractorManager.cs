using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractorManager : MonoBehaviour
{
    [Header("Interactors")]
    public GameObject teleportInteractorObject;
    public GameObject directInteractorObject;
    public GameObject grabInteractorObject;
    public float activationThreshold = 0.1f;

    public bool isActivated {get; private set;}
    public bool objectEnabled {get; private set;}
    
    // interactors
    private XRRayInteractor teleportInteractor;
    private XRDirectInteractor directInteractor;
    private XRRayInteractor grabInteractor;

    // inputs
    private InputActionProperty teleportInput;
    private InputActionProperty directGrabInput;
    private InputActionProperty activateInput;

    // bools
    private bool isDirectGrabSelected;
    private bool isDirectGrabHovered;
    private bool isTeleportSelected;
    private bool isGrabRayHovered;
    private bool isGrabRaySelected;

    // enums
    public ObjectSelected objectSelected {get; private set;}
    public enum ObjectSelected
    {
        Nothing,
        Weapon
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Get Interactors
        grabInteractor = grabInteractorObject.GetComponent<XRRayInteractor>();
        teleportInteractor = teleportInteractorObject.GetComponent<XRRayInteractor>();
        directInteractor = directInteractorObject.GetComponent<XRDirectInteractor>();

        // Setup input variables
        teleportInput = teleportInteractorObject.GetComponent<ActionBasedController>().selectAction;
        directGrabInput = directInteractorObject.GetComponent<ActionBasedController>().selectAction;
        activateInput = directInteractorObject.GetComponent<ActionBasedController>().activateAction;

        // Add Handlers to Interactors
        grabInteractor.selectEntered.AddListener(GrabSelectEnteredHandler);
        grabInteractor.selectExited.AddListener(GrabSelectExitedHandler);
        directInteractor.selectEntered.AddListener(DirectSelectEnteredHandler);
        directInteractor.selectExited.AddListener(DirectSelectExitedHandler);
    }

    // Update is called once per frame
    private void Update()
    {
        isDirectGrabSelected = directInteractor.interactablesSelected.Count > 0;
        isDirectGrabHovered = directInteractor.interactablesHovered.Count > 0;
        isTeleportSelected = teleportInput.action.ReadValue<float>() > activationThreshold;
        isGrabRayHovered = grabInteractor.TryGetHitInfo(out Vector3 position, out Vector3 normal, out int number, out bool isValid);
        isGrabRaySelected = grabInteractor.interactablesSelected.Count > 0;
        isActivated = activateInput.action.ReadValue<float>() > activationThreshold;

        HandleInteractors();
    }

    private void HandleInteractors()
    {
        // Disable Teleport, Disable Grab Ray
        if(isDirectGrabSelected)
        {
            teleportInteractorObject.SetActive(false);
            grabInteractorObject.SetActive(false);
        }
        // Disable Teleport, Disable Gray Ray Line Visual
        else if(isDirectGrabHovered)
        {
            teleportInteractorObject.SetActive(false);
            grabInteractorObject.GetComponent<XRInteractorLineVisual>().enabled = false;
        }
        // Enable Teleport, Disable Grab Ray
        else if(isTeleportSelected && !isGrabRayHovered && !isGrabRaySelected)
        {
            teleportInteractorObject.SetActive(true);
            grabInteractorObject.SetActive(false);
        }
        // Disable Teleport, Enable Grab Ray
        else
        {
            teleportInteractorObject.SetActive(false);
            grabInteractorObject.SetActive(true);
            grabInteractorObject.GetComponent<XRInteractorLineVisual>().enabled = true;
        }
    }

    private void GrabSelectEnteredHandler(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("Weapon"))
        {
            // disable anchor control
            grabInteractor.allowAnchorControl = false;
            objectSelected = ObjectSelected.Weapon;
        }
    }

    private void GrabSelectExitedHandler(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("Weapon"))
        {
            objectSelected = ObjectSelected.Nothing;
        }

        // enable anchor control
        grabInteractor.allowAnchorControl = true;
    }

    private void DirectSelectEnteredHandler(SelectEnterEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("Weapon"))
        {
            objectSelected = ObjectSelected.Weapon;
        }
    }

    private void DirectSelectExitedHandler(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.CompareTag("Weapon"))
        {
            objectSelected = ObjectSelected.Nothing;
        }
    }
}
