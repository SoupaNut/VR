using System.Collections;
using System.Collections.Generic;
using Unity.Game.Shared;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Unity.Game.NPC
{
    public class AnimationRigController : MonoBehaviour
    {
        [Header("General")]
        [Tooltip("How long it takes the NPC to turn it's head to the desired position")]
        public float TurnDuration = 0.25f;

        [Tooltip("Maximum angle for the NPC's field of vision")]
        [Range(0, 180)]
        public float MaxAngle = 60f;

        [Tooltip("Smoothing time for the weight changes.")]
        public float SmoothTime = 0.1f;

        [Tooltip("")]
        public float LookRange = 4f;

        [Header("Head")]
        [Tooltip("Constraint of the NPC's head.")]
        public MultiAimConstraint HeadAim;

        [Tooltip("The min weight of the head aim constraint")]
        public float MinHeadAimWeight = 0f;

        [Tooltip("The max weight of the head aim constraint")]
        public float MaxHeadAimWeight = 1f;

        [Header("Chest")]
        [Tooltip("Constraint of the NPC's chest.")]
        public MultiAimConstraint ChestAim;

        [Tooltip("The min weight of the chest aim constraint")]
        public float MinChestAimWeight = 0f;

        [Tooltip("The max weight of the chest aim constraint")]
        public float MaxChestAimWeight = 0.3f;

        public List<MultiAimConstraints> Constraints;

        [System.Serializable]
        public struct MultiAimConstraints
        {
            public MultiAimConstraint constraint;
            public float minWeight;
            public float maxWeight;
        }

        public Transform Target { get; private set; }
        public bool IsSelected { get; private set; }

        ActorsManager m_ActorsManager;
        Rig m_Rig;
        float m_CurrentWeightVelocity;
        float m_TargetWeight;

        void Awake()
        {
            m_Rig = GetComponent<Rig>();
            DebugUtility.HandleErrorIfNullGetComponent<Rig, AnimationRigController>(m_Rig, this, gameObject);

            m_ActorsManager = FindObjectOfType<ActorsManager>();
            DebugUtility.HandleErrorIfNullFindObject<ActorsManager, AnimationRigController>(m_ActorsManager, this);
        }

        void Update()
        {
            if(IsSelected && Target)
            {
                //Vector3 direction = Target.position - transform.position;

                //float angle = Vector3.Angle(transform.forward, direction);

                //var sources = HeadAim.data.sourceObjects;

                //if (angle <= MaxAngle)
                //{
                //    sources[0].transform.LookAt(Target);
                //    float targetWeight = Mathf.Lerp(MinHeadAimWeight, MaxHeadAimWeight, angle/MaxAngle);
                //    float dampedWeight = Mathf.SmoothDamp(sources.GetWeight(0), targetWeight, ref m_CurrentWeightVelocity, SmoothTime);
                //    sources.SetWeight(0, dampedWeight);
                //}
                //else
                //{
                //    float dampedWeight = Mathf.SmoothDamp(sources.GetWeight(0), MinHeadAimWeight, ref m_CurrentWeightVelocity, SmoothTime);
                //    sources.SetWeight(0, dampedWeight);
                //}

                //HeadAim.data.sourceObjects = sources;
            }
        }



        IEnumerator TurnBodyPart(MultiAimConstraint bodyPart, float startWeight, float endWeight, float lerpDuration)
        {
            var sources = bodyPart.data.sourceObjects;
            float elapsedTime = 0f;

            while (elapsedTime < lerpDuration)
            {
                float currentWeight = Mathf.Lerp(startWeight, endWeight, elapsedTime / lerpDuration);

                sources.SetWeight(0, currentWeight);

                bodyPart.data.sourceObjects = sources;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // ensure final weight is set to 1
            sources.SetWeight(0, endWeight);
            bodyPart.data.sourceObjects = sources;
        }

        public void TurnNPCTowardsTarget(Transform target)
        {
            Target = target;
            IsSelected = true;

            // Turn Head
            //if(HeadAim)
            //    StartCoroutine(TurnBodyPart(HeadAim, MinHeadAimWeight, MaxHeadAimWeight, TurnDuration));

            // Turn Chest
            //if(ChestAim)
            //    StartCoroutine(TurnBodyPart(ChestAim, MinChestAimWeight, MaxChestAimWeight, TurnDuration));
        }

        public void TurnNPCAwayFromTarget()
        {
            Target = null;
            IsSelected = false;

            // Turn Head
            //if(HeadAim)
            //    StartCoroutine(TurnBodyPart(HeadAim, MaxHeadAimWeight, MinHeadAimWeight, TurnDuration));


            // Turn Chest
            //if (ChestAim)
            //    StartCoroutine(TurnBodyPart(ChestAim, MaxChestAimWeight, MinChestAimWeight, TurnDuration));
        }
    }
}


