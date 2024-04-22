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
        public BasicSpell Animation;

        public Material LineMaterial;

        public Sprite Artwork;

        public Sprite CardTemplate;

        public Sprite SchoolSymbol;

        public Sprite TypeIcon;


        [Header("Card Info")]
        public string Name;

        [TextArea(2, 4)]
        public string Description;

        public SpellSchools School;

        //public bool UseMovement;

        [Range(0, 14)]
        public int PipCost;

        [Range(0, 100)]
        public int Accuracy = 100;

        public float Damage;

        public bool UseVoice;
    }
}
