using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Game.Utilities;

namespace Unity.Game.Interaction
{
    public class InputManager : MonoBehaviour
    {
        public ActionBasedController TeleportInteractor;
        public ActionBasedController DirectInteractor;
        public ActionBasedController RayInteractor;
        public float ActivationThreshold = 0.1f;

        public InputActionProperty TeleportInput { get => m_TeleportInput; set => m_TeleportInput = value; }
        public InputActionProperty ActivateInput { get => m_ActivateInput; set => m_ActivateInput = value; }

        public bool IsDirectGrabSelected { get; private set; }
        public bool IsDirectGrabHovered { get; private set; }
        public bool IsTeleportSelected { get; private set; }
        public bool IsGrabRayHovered { get; private set; }
        public bool IsGrabRaySelected { get; private set; }
        public bool IsActivated { get; private set; }

        // inputs
        InputActionProperty m_TeleportInput;
        InputActionProperty m_ActivateInput;


        // Start is called before the first frame update
        void Start()
        {
            m_TeleportInput = TeleportInteractor.selectAction;
            DebugUtility.HandleErrorIfNullInputActionProperty<InputManager>(m_TeleportInput, this, gameObject);

            m_ActivateInput = DirectInteractor.activateAction;
            DebugUtility.HandleErrorIfNullInputActionProperty<InputManager>(m_ActivateInput, this, gameObject);

        }

        // Update is called once per frame
        void Update()
        {
            IsTeleportSelected = m_TeleportInput.action.ReadValue<float>() > ActivationThreshold;
            IsActivated = m_ActivateInput.action.ReadValue<float>() > ActivationThreshold;
        }
    }
}


