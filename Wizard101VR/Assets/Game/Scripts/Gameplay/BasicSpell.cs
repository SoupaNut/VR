using UnityEngine;

namespace Unity.Game.Gameplay
{
    public class BasicSpell : MonoBehaviour
    {
        public virtual void Initialize(Transform wandTip)
        {
            transform.position = wandTip.position;
            transform.rotation = wandTip.rotation;
        }
    }
}