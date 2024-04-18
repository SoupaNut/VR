using UnityEngine;

namespace Unity.Game.Shared
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Player;
        public ActorsManager ActorsManager{get; set;}
        void Awake()
        {
            ActorsManager = GetComponent<ActorsManager>();
            DebugUtility.HandleErrorIfNullGetComponent<ActorsManager, GameManager>(ActorsManager, this, gameObject);

            ActorsManager.SetPlayer(Player);
        }
    }
}


