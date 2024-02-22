using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Unity.Game.Utilities;

public class WeaponController : MonoBehaviour
{
    [Header("Shoot Parameters")]
    public Transform weaponMuzzle;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public float projectileDespawnTime = 3f;
    public float fireRate = 1f;
    public AudioClip weaponSound;


    [SerializeField]
    private WeaponMode m_weaponMode;
    private enum WeaponMode
    {
        Auto,
        SemiAuto
    }
    
    private float m_nextFireTime = 0f;
    private InteractorManager m_InteractorManager;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable weapon = GetComponent<XRGrabInteractable>();

        // Handle when an interactor selects weapon
        weapon.selectEntered.AddListener(GetInteractorManager);
        weapon.selectExited.AddListener(ClearInteractorManager);

        if (m_weaponMode == WeaponMode.SemiAuto)
        {
            weapon.activated.AddListener(ShootProjectile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_InteractorManager != null)
        {
            // Weapon is auto
            if(m_InteractorManager.isActivated && m_weaponMode == WeaponMode.Auto)
            {
                // Fire button is held and enough time has passed
                if(Time.time >= m_nextFireTime)
                {
                    Fire();

                    // Set next allowed fire time based on fire rate
                    m_nextFireTime = Time.time + 1f / fireRate;
                }
            }
        }
    }

    private void GetInteractorManager(SelectEnterEventArgs args)
    {
        var interactorManager = args.interactorObject.transform.parent.gameObject.GetComponent<InteractorManager>();

        if(interactorManager != null)
        {
            m_InteractorManager = interactorManager;
        }
    }

    private void ClearInteractorManager(SelectExitEventArgs args)
    {
        // clear reference
        m_InteractorManager = new InteractorManager();
    }

    private void Fire()
    {
        // spawn bullet
        GameObject spawnedProjectile = Instantiate(projectilePrefab, weaponMuzzle.position, weaponMuzzle.rotation);
        spawnedProjectile.GetComponent<Rigidbody>().velocity = weaponMuzzle.forward * projectileSpeed;

        // play weapon bullet sound
        AudioUtility.CreateSfx(weaponSound, weaponMuzzle.position, 0.3f);


        // destroy spawned bullet
        Destroy(spawnedProjectile, projectileDespawnTime);
    }

    private void ShootProjectile(ActivateEventArgs args)
    {
        Fire();
    }
}
