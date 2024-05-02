using UnityEngine;
using UnityEngine.AI;
using Unity.Game.Shared;

namespace Unity.Game.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BaseEntityController : MonoBehaviour
    {
        [Header("Patrol Parameters")]
        [Tooltip("The distance at which the entity considers that it has reached its current path destination point")]
        public float PathReachingRadius = 2f;

        [Tooltip("Color of the sphere gizmo representing the path reaching range")]
        public Color PathReachingRangeColor = Color.yellow;

        public NavMeshAgent NavMeshAgent { get; set; }
        public PatrolPath PatrolPath { get; set; }

        int m_DestinationPathNodeIndex;

        protected virtual void Start()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            DebugUtility.HandleErrorIfNullGetComponent<NavMeshAgent, BaseEntityController>(NavMeshAgent, this, gameObject);
        }

        bool IsPathValid()
        {
            return PatrolPath && PatrolPath.PathNodes.Count > 0;
        }

        public void UpdatePathDestination(bool inverseOrder = false)
        {
            if (IsPathValid())
            {
                // Check if reached the path destination
                if ((transform.position - GetPositionOfDestination()).magnitude <= PathReachingRadius)
                {
                    // increment path destination index
                    m_DestinationPathNodeIndex = inverseOrder ? (m_DestinationPathNodeIndex - 1) : (m_DestinationPathNodeIndex + 1);
                    if (m_DestinationPathNodeIndex < 0)
                    {
                        m_DestinationPathNodeIndex = PatrolPath.PathNodes.Count - 1;
                    }

                    if (m_DestinationPathNodeIndex >= PatrolPath.PathNodes.Count)
                    {

                        m_DestinationPathNodeIndex = 0;
                    }
                }
            }
        }

        public void SetDestinationToClosestPathNode()
        {
            if (IsPathValid())
            {
                int closestPathNodeIndex = 0;
                for (int i = 0; i < PatrolPath.PathNodes.Count; i++)
                {
                    float distanceToNode = PatrolPath.GetDistanceToNode(transform.position, i);
                    if (distanceToNode < PatrolPath.GetDistanceToNode(transform.position, closestPathNodeIndex))
                    {
                        closestPathNodeIndex = i;
                    }
                }

                m_DestinationPathNodeIndex = closestPathNodeIndex;
            }
            else
            {
                m_DestinationPathNodeIndex = 0;
            }
        }

        public Vector3 GetPositionOfDestination()
        {
            if (IsPathValid())
            {
                return PatrolPath.GetPositionOfPathNode(m_DestinationPathNodeIndex);
            }
            else
            {
                return transform.position;
            }
        }

        public void SetPathDestination(Vector3 destination)
        {
            if (NavMeshAgent)
            {
                NavMeshAgent.SetDestination(destination);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            // Path reaching range
            Gizmos.color = PathReachingRangeColor;
            Gizmos.DrawWireSphere(transform.position, PathReachingRadius);
        }
    }
}