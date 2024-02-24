using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UniqueObjectSocketInteractor : XRSocketInteractor
{
    [SerializeField]
    private GameObject uniqueObject;

    [SerializeField]
    [Tooltip("This needs to be the renderer of the object and HAS to be on a child game object.")]
    private Transform uniqueObjectTransform;

    [SerializeField]
    private float selectedScaleFactor = 2f;

    private int uniqueId;
    private Vector3 originalObjectScale;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // get ID of object
        uniqueId = uniqueObject.GetInstanceID();

        // Get original scale of object
        originalObjectScale = uniqueObjectTransform.localScale;
    }

    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        XRGrabInteractable grabbable = (XRGrabInteractable) interactable;
        int interactableId = grabbable.gameObject.GetInstanceID();

        return base.CanSelect(interactable) && (interactableId == uniqueId);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        uniqueObjectTransform.localScale = originalObjectScale * selectedScaleFactor;
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        uniqueObjectTransform.localScale = originalObjectScale;
        base.OnSelectExited(args);
    }


}