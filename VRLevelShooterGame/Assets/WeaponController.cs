using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{

    [Header("References")]
    public InteractorManager leftInteractorManager;
    public InteractorManager rightInteractorManager;
    

    [Header("Shoot Parameters")]
    public Transform weaponMuzzle;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float projectileDespawnTime = 3f;
    public float fireRate = 1f;


    [SerializeField]
    private WeaponMode weaponMode;
    private enum WeaponMode
    {
        Auto,
        SemiAuto
    }

    
    private AudioSource weaponSound;
    private bool autoWeaponModeEnabled;
    private float nextFireTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Weapon Mode - Semi Auto
        if (weaponMode == WeaponMode.SemiAuto)
        {
            GetComponent<XRGrabInteractable>().activated.AddListener(ShootProjectile);
            autoWeaponModeEnabled = false;
        }
        // Weapon Mode - Auto
        else
        {
            autoWeaponModeEnabled = true;
        }
        
        // get weapon sound
        weaponSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // if weapon auto and weapon is selected by an interactor
        if (autoWeaponModeEnabled && (leftInteractorManager.objectSelected == InteractorManager.ObjectSelected.Weapon || rightInteractorManager.objectSelected == InteractorManager.ObjectSelected.Weapon))
        {
            // check if player is pressing the fire button and enough time has passed
            if((leftInteractorManager.isActivated || rightInteractorManager.isActivated) && Time.time >= nextFireTime)
            {
                Fire();

                // Set next allowed fire time based on fire rate
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    private void Fire()
    {
        // spawn bullet
        GameObject spawnedProjectile = Instantiate(projectilePrefab, weaponMuzzle.position, weaponMuzzle.rotation);
        spawnedProjectile.GetComponent<Rigidbody>().velocity = weaponMuzzle.forward * projectileSpeed;

        // play weapon bullet sound
        weaponSound.Play();

        // destroy spawned bullet
        Destroy(spawnedProjectile, projectileDespawnTime);
    }

    private void ShootProjectile(ActivateEventArgs args)
    {
        Fire();
    }
}
