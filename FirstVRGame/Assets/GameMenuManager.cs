using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2f;
    public GameObject menu;
    public InputActionProperty menuButton;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SpawnMenu();
    }

    private void SpawnMenu()
    {
        if (menuButton.action.WasPressedThisFrame())
        {
            menu.SetActive(!menu.activeSelf);
        }

        if (menu.activeSelf)
        {
            menu.transform.position = head.position + head.forward * spawnDistance;
            menu.transform.LookAt(head);
            menu.transform.forward *= -1;
        }
    }
}
