using TMPro;
using Unity.Game.Gameplay;
using Unity.Game.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Game.UI
{
    public class CardDisplay : MonoBehaviour
    {
        [Header("Card Canvas References")]
        public GameObject Outline;

        public Image Artwork;

        public Image CardTemplate;

        public Image SchoolSymbol;

        public Image TypeIcon;

        public TextMeshProUGUI Name;

        public TextMeshProUGUI Description;

        public TextMeshProUGUI PipCost;

        public TextMeshProUGUI Accuracy;

        public void DisplayCardOutline(bool active)
        {
            if(Outline)
            {
                Outline.SetActive(active);
            }
        }

        public void Load(CardData data)
        {
            if(data != null)
            {
                Artwork.sprite = data.Artwork;

                CardTemplate.sprite = data.CardTemplate;

                SchoolSymbol.sprite = data.SchoolSymbol;

                TypeIcon.sprite = data.TypeIcon;

                Name.text = data.Name;

                Description.text = data.Description;

                PipCost.text = data.PipCost.ToString();

                Accuracy.text = data.Accuracy.ToString() + "%";
            }
        }
    }
}


