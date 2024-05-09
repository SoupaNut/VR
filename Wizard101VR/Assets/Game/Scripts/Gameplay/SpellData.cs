using UnityEngine;

namespace Unity.Game.Gameplay
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
    public class SpellData : ScriptableObject
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
        [Tooltip("Prefab of the spell animation to instantiate.")]
        public BasicSpell SpellPrefab;

        [Tooltip("Set to true to make the spell use the voice recognizer.")]
        public bool UseVoice = false;

        [Tooltip("The name to compare in the voice recognizer")]
        public string VoiceName;

        [Tooltip("Set to true to make the spell use the movement recognizer.")]
        public bool UseMovement = false;

        [Tooltip("The movement to compare to in the movement recognizer.")]
        public string MovementName;

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

        [Tooltip("Name string that is displayed on the top of the card")]
        public string CardName;

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
    }
}
