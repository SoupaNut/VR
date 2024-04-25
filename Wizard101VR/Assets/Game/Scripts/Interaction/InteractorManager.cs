using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Game.Shared;

namespace Unity.Game.Interaction
{
    public class InteractorManager : MonoBehaviour
    {
        [Header("Interactor Manager")]
        [Tooltip("Reference to the other hand's Interactor Manager.")]
        public InteractorManager OtherInteractorManager;

        [Header("Interactors")]
        public GameObject TeleportInteractor;
        public GameObject DirectInteractor;
        public GameObject GrabInteractor;
        public float ActivationThreshold = 0.1f;

        public InputActionProperty TeleportInput { get => m_TeleportInput; set => m_TeleportInput = value; }
        public InputActionProperty ActivateInput { get => m_ActivateInput; set => m_ActivateInput = value; }

        public bool IsDirectGrabSelected { get; private set; }
        public bool IsDirectGrabHovered { get; private set; }
        public bool IsTeleportSelected { get; private set; }
        public bool IsGrabRayHovered { get; private set; }
        public bool IsGrabRaySelected { get; private set; }
        public bool IsActivated { get; private set; }

        public GameObject SelectedInteractable { get; private set; }

        // interactors
        XRRayInteractor m_TeleportInteractor;
        XRDirectInteractor m_DirectInteractor;
        XRRayInteractor m_GrabInteractor;

        // inputs
        InputActionProperty m_TeleportInput;
        InputActionProperty m_ActivateInput;

        // Start is called before the first frame update
        void Start()
        {
            // Get Interactors
            {
                m_GrabInteractor = GrabInteractor.GetComponent<XRRayInteractor>();
                DebugUtility.HandleErrorIfNullGetComponent<XRRayInteractor, InteractorManager>(m_GrabInteractor, this, gameObject);

                m_TeleportInteractor = TeleportInteractor.GetComponent<XRRayInteractor>();
                DebugUtility.HandleErrorIfNullGetComponent<XRRayInteractor, InteractorManager>(m_TeleportInteractor, this, gameObject);

                m_DirectInteractor = DirectInteractor.GetComponent<XRDirectInteractor>();
                DebugUtility.HandleErrorIfNullGetComponent<XRDirectInteractor, InteractorManager>(m_DirectInteractor, this, gameObject);
            }

            // Input Actions
            {
                m_TeleportInput = TeleportInteractor.GetComponent<ActionBasedController>().selectAction;
                m_ActivateInput = DirectInteractor.GetComponent<ActionBasedController>().activateAction;
            }

            // Handlers
            {
                m_GrabInteractor.hoverEntered.AddListener(GrabHoverEnteredHandler);
                m_GrabInteractor.hoverExited.AddListener(GrabHoverExitedHandler);
                m_GrabInteractor.selectEntered.AddListener(GrabSelectEnteredHandler);
                m_GrabInteractor.selectExited.AddListener(GrabSelectExitedHandler);

                m_DirectInteractor.hoverEntered.AddListener(DirectHoverEnteredHandler);
                m_DirectInteractor.hoverExited.AddListener(DirectHoverExitedHandler);
                m_DirectInteractor.selectEntered.AddListener(DirectSelectEnteredHandler);
                m_DirectInteractor.selectExited.AddListener(DirectSelectExitedHandler);
            }

        }

        // Update is called once per frame
        void Update()
        {
            // Direct Interactor
            IsDirectGrabSelected = m_DirectInteractor.interactablesSelected.Count > 0;
            IsDirectGrabHovered = m_DirectInteractor.interactablesHovered.Count > 0;

            // Teleport Interactor
            IsTeleportSelected = m_TeleportInput.action.ReadValue<float>() > ActivationThreshold;

            // Grab Ray Interactor
            IsGrabRayHovered = m_GrabInteractor.TryGetHitInfo(out Vector3 position, out Vector3 normal, out int number, out bool isValid);
            IsGrabRaySelected = m_GrabInteractor.interactablesSelected.Count > 0;

            // Activate
            IsActivated = m_ActivateInput.action.ReadValue<float>() > ActivationThreshold;

            HandleInteractors();
        }

        void HandleInteractors()
        {
            // Disable Teleport, Disable Grab Ray
            if (IsDirectGrabSelected)
            {
                TeleportInteractor.SetActive(false);
                GrabInteractor.SetActive(false);
            }
            // Disable Teleport, Disable Grab Ray Line Visual
            else if (IsDirectGrabHovered)
            {
                TeleportInteractor.SetActive(false);
                GrabInteractor.GetComponent<XRInteractorLineVisual>().enabled = false;
            }
            // Enable Teleport, Disable Grab Ray
            else if (IsTeleportSelected && !IsGrabRayHovered && !IsGrabRaySelected)
            {
                TeleportInteractor.SetActive(true);
                GrabInteractor.SetActive(false);
            }
            // Disable Teleport, Enable Grab Ray
            else
            {
                TeleportInteractor.SetActive(false);
                GrabInteractor.SetActive(true);
                GrabInteractor.GetComponent<XRInteractorLineVisual>().enabled = true;
            }
        }

        void GrabHoverEnteredHandler(HoverEnterEventArgs args)
        {
        }

        void GrabHoverExitedHandler(HoverExitEventArgs args)
        {
        }

        void GrabSelectEnteredHandler(SelectEnterEventArgs args)
        {
            if (args.interactableObject.transform.GetComponent<WeaponGrabInteractable>())
            {
                // disable anchor control
                m_GrabInteractor.allowAnchorControl = false;
            }

            SelectedInteractable = args.interactableObject.transform.gameObject;
        }

        void GrabSelectExitedHandler(SelectExitEventArgs args)
        {
            // enable anchor control
            m_GrabInteractor.allowAnchorControl = true;

            SelectedInteractable = null;
        }

        void DirectHoverEnteredHandler(HoverEnterEventArgs args)
        {
        }

        void DirectHoverExitedHandler(HoverExitEventArgs args)
        {
        }

        void DirectSelectEnteredHandler(SelectEnterEventArgs args)
        {
            SelectedInteractable = args.interactableObject.transform.gameObject;
        }

        void DirectSelectExitedHandler(SelectExitEventArgs args)
        {
            SelectedInteractable = null;
        }
    }

}

