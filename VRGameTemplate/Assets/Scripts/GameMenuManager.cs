using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GameMenuManager : MonoBehaviour
{
    [Header("Menu")]
    public Transform head;
    public float spawnDistance = 1f;
    public GameObject menu;
    public InputActionProperty showButton;

    [Header("Menu Items")]
    public ActionBasedContinuousTurnProvider continuousTurn;
    public ActionBasedSnapTurnProvider snapTurn;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        SpawnMenu();
    }

    private void SpawnMenu()
    {
        if (showButton.action.WasPressedThisFrame())
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

    public void SetTypeFromIndex(int index)
    {
        if (index == 0)
        {
            snapTurn.enabled = false;
            continuousTurn.enabled = true;
        }
        else if (index == 1)
        {
            snapTurn.enabled = true;
            continuousTurn.enabled = false;
        }
    }
}
