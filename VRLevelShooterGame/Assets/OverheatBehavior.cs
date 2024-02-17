using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheatBehavior : MonoBehaviour
{
    [Header("Visual")]
    public ParticleSystem steamVfx;

    // Start is called before the first frame update
    void Start()
    {
        steamVfx.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
