using UnityEngine;

namespace Unity.Game.Gameplay
{
    [System.Serializable]
    public class CardData
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

        public BasicSpell SpellPrefab;

        public Material LineMaterial;

        public string SpellName;

        public bool UseVoice;

        public SpellSchools School;

        //public bool UseMovement;

        [Range(0, 14)]
        public int PipCost;

        [Range(0, 100)]
        public int Accuracy = 100;

        public float Damage;
    }
}


