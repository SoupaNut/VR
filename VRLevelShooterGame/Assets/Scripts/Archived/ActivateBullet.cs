using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateBullet : MonoBehaviour
{
    public GameObject bullet;
    public float bulletTravelSpeed = 20f;
    public float bulletDespawnTime = 5f;
    public Transform bulletSpawnPoint;

    // Start is called before the first frame update
    private void Start()
    {
        // get grabbable object
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(SpawnBullet);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnBullet(ActivateEventArgs args)
    {
        GameObject spawnedBullet = Instantiate(bullet);
        spawnedBullet.transform.position = bulletSpawnPoint.position;
        spawnedBullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletTravelSpeed;
        Destroy(spawnedBullet, bulletDespawnTime);
    }
}
