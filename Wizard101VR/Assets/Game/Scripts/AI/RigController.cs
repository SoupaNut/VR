using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Unity.Game.AI
{
    public class RigController : MonoBehaviour
    {
        [Range(0, 180)]
        public float FieldOfVision = 90f;

        public float TurnDuration = 0.25f;

        public MultiAimConstraintData[] Constraints;

        [System.Serializable]
        public struct MultiAimConstraintData
        {
            public MultiAimConstraint constraint;

            [Range(0f, 1f)]
            public float minWeight;

            [Range(0f, 1f)] 
            public float maxWeight;
        }

        bool m_IsSelected;
        bool m_IsLookingAtTarget;
        Transform m_Target;
        
        // Start is called before the first frame update
        void Awake()
        {
            foreach(var constraint in Constraints)
            {
                constraint.constraint.weight = 0f;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(m_IsSelected)
            {
                if(!IsTargetWithinFieldOfView() && m_IsLookingAtTarget)
                {
                    m_IsLookingAtTarget = false;
                    StartTurnBodyPart();
                }
                else if(IsTargetWithinFieldOfView() && !m_IsLookingAtTarget)
                {
                    m_IsLookingAtTarget = true;
                    StartTurnBodyPart();
                }
            }
        }

        IEnumerator TurnBodyPart(MultiAimConstraint constraint, float startWeight, float endWeight, float lerpDuration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < lerpDuration)
            {
                float currentWeight = Mathf.Lerp(startWeight, endWeight, elapsedTime / lerpDuration);

                constraint.weight = currentWeight;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // ensure final weight is set to endweight
            constraint.weight = endWeight;
        }
        public void TurnNPCTowardsTarget(Transform target)
        {
            m_IsSelected = true;
            m_Target = target;

            if(IsTargetWithinFieldOfView())
            {
                m_IsLookingAtTarget = true;
                StartTurnBodyPart();
            }
        }

        public void TurnNPCAwayFromTarget()
        {
            m_IsSelected = false;
            m_Target = null;
            m_IsLookingAtTarget = false;

            StartTurnBodyPart();
        }

        void StartTurnBodyPart()
        {
            foreach (var constraint in Constraints)
            {
                var startWeight = m_IsLookingAtTarget ? constraint.minWeight : constraint.maxWeight;
                var endWeight = m_IsLookingAtTarget ? constraint.maxWeight : constraint.minWeight;
                StartCoroutine(TurnBodyPart(constraint.constraint, startWeight, endWeight, TurnDuration));
            }
        }

        bool IsTargetWithinFieldOfView()
        {
            if(m_Target == null)
            {
                return false;
            }
            else
            {
                Vector3 direction = m_Target.position - transform.position;
                float angle = Vector3.Angle(transform.forward, direction);

                return angle <= FieldOfVision;
            }
            
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            float visionDistance = 2f;
            Vector3 forward = transform.forward * visionDistance;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-FieldOfVision, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(FieldOfVision, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * transform.forward * visionDistance;
            Vector3 rightRayDirection = rightRayRotation * transform.forward * visionDistance;
            Gizmos.DrawRay(transform.position, leftRayDirection);
            Gizmos.DrawRay(transform.position, rightRayDirection);
            Gizmos.DrawLine(transform.position, transform.position + forward + leftRayDirection);
            Gizmos.DrawLine(transform.position, transform.position + forward + rightRayDirection);
        }
    }
}


