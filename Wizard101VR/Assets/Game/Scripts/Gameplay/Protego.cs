using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Game.Gameplay
{
    public class Protego : BasicSpell
    {
        public override void Initialize(Transform wandTip)
        {
            transform.position = wandTip.position;
            transform.eulerAngles = new Vector3(0, wandTip.eulerAngles.y, 0);
        }
    }
}


