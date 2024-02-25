using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Game.AI
{
    [RequireComponent(typeof(EnemyController))]
    public class EnemyMobile : MonoBehaviour
    {
        public enum AIState
        {
            Patrol,
            Follow,
            Attack
        }

        public AIState AiState { get; private set; }

        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;

        EnemyController m_EnemyController;

        // Start is called before the first frame update
        void Start()
        {
            m_EnemyController = GetComponent<EnemyController>();
            if(m_EnemyController != null )
            {
                m_EnemyController.OnDetectedTarget += OnDetectedTarget;
                m_EnemyController.OnLostTarget += OnLostTarget;
                m_EnemyController.OnDamaged += OnDamaged;
                m_EnemyController.SetDestinationToClosestPathNode();
            }

            // Start Patrolling
            AiState = AIState.Patrol;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateCurrentAiState();
        }

        private void UpdateCurrentAiState()
        {
            // Handle States
            switch(AiState)
            {
                case AIState.Patrol:
                    m_EnemyController.UpdatePathDestination();
                    m_EnemyController.SetPathDestination(m_EnemyController.GetPositionOfDestination());
                    break;
            }
        }

        private void OnDetectedTarget()
        {

        }

        private void OnLostTarget()
        {

        }

        private void OnDamaged()
        {

        }
    }
}


