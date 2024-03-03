using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Unity.Game.Shared;
using Unity.Game.Utilities;

namespace Unity.Game.AI
{
    public class DetectionModule : MonoBehaviour
    {
        [Tooltip("The point representing the source of target-detection raycasts for the enemy AI")]
        public Transform DetectionSourcePoint;

        [Tooltip("The max distance at which the enemy can see targets")]
        public float DetectionRange = 20f;

        [Tooltip("The max distance at which the enemy can attack its target")]
        public float AttackRange = 10f;

        [Tooltip("Time before an enemy abandons a known target that it can't see anymore")]
        public float KnownTargetTimeout = 4f;

        public UnityAction onDetectTarget;
        public UnityAction onLostTarget;

        public GameObject KnownDetectedTarget { get; private set; }
        public bool IsTargetInAttackRange { get; private set; }
        public bool IsSeeingTarget { get; private set; }
        public bool HadKnownTarget { get; private set; }

        protected float TimeLastSeenTarget = Mathf.NegativeInfinity;

        private ActorsManager m_ActorsManager;

        protected virtual void Start()
        {
            m_ActorsManager = FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, DetectionModule>(m_ActorsManager, this);
        }

        public virtual void HandleTargetDetection(Actor actor, Collider[] selfColliders)
        {
            // Handle known target detection timeout
            if(KnownDetectedTarget && !IsSeeingTarget && (Time.time - TimeLastSeenTarget) > KnownTargetTimeout)
            {
                KnownDetectedTarget = null;
            }

            // Find the closest visible hostile actor
            float sqrDetectionRange = DetectionRange * DetectionRange;
            IsSeeingTarget = false;
            float closestSqrDistance = Mathf.Infinity;

            foreach(Actor otherActor in m_ActorsManager.Actors)
            {
                // if other actor does not have the same affiliation as self actor
                if(otherActor.Affiliation != actor.Affiliation)
                {
                    float sqrDistance = (otherActor.transform.position - DetectionSourcePoint.position).sqrMagnitude; // sqrMagnitude is a micro-optimization. Supposedly better than Vector3.Distance                                                              // if Target is in detection range
                    if (sqrDistance < sqrDetectionRange && sqrDistance < closestSqrDistance)
                    {
                        // Check for obstructions
                        RaycastHit[] hits = Physics.RaycastAll(DetectionSourcePoint.position, (otherActor.AimPoint.position - DetectionSourcePoint.position).normalized, DetectionRange, -1, QueryTriggerInteraction.Ignore);
                        RaycastHit closestValidHit = new RaycastHit();
                        closestValidHit.distance = Mathf.Infinity;
                        bool foundValidHit = false;

                        foreach (var hit in hits)
                        {
                            if (!selfColliders.Contains(hit.collider) && !hit.collider.GetComponent<IgnoreHitDetection>() && hit.distance < closestValidHit.distance)
                            {
                                closestValidHit = hit;
                                foundValidHit = true;
                            }
                        }

                        if (foundValidHit)
                        {
#if UNITY_EDITOR
                            Debug.DrawRay(DetectionSourcePoint.position, (closestValidHit.transform.position - DetectionSourcePoint.position).normalized * closestValidHit.distance, Color.green);
#endif
                            Actor hitActor = closestValidHit.collider.GetComponentInParent<Actor>();
                            if(hitActor == otherActor)
                            {
                                IsSeeingTarget = true;
                                closestSqrDistance = sqrDistance;

                                TimeLastSeenTarget = Time.time;
                                KnownDetectedTarget = otherActor.AimPoint.gameObject;
                            }
                        }
                    }
                }
            }

            float sqrAttackRange = AttackRange * AttackRange;

            IsTargetInAttackRange = KnownDetectedTarget != null && (KnownDetectedTarget.transform.position - DetectionSourcePoint.position).sqrMagnitude <= sqrAttackRange;


            // Detection Events
            if (HadKnownTarget && KnownDetectedTarget == null)
            {
                OnLostTarget();
            }

            if(!HadKnownTarget && KnownDetectedTarget != null)
            {
                OnDetectTarget();
            }

            HadKnownTarget = KnownDetectedTarget != null;
        }

        public virtual void OnDetectTarget() => onDetectTarget?.Invoke();
        
        public virtual void OnLostTarget() => onLostTarget?.Invoke();

        public virtual void OnDamaged(GameObject damageSource)
        {
            TimeLastSeenTarget = Time.time;

            KnownDetectedTarget = damageSource;
        }
    }
}


