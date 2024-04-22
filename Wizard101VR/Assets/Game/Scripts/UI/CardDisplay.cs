using TMPro;
using Unity.Game.Gameplay;
using Unity.Game.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Game.UI
{
    [RequireComponent(typeof(CardInteractable))]
    public class CardDisplay : MonoBehaviour
    {
        [Header("Card Canvas References")]
        public Image Artwork;

        public Image CardTemplate;

        public Image SchoolSymbol;

        public Image TypeIcon;

        public TextMeshProUGUI Name;

        public TextMeshProUGUI Description;

        public TextMeshProUGUI PipCost;

        public TextMeshProUGUI Accuracy;


        CardData Card;

        // Start is called before the first frame update
        void Start()
        {
            Card = GetComponent<CardInteractable>().SpellCardData;

            Artwork.sprite = Card.Artwork;

            CardTemplate.sprite = Card.CardTemplate;

            SchoolSymbol.sprite = Card.SchoolSymbol;

            TypeIcon.sprite = Card.TypeIcon;

            Name.text = Card.Name;

            Description.text = Card.Description;

            PipCost.text = Card.PipCost.ToString();

            Accuracy.text = Card.Accuracy.ToString() + "%";
        }
    }
}


