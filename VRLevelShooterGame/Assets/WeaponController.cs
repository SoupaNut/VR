using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{

    //[Header("References")]
    //public InteractorManager leftInteractors;
    //public InteractorManager rightInteractors;
    

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
    private float nextFireTime = 0f;
    private InteractorManager interactorManager;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable weapon = GetComponent<XRGrabInteractable>();
        if (weaponMode == WeaponMode.SemiAuto)
        {
            weapon.activated.AddListener(ShootProjectile);
        }

        weapon.selectEntered.AddListener(GetInteractorManager);
        weapon.selectExited.AddListener(ClearInteractorManager);

        // get weapon sound
        weaponSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // Scratch Code - TEST LATER
        if(interactorManager != null)
        {
            // Weapon is auto
            if(weaponMode == WeaponMode.Auto)
            {
                // Fire button is held and enough time has passed
                if(interactorManager.isActivated && Time.time >= nextFireTime)
                {
                    Fire();

                    // Set next allowed fire time based on fire rate
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
        }

        // if weapon auto and weapon is selected by an interactor
        //if (weaponMode == WeaponMode.Auto && (leftInteractors.objectSelected == InteractorManager.ObjectSelected.Weapon || rightInteractors.objectSelected == InteractorManager.ObjectSelected.Weapon))
        //{
        //    // check if player is pressing the fire button and enough time has passed
        //    if((leftInteractors.isActivated || rightInteractors.isActivated) && Time.time >= nextFireTime)
        //    {
        //        Fire();

        //        // Set next allowed fire time based on fire rate
        //        nextFireTime = Time.time + 1f / fireRate;
        //    }
        //}
    }

    private void GetInteractorManager(SelectEnterEventArgs args)
    {
        interactorManager = args.interactorObject.gameObject.GetComponent<InteractorManager>();
    }

    private void ClearInteractorManager(SelectExitEventArgs args)
    {
        interactorManager = null;
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
