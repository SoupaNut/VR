// NOTE: THIS SCRIPT IS TEMPORARY. REDO THIS WHEN THERE IS TIME.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Shared;
using Unity.Game.Utilities;

public class OnStartupInit : MonoBehaviour
{
    ActorsManager m_ActorsManager;

    // Start is called before the first frame update
    void Awake()
    {
        m_ActorsManager = GameObject.FindObjectOfType<ActorsManager>();
        DebugUtility.HandleErrorIfNullFindObject<ActorsManager, Actor>(m_ActorsManager, this);

        m_ActorsManager.SetPlayer(gameObject);
    }
}
