using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.Events;
using Unity.Game.Shared;
using Unity.Game.Gameplay;

namespace Unity.Game.Interaction
{
    [RequireComponent(typeof(CardDisplay))]
    public class CardInteractable : XRSimpleInteractable
    {
        public SpellData SpellCardData;
        public UnityAction<CardInteractable> onCardSelect;

        public SpellcastManager SpellcastManager { get; set; }
        public CardDisplay CardDisplay { get; set; }
        public bool IsCardSelected { get; private set; }
        float m_TimeThreshold = 0.25f;
        float LastEventTime;

        void Start()
        {
            CardDisplay = GetComponent<CardDisplay>();
            DebugUtility.HandleErrorIfNullGetComponent<CardDisplay, CardInteractable>(CardDisplay, this, gameObject);
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            // only trigger hover entered event if a certain amount of time has passed from the previous hover enter event
            float currentTime = Time.time;

            if(currentTime - LastEventTime > m_TimeThreshold)
            {
                base.OnHoverEntered(args);
            }

            LastEventTime = currentTime;

        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            SpellcastManager = null;
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
                        SpellcastManager = spellcastManager;
                        onCardSelect?.Invoke(this);
                    }
                }
            }

            base.OnSelectEntered(args);
        }

        public void Load(SpellData spell)
        {
            SpellCardData = spell;
            CardDisplay.Load(spell);
        }
    }
}


