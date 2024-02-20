using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.Scripts.Gameplay
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Interactors")]
        public XRRayInteractor LeftTeleportInteractor;
        public XRRayInteractor RightTeleportInteractor;
        public XRDirectInteractor LeftDirectInteractor;
        public XRDirectInteractor RightDirectInteractor;
        public XRRayInteractor LeftGrabInteractor;
        public XRRayInteractor RightGrabInteractor;
        public float activationThreshold = 0.1f;

        // ----- Inputs -----
        // Direct Interactor
        private InputActionProperty m_LeftDirectGrabInput;
        public InputActionProperty LeftDirectGrabInput { get => m_LeftDirectGrabInput; private set; }

        private InputActionProperty m_RightDirectGrabInput;
        public InputActionProperty RightDirectGrabInput { get => m_RightDirectGrabInput; private set; }

        private InputActionProperty m_LeftDirectActivateInput;
        public InputActionProperty LeftDirectActivateInput { get => m_LeftDirectActivateInput; private set; }

        private InputActionProperty m_RightDirectActivateInput;
        public InputActionProperty RightDirectActivateInput { get => m_RightDirectActivateInput; private set; }

        // Teleport Interactor
        private InputActionProperty m_LeftTeleportInput;
        public InputActionProperty LeftTeleportInput { get => m_LeftTeleportInput; private set; }

        private InputActionProperty m_RightTeleportInput;
        public InputActionProperty RightTeleportInput { get => m_RightTeleportInput; private set; }

        // Ray Interactor
        private InputActionProperty m_LeftRayGrabInput;
        public InputActionProperty LeftRayGrabInput { get => m_LeftRayGrabInput; private set; }

        private InputActionProperty m_RightRayGrabInput;
        public InputActionProperty RightRayActivateInput { get => m_RightRayGrabInput; private set; }

        private InputActionProperty m_LeftRayActivateInput;
        public InputActionProperty LeftRayActivateInput { get => m_LeftRayActivateInput; private set; }

        private InputActionProperty m_RightRayActivateInput;
        public InputActionProperty RightRayActivateInput { get => m_RightRayActivateInput; private set; }

        // ----- Bools -----
        // Direct Interactor
        private bool m_IsLeftDirectSelected;
        public bool IsLeftDirectSelected { get => m_IsLeftDirectSelected; private set; }

        private bool m_IsRightDirectSelected;
        public bool IsRightDirectSelected { get => m_IsRightDirectSelected; private set; }

        private bool m_IsLeftDirectActivated;
        public bool IsLeftDirectActivated { get => m_IsLeftDirectActivated; private set; }

        private bool m_IsRightDirectActivated;
        public bool IsRightDirectActivated { get => m_IsRightDirectActivated; private set; }

        private bool m_IsLeftDirectHovered;
        public bool IsLeftDirectHovered { get => m_IsLeftDirectHovered; private set; }

        private bool m_IsRightDirectHovered;
        public bool IsRightDirectHovered { get => m_IsRightDirectHovered; private set; }

        // Teleport Interactor
        private bool m_IsLeftTeleportSelected;
        public bool IsLeftTeleportSelected { get => m_IsLeftTeleportSelected; private set; }

        private bool m_IsRightTeleportSelected;
        public bool IsRightTeleportSelected { get => m_IsRightTeleportSelected; private set; }

        // Ray Interactor
        private bool m_IsLeftRayHovered;
        public bool IsLeftRayHovered { get => m_IsLeftRayHovered; private set; }

        private bool m_IsRightRayHovered;
        public bool IsRightRayHovered { get => m_IsRightRayHovered; private set; }

        private bool m_IsLeftRaySelected;
        public bool IsLeftRaySelected { get => m_IsLeftRaySelected; private set; }

        private bool m_IsRightRaySelected;
        public bool IsRightRaySelected { get => m_IsRightRaySelected; private set; }

        private bool m_IsLeftRayActivated;
        public bool IsLeftRayActivated { get => m_IsLeftRayActivated; private set; }

        private bool m_IsRightRayActivated;
        public bool IsRightRayActivated { get => m_IsRightRayActivated; private set; }



        private void Start()
        {
            // Direct
            m_LeftDirectGrabInput = LeftDirectInteractor.gameObject.GetComponent<ActionBasedController>().selectAction;
            m_RightDirectGrabInput = RightDirectInteractor.gameObject.GetComponent<ActionBasedController>().selectAction;

            m_LeftDirectActivateInput = LeftDirectInteractor.gameObject.GetComponent<ActionBasedController>().activateAction;
            m_RightDirectActivateInput = RightDirectInteractor.gameObject.GetComponent<ActionBasedController>().activateAction;

            // Teleport
            m_LeftTeleportInput = LeftTeleportInteractor.gameObject.GetComponent<ActionBasedController>().selectAction;
            m_RightTeleportInput = RightTeleportInteractor.gameObject.GetComponent<ActionBasedController>().selectAction;

            // Ray
            m_LeftRayGrabInput = LeftGrabInteractor.gameObject.GetComponent<ActionBasedController>().selectAction;
            m_RightRayGrabInput = RightGrabInteractor.gameObject.GetComponent<ActionBasedController>().selectAction;

            m_LeftRayActivateInput = LeftGrabInteractor.gameObject.GetComponent<ActionBasedController>().activateAction;
            m_RightRayActivateInput = RightGrabInteractor.gameObject.GetComponent<ActionBasedController>().activateAction;
        }

        private void Update()
        {
            // Direct
            m_IsLeftDirectSelected = LeftDirectInteractor.interactablesSelected.Count > 0;
            m_IsRightDirectSelected = RightDirectInteractor.interactablesSelected.Count > 0;

            m_IsLeftDirectActivated = m_LeftDirectActivateInput.action.ReadValue<float>() > activationThreshold;
            m_IsRightDirectActivated = m_RightDirectActivateInput.action.ReadValue<float>() > activationThreshold;

            m_IsLeftDirectHovered = LeftDirectInteractor.interactablesHovered.Count > 0;
            m_IsRightDirectHovered = RightDirectInteractor.interactablesHovered.Count > 0;

            // Teleport
            m_IsLeftTeleportSelected = m_LeftTeleportInput.action.ReadValue<float>() > activationThreshold;
            m_IsRightTeleportSelected = m_RightTeleportInput.action.ReadValue<float>() > activationThreshold;

            // Ray
            m_IsLeftRayHovered = LeftGrabInteractor.TryGetHitInfo(out Vector3 position, out Vector3 normal, out int number, out bool isValid);
            m_IsRightRayHovered = RightGrabInteractor.TryGetHitInfo(out Vector3 position, out Vector3 normal, out int number, out bool isValid);

            m_IsLeftRaySelected = LeftGrabInteractor.interactablesSelected.Count > 0;
            m_IsRightRaySelected = RightGrabInteractor.interactablesSelected.Count > 0;

            m_IsLeftRayActivated = m_LeftRayActivateInput.action.ReadValue<float>() > activationThreshold;
            m_IsRightRayActivated = m_RightRayActivateInput.action.ReadValue<float>() > activationThreshold;
        }
    }
}