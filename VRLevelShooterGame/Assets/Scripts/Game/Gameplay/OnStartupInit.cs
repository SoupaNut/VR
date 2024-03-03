// NOTE: THIS SCRIPT IS TEMPORARY. REDO THIS WHEN THERE IS TIME.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Shared;
using Unity.Game.Utilities;

public class OnStartupInit : MonoBehaviour
{
    ActorsManager m_ActorsManager;
    CharacterController m_CharacterController;
    // Start is called before the first frame update
    void Awake()
    {
        m_ActorsManager = GameObject.FindObjectOfType<ActorsManager>();
        DebugUtility.HandleErrorIfNullFindObject<ActorsManager, Actor>(m_ActorsManager, this);

        m_ActorsManager.SetPlayer(gameObject);

        m_CharacterController = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        m_CharacterController.center = new Vector3(0, 1, 0);
    }

    void Update()
    {
        //m_CharacterController.center = new Vector3(0, 1, 0);
        //Debug.Log(m_CharacterController.center);
    }
}
