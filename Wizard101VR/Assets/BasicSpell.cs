using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpell : MonoBehaviour
{
    public virtual void Initialize(Transform wandTip)
    {
        transform.position = wandTip.position;
        transform.rotation = wandTip.rotation;
    }
}
