using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Game.Shared
{
    public class DetectionModule : MonoBehaviour
    {
        [Header("Detection Parameters")]
        [Tooltip("The point representing the source of target-detection raycasts for the AI")]
        public Transform DetectionSourcePoint;

        [Tooltip("The max distance at which the AI can detect targets")]
        public float DetectionRange = 20f;

        [Tooltip("The distance at which the AI will do a unique behavior to the target.")]
        public float InnerRange = 10f;

        [Tooltip("Time before an AI abandons a known target that it can't detect anymore")]
        public float KnownTargetTimeout = 4f;

        public UnityAction onDetectTarget;
        public UnityAction onLostTarget;

        public GameObject KnownDetectedTarget { get; private set; }
        public bool IsTargetInInnerRange { get; private set; }
        public bool IsDetectingTarget { get; private set; }
        public bool HadKnownTarget { get; private set; }

        protected ActorsManager ActorsManager;
        protected float TimeLastSeenTarget = Mathf.NegativeInfinity;

        protected virtual void Start()
        {
            ActorsManager = FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, DetectionModule>(ActorsManager, this);
        }

        public virtual void HandleTargetDetection(Actor actor, Collider[] selfColliders)
        {
            // Handle known target timeout
            if (KnownDetectedTarget && !IsDetectingTarget && (Time.time - TimeLastSeenTarget) > KnownTargetTimeout)
            {
                KnownDetectedTarget = null;
            }

            // find the closest actor
            float sqrDetectionRange = DetectionRange * DetectionRange;
            IsDetectingTarget = false;
            float closestSqrDistance = Mathf.Infinity;

            foreach(Actor otherActor in ActorsManager.Actors)
            {
                // If other actor has different affiliation
                if(otherActor.Affiliation != actor.Affiliation)
                {
                    float sqrDistance = (otherActor.transform.position - DetectionSourcePoint.position).sqrMagnitude;
                    if (sqrDistance < sqrDetectionRange && sqrDistance < closestSqrDistance)
                    {
                        // Check for obstructions
                        RaycastHit[] hits = Physics.RaycastAll(DetectionSourcePoint.position, (otherActor.LookPoint.position - DetectionSourcePoint.position).normalized, DetectionRange, -1, QueryTriggerInteraction.Ignore);
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

                            Actor hitActor = closestValidHit.collider.GetComponentInParent<Actor>();
                            if (hitActor == otherActor)
                            {
                                IsDetectingTarget = true;
                                closestSqrDistance = sqrDistance;

                                TimeLastSeenTarget = Time.time;
                                KnownDetectedTarget = otherActor.gameObject;
#if UNITY_EDITOR
                                Debug.DrawRay(DetectionSourcePoint.position, (hitActor.LookPoint.position - DetectionSourcePoint.position).normalized * closestValidHit.distance, Color.green);
#endif
                            }
                        }
                    }
                }
            }

            float sqrInnerRange = InnerRange * InnerRange;

            IsTargetInInnerRange = KnownDetectedTarget != null && (KnownDetectedTarget.transform.position - DetectionSourcePoint.position).sqrMagnitude <= sqrInnerRange;

            // Detection Events
            if (HadKnownTarget && KnownDetectedTarget == null)
            {
                OnLostTarget();
            }

            if (!HadKnownTarget && KnownDetectedTarget != null)
            {
                OnDetectTarget();
            }

            HadKnownTarget = KnownDetectedTarget != null;
        }

        public virtual void OnDetectTarget() => onDetectTarget?.Invoke();

        public virtual void OnLostTarget() => onLostTarget?.Invoke();
    }
}

