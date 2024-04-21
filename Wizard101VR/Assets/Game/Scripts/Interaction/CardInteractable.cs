using System.Collections;
using System.Collections.Generic;
using Unity.Game.Interaction;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.Game.Gameplay
{
    public class CardInteractable : XRSimpleInteractable
    {
        public CardData SpellCardData;

        public bool IsCardSelected { get; private set; }

        Outline m_Outline;


        // Start is called before the first frame update
        void Start()
        {
            IsCardSelected = false;

            m_Outline = GetComponent<Outline>();
            if (m_Outline)
            {
                m_Outline.enabled = false;
            }
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

            // Toggle outline
            if(m_Outline)
            {
                m_Outline.enabled = IsCardSelected;
            }

            base.OnSelectEntered(args);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            //IsCardSelected = false;
            //SetSpellToCast(args, null);

            base.OnSelectExited(args);
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


