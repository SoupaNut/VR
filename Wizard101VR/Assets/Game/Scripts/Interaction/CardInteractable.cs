using Unity.Game.Interaction;
using Unity.Game.Shared;
using Unity.Game.UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.Game.Gameplay
{
    public class CardInteractable : XRSimpleInteractable
    {
        public CardData SpellCardData;

        public bool IsCardSelected { get; private set; }

        CardDisplay m_CardDisplay;

        void Start()
        {
            m_CardDisplay = GetComponentInChildren<CardDisplay>();
            DebugUtility.HandleErrorIfNullGetComponent<CardDisplay, CardInteractable>(m_CardDisplay, this, gameObject);

            m_CardDisplay.Load(SpellCardData);
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            if(IsCardSelected)
            {
                IsCardSelected = false;
                SetSpellToCast(args, null);
            }
            else
            {
                IsCardSelected = true;
                SetSpellToCast(args, SpellCardData);
            }

            m_CardDisplay.DisplayCardOutline(IsCardSelected);

            base.OnSelectEntered(args);
        }

        void SetSpellToCast(BaseInteractionEventArgs args, CardData spellToCast)
        {
            // Check if we have an interactor
            var interactor = args.interactorObject.transform.parent.GetComponent<InteractorManager>();

            if (interactor)
            {
                // Check if we have an interactable selected on the other hand
                var interactable = interactor.OtherInteractorManager.SelectedInteractable;

                if (interactable != null)
                {
                    var spellcastManager = interactable.GetComponent<SpellcastManager>();

                    if (spellcastManager)
                    {
                        spellcastManager.SetSpellToCast(spellToCast);
                    }
                }
            }
        }
    }
}


