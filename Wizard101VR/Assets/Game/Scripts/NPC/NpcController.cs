using System.Collections;
using System.Collections.Generic;
using Unity.Game.Shared;
using UnityEngine;

namespace Unity.Game.NPC
{
    public class NpcController : MonoBehaviour
    {
        [Header("Debug Display")]
        [Tooltip("Color of the sphere gizmo representing the inner range")]
        public Color InnerRangeColor = Color.red;

        [Tooltip("Color of the sphere gizmo representing the detection range")]
        public Color DetectionRangeColor = Color.blue;

        // - - - - - - - - - - P U B L I C - - - - - - - - - - //
        public DetectionModule DetectionModule { get; set; }
        public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
        public bool IsTargetInInnerRange => DetectionModule.IsTargetInInnerRange;
        public bool IsDetectingTarget => DetectionModule.IsDetectingTarget;
        public bool HadKnownTarget => DetectionModule.HadKnownTarget;
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        // - - - - - - - - - - P R I V A T E - - - - - - - - - - //
        Actor m_Actor;
        Collider[] m_SelfColliders;
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        // Start is called before the first frame update
        void Start()
        {
            DetectionModule = GetComponentInChildren<DetectionModule>();
            DebugUtility.HandleErrorIfNullGetComponent<DetectionModule, NpcController>(DetectionModule, this, gameObject);

            m_Actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, NpcController>(m_Actor, this, gameObject);

            m_SelfColliders = GetComponentsInChildren<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            DetectionModule.HandleTargetDetection(m_Actor, m_SelfColliders);

            if(KnownDetectedTarget)
            {
                Vector3 direction = KnownDetectedTarget.GetComponent<Actor>().LookPoint.position - m_Actor.LookPoint.position;
                DetectionModule.DetectionSourcePoint.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        void OnDrawGizmos()
        {
            if(DetectionModule != null)
            {
                // Detection range
                Gizmos.color = DetectionRangeColor;
                Gizmos.DrawWireSphere(transform.position, DetectionModule.DetectionRange);

                // Attack range
                Gizmos.color = InnerRangeColor;
                Gizmos.DrawWireSphere(transform.position, DetectionModule.InnerRange);
            }
        }
    }
}

