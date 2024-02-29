using UnityEngine;
using Unity.Game.Utilities;

namespace Unity.Game.Shared
{
    public class Actor : MonoBehaviour
    {
        [Tooltip("Represents the affiliation (or team) of the actor. Actors of the same affiliation are friendly to each other")]
        public int Affiliation;

        [Tooltip("Represents point where other actors will aim when they attack this actor")]
        public Transform AimPoint;

        private ActorsManager m_ActorsManager;

        void Start()
        {
            m_ActorsManager = GameObject.FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, Actor>(m_ActorsManager, this);

            // Register as an actor
            if (!m_ActorsManager.Actors.Contains(this))
            {
                m_ActorsManager.Actors.Add(this);
            }
        }

        private void OnDestroy()
        {
            if(m_ActorsManager != null )
            {
                m_ActorsManager.Actors.Remove(this);
            }
        }
    }
}


