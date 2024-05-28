using UnityEngine;

namespace Unity.Game.Gameplay.Spells
{
    public class ShieldSpell : MovingSpell
    {

        public override void Cast(SpellcastManager manager)
        {
            base.Cast(manager);
            transform.eulerAngles = new Vector3(0, manager.WandTip.eulerAngles.y, 0);
        }


        public override void HandleHitDetection()
        {
            
        }
    }
}

