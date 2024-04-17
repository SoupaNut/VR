using System.Collections;
using System.Collections.Generic;
using Unity.Game.Shared;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Unity.Game.NPC
{
    public class NpcController : MonoBehaviour
    {
        [Header("Animation Rigging")]
        public MultiAimConstraint HeadAim;

        public float HeadTurnDuration = 0.25f;

        [Header("VFX")]
        [Tooltip("VFX marker to display when NPC is active to talk to. Displays at character's feet.")]
        public GameObject NPCActiveMarker;

        [Header("SFX")]
        [Tooltip("SFX to play when NPC is active")]
        public AudioClip NPCActiveClip;

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
        ActorsManager m_ActorsManager;
        Actor m_Actor;
        Collider[] m_SelfColliders;
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        // Start is called before the first frame update
        void Start()
        {
            DetectionModule = GetComponentInChildren<DetectionModule>();
            DebugUtility.HandleErrorIfNullGetComponent<DetectionModule, NpcController>(DetectionModule, this, gameObject);

            m_ActorsManager = FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, NpcController>(m_ActorsManager, this);

            m_Actor = GetComponent<Actor>();
            DebugUtility.HandleErrorIfNullGetComponent<Actor, NpcController>(m_Actor, this, gameObject);

            m_SelfColliders = GetComponentsInChildren<Collider>();
        }

        // Update is called once per frame
        void Update()
        {
            DetectionModule.HandleTargetDetection(m_Actor, m_SelfColliders);
        }

        IEnumerator TurnHead(float startWeight, float endWeight, float lerpDuration)
        {
            var sources = HeadAim.data.sourceObjects;
            float elapsedTime = 0f;

            while(elapsedTime < lerpDuration)
            {
                float currentWeight = Mathf.Lerp(startWeight, endWeight, elapsedTime / lerpDuration);

                sources.SetWeight(0, currentWeight);

                HeadAim.data.sourceObjects = sources;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // ensure final weight is set to 1
            sources.SetWeight(0, endWeight);
            HeadAim.data.sourceObjects = sources;
        }

        public void SetNPCActiveMarker()
        {
            if(NPCActiveMarker)
            {
                NPCActiveMarker.SetActive(true);

            }

            if(NPCActiveClip)
            {
                AudioUtility.CreateSfx(NPCActiveClip, transform.position, AudioUtility.AudioGroups.UserInterface, 1f, 3f);
            }

            StartCoroutine(TurnHead(0f, 1f, HeadTurnDuration));
        }

        public void ClearNPCActiveMarker()
        {
            if(NPCActiveMarker)
            {
                NPCActiveMarker.SetActive(false);
            }

            StartCoroutine(TurnHead(1f, 0f, HeadTurnDuration));
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

