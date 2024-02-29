using System.Collections.Generic;
using UnityEngine;

namespace Unity.Game.Shared
{
    public class ActorsManager : MonoBehaviour
    {
        public List<Actor> Actors { get; private set; }
        public GameObject Player { get; private set; }

        public void SetPlayer(GameObject player) => Player = player;

        private void Awake()
        {
            Actors = new List<Actor>();
        }
    }
}


