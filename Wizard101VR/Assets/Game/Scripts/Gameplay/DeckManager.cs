using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Game.Shared;
using Unity.Game.Interaction;
using UnityEngine.InputSystem;

namespace Unity.Game.Gameplay
{
    public class DeckManager : MonoBehaviour
    {
        public InputActionProperty DeckButton;
        public List<CardInteractable> DisplayCards;
        public List<CardData> AllSpells;
        
        public DeckDisplay DeckDisplay { get; private set; }
        public bool DeckEnabled { get; set; }

        CardInteractable SelectedCard;

        void Start()
        {
            DeckDisplay = GetComponent<DeckDisplay>();
            DebugUtility.HandleErrorIfNullGetComponent<DeckDisplay, DeckManager>(DeckDisplay, this, gameObject);

            Shuffle();

            foreach(CardInteractable card in DisplayCards)
            {
                // Add Handler
                card.onCardSelect += CardSelectHandler;

                // Draw starting hand
                card.Load(DrawCard());
            }

            DeckDisplay.Display.SetActive(false);
            DeckButton.action.performed += DeckButtonPressed;
        }

        void CardSelectHandler(CardInteractable card)
        {
            // we don't have a card selected
            if(SelectedCard == null)
            {
                SetSelectedCard(card);
            }
            // we have a different card selected
            else if(SelectedCard != card)
            {
                ClearSelectedCard();
                SetSelectedCard(card);
            }
            // we have selected the same card
            else
            {
                ClearSelectedCard();
            }
        }

        public void ClearSelectedCard()
        {
            if(SelectedCard != null)
            {
                SelectedCard.CardDisplay.DisplayCardOutline(false);
                SelectedCard.SpellcastManager.SetSpellToCast(null);
                SelectedCard = null;
            }
        }

        public void SetSelectedCard(CardInteractable card)
        {
            SelectedCard = card;
            SelectedCard.SpellcastManager.SetSpellToCast(card.SpellCardData);
            SelectedCard.CardDisplay.DisplayCardOutline(true);
        }

        public CardData DrawCard()
        {
            if(AllSpells.Count == 0)
                return null;

            CardData drawnCard = AllSpells[0];
            AllSpells.RemoveAt(0);
            return drawnCard;
        }

        public void Shuffle()
        {
            System.Random rng = new System.Random();
            int n = AllSpells.Count;
            while(n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                CardData value = AllSpells[k];
                AllSpells[k] = AllSpells[n];
                AllSpells[n] = value;
            }
        }

        void DeckButtonPressed(InputAction.CallbackContext context)
        {
            if (DeckEnabled)
            {
                DeckDisplay.Toggle();

                // deselect card if display is inactive
                if (!DeckDisplay.Display.activeSelf)
                    ClearSelectedCard();
            }
        }
    }
}