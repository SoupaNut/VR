using UnityEngine;

namespace Unity.Game.Gameplay
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Card")]
    public class CardData : ScriptableObject
    {
        public enum SpellSchools
        {
            Fire,
            Ice,
            Storm,
            Myth,
            Life,
            Death,
            Balance
        }

        [Header("Art")]
        [Tooltip("Prefab of the spell animation to instantiate.")]
        public BasicSpell Animation;

        [Tooltip("The material that the Movement Recognizer on the wand will be set to")]
        public Material LineMaterial;

        [Tooltip("The spell artwork on the card.")]
        public Sprite Artwork;

        [Tooltip("The card border art.")]
        public Sprite CardTemplate;

        [Tooltip("The school symbol to display on the card.")]
        public Sprite SchoolSymbol;

        [Tooltip("Icon for the spell type of the card.")]
        public Sprite TypeIcon;


        [Header("Card Info")]
        [Tooltip("The spell's name.")]
        public string Name;

        [Tooltip("Description of the card.")]
        [TextArea(2, 4)]
        public string Description;

        [Tooltip("The spell's school. This is used in the SpellcastManager to search for the gesture to compare against.")]
        public SpellSchools School;

        [Tooltip("Number of pips required to cast the spell")]
        [Range(0, 14)]
        public int PipCost;

        [Tooltip("The probability that the spell will be cast.")]
        [Range(0, 100)]
        public int Accuracy = 100;

        [Tooltip("How much damage the spell does.")]
        public float Damage;

        [Tooltip("Set to true to make the spell use the voice recognizer.")]
        public bool UseVoice = false;

        [Tooltip("Set to true to make the spell use the movement recognizer.")]
        public bool UseMovement = true;
    }
}
