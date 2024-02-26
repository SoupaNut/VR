using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Game.AI
{
    public class DetectionModule : MonoBehaviour
    {
        [Tooltip("The max distance at which the enemy can see targets")]
        public float DetectionRange = 20f;

        [Tooltip("The max distance at which the enemy can attack its target")]
        public float AttackRange = 10f;

        [Tooltip("Time before an enemy abandons a known target that it can't see anymore")]
        public float KnownTargetTimeout = 4f;

        [Tooltip("The game object that the detection module should target")]
        public GameObject Target;

        [Tooltip("LayerMask of the Target")]
        public LayerMask TargetLayer;

        public UnityAction onDetectTarget;
        public UnityAction onLostTarget;

        public GameObject KnownDetectedTarget { get; private set; }
        public bool IsTargetInAttackRange { get; private set; }
        public bool IsSeeingTarget { get; private set; }
        public bool HadKnownTarget { get; private set; }

        protected float TimeLastSeenTarget = Mathf.NegativeInfinity;

        public virtual void HandleTargetDetection(Collider[] selfColliders)
        {
            // Handle known target detection timeout
            if(KnownDetectedTarget && !IsSeeingTarget && (Time.time - TimeLastSeenTarget) < KnownTargetTimeout)
            {
                KnownDetectedTarget = null;
            }

            // Find the target
            IsSeeingTarget = false;
            bool targetInDetectionRange = Physics.CheckSphere(transform.position, DetectionRange, TargetLayer);
            bool targetInAttackRange = Physics.CheckSphere(transform.position, AttackRange, TargetLayer);


            // Detection Events
            if(HadKnownTarget && KnownDetectedTarget == null)
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


