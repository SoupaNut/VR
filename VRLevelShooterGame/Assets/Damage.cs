using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [Tooltip("Amount of damage dealt when collider trigger is entered.")]
    public float damage = 10f;

    // Start is called before the first frame update
    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Destroy object on impact
        Destroy(gameObject);
    }
}