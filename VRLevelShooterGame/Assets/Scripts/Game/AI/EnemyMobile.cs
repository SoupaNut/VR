using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Game.Utilities;

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

        [Tooltip("Animations of the enemy")]
        public Animator Animator;

        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;

        [Tooltip("The random hit damage effects")]
        public ParticleSystem[] RandomHitSparks;

        public ParticleSystem[] OnDetectVfx;
        public AudioClip OnDetectSfx;
        
        EnemyController m_EnemyController;

        const string k_AnimMoveSpeedParameter = "MoveSpeed";
        const string k_AnimAttackParameter = "Attack";
        const string k_AnimAlertedParameter = "Alerted";
        const string k_AnimOnDamagedParameter = "OnDamaged";
        // Start is called before the first frame update
        void Start()
        {
            m_EnemyController = GetComponent<EnemyController>();
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, EnemyMobile>(m_EnemyController, this, gameObject);

            m_EnemyController.onAttack += OnAttack;
            m_EnemyController.onDetectedTarget += OnDetectedTarget;
            m_EnemyController.onLostTarget += OnLostTarget;
            m_EnemyController.onDamaged += OnDamaged;
            m_EnemyController.SetDestinationToClosestPathNode();

            // Start Patrolling
            AiState = AIState.Patrol;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAiStateTransitions();
            UpdateCurrentAiState();

            // update Animator speed parameter
            float speed = m_EnemyController.NavMeshAgent.velocity.magnitude;
            Animator.SetFloat(k_AnimMoveSpeedParameter, speed);
        }

        private void UpdateAiStateTransitions()
        {
            // Handle Transitions
            switch(AiState)
            {
                // attack target is in range and we have line of sight
                case AIState.Follow:
                    if(m_EnemyController.IsTargetInAttackRange && m_EnemyController.IsSeeingTarget)
                    {
                        AiState = AIState.Attack;
                        m_EnemyController.SetPathDestination(transform.position);
                    }
                    break;
                // change state to follow if target not in attack range or lost line of sight
                case AIState.Attack:
                    if(!m_EnemyController.IsSeeingTarget || !m_EnemyController.IsTargetInAttackRange)
                    {
                        AiState = AIState.Follow;
                    }
                    break;
            }
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
                case AIState.Follow:
                    m_EnemyController.SetPathDestination(m_EnemyController.KnownDetectedTarget.transform.position);
                    m_EnemyController.OrientTowards(m_EnemyController.KnownDetectedTarget.transform.position);
                    break;
                case AIState.Attack:
                    // Check if distance from enemy to player is within stop distance
                    float distance = Vector3.Distance(m_EnemyController.KnownDetectedTarget.transform.position, m_EnemyController.DetectionModule.DetectionSourcePoint.position);
                    float stopDistance = AttackStopDistanceRatio * m_EnemyController.DetectionModule.AttackRange;
                    if (distance >= stopDistance)
                    {
                        m_EnemyController.SetPathDestination(m_EnemyController.KnownDetectedTarget.transform.position);
                    }
                    else
                    {
                        m_EnemyController.SetPathDestination(transform.position);
                    }
                    m_EnemyController.OrientTowards(m_EnemyController.KnownDetectedTarget.transform.position);
                    m_EnemyController.TryAttack();
                    break;
            }
        }

        private void OnAttack()
        {
            Animator.SetTrigger(k_AnimAttackParameter);
        }

        private void OnDetectedTarget()
        {
            if(AiState == AIState.Patrol)
            {
                AiState = AIState.Follow;
            }

            for(int i = 0; i < OnDetectVfx.Length; i++)
            {
                OnDetectVfx[i].Play();
            }

            if(OnDetectSfx)
            {
                float spatialBlend = 1f;
                float rolloffDistanceMin = 10f;
                float volume = 1f;
                AudioUtility.CreateSfx(OnDetectSfx, transform.position, AudioUtility.AudioGroups.EnemyDetection, spatialBlend, rolloffDistanceMin, volume);
            }

            Animator.SetBool(k_AnimAlertedParameter, true);
        }

        private void OnLostTarget()
        {
            if(AiState == AIState.Follow || AiState == AIState.Attack)
            {
                AiState = AIState.Patrol;
            }

            for (int i = 0; i < OnDetectVfx.Length; i++)
            {
                OnDetectVfx[i].Stop();
            }

            Animator.SetBool(k_AnimAlertedParameter, false);
        }

        private void OnDamaged()
        {
            if (RandomHitSparks.Length > 0)
            {
                int n = Random.Range(0, RandomHitSparks.Length - 1);
                RandomHitSparks[n].Play();
            }

            Animator.SetTrigger(k_AnimOnDamagedParameter);
        }
    }
}


