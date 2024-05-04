using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Game.Gameplay
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

        public void Load(SpellData data)
        {
            if(data != null)
            {
                Artwork.sprite = data.Artwork;

                CardTemplate.sprite = data.CardTemplate;

                SchoolSymbol.sprite = data.SchoolSymbol;

                TypeIcon.sprite = data.TypeIcon;

                Name.text = data.CardName;

                Description.text = data.Description;

                PipCost.text = data.PipCost.ToString();

                Accuracy.text = data.Accuracy.ToString() + "%";
            }
        }
    }
}


