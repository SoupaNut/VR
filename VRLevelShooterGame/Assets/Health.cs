using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Tooltip("Maximum amount of health")] 
    public float MaxHealth = 100f;

    public float CurrentHealth { get; set; }
    public bool Invincible { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    private void TakeDamage(float damage)
    {
        if(Invincible)
        {
            return;
        }
    }
}
