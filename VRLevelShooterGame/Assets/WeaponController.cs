using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Unity.Game.Utilities;

public class WeaponController : MonoBehaviour
{
    [Header("Shoot Parameters")]
    public Transform WeaponMuzzle;
    public GameObject ProjectilePrefab;
    public float ProjectileSpeed = 20f;
    public float FireRate = 1f;
    public AudioClip WeaponSound;
    public float Damage = 10f;


    [SerializeField]
    private WeaponMode m_WeaponMode;
    private enum WeaponMode
    {
        Auto,
        SemiAuto
    }
    
    private float m_NextFireTime = 0f;
    private InteractorManager m_InteractorManager;

    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable weapon = GetComponent<XRGrabInteractable>();

        // Handle when an interactor selects weapon
        weapon.selectEntered.AddListener(GetInteractorManager);
        weapon.selectExited.AddListener(ClearInteractorManager);

        if (m_WeaponMode == WeaponMode.SemiAuto)
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
            if(m_InteractorManager.isActivated && m_WeaponMode == WeaponMode.Auto)
            {
                // Fire button is held and enough time has passed
                if(Time.time >= m_NextFireTime)
                {
                    Fire();

                    // Set next allowed fire time based on fire rate
                    m_NextFireTime = Time.time + 1f / FireRate;
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
        GameObject spawnedProjectile = Instantiate(ProjectilePrefab, WeaponMuzzle.position, WeaponMuzzle.rotation);
        //spawnedProjectile.GetComponent<Rigidbody>().velocity = WeaponMuzzle.forward * ProjectileSpeed;
        spawnedProjectile.Shoot(this);

        // play weapon bullet sound
        AudioUtility.CreateSfx(WeaponSound, WeaponMuzzle.position, 0.3f);
    }

    private void ShootProjectile(ActivateEventArgs args)
    {
        Fire();
    }
}
