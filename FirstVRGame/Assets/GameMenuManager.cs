using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2f;
    public GameObject menu;
    public InputActionProperty showButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(showButton.action.WasPressedThisFrame())
        {
            menu.SetActive(!menu.activeSelf);
        }

        if(menu.activeSelf)
        {
            menu.transform.position = head.position + head.forward * spawnDistance;
            menu.transform.LookAt(head);
            menu.transform.forward *= -1;
        }
    }
}
