using UnityEngine;

namespace Unity.Game.Gameplay
{
    public class BasicSpell : MonoBehaviour
    {
        [Tooltip("Max amount of time the object stays active in the scene.")]
        public float MaxLifeTime = 5f;

        public GameObject Owner { get; private set; }

        public virtual void Cast(SpellcastManager manager)
        {
            transform.position = manager.WandTip.position;
            transform.forward = manager.WandTip.forward;

            Owner = manager.Owner;

            Destroy(gameObject, MaxLifeTime);
        }
    }
}