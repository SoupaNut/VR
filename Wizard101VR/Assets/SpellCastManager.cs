using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellCastManager : MonoBehaviour
{
    public InputActionProperty SpellInput;

    public BasicSpell DefaultSpell;

    public Transform WandTip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SpellInput.action.WasPerformedThisFrame())
        {
            StartCasting();
        }
        else if (SpellInput.action.WasReleasedThisFrame())
        {
            StopCasting();
        }
    }

    public void StartCasting()
    {

    }

    public void StopCasting()
    {
        BasicSpell spawnedSpell = Instantiate(DefaultSpell);
        spawnedSpell.Initialize(WandTip);
    }

}
