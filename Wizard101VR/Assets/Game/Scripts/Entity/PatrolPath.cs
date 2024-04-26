using System.Collections.Generic;
using UnityEngine;

namespace Unity.Game.Entity
{
    public class PatrolPath : MonoBehaviour
    {
        [Tooltip("Entities that will be assigned to this path on Start")]
        public List<BaseEntityController> EntitiesToAssign = new List<BaseEntityController>();

        [Tooltip("The Nodes making up the path")]
        public List<Transform> PathNodes = new List<Transform>();

        void Start()
        {
            foreach (var entity in EntitiesToAssign)
            {
                entity.PatrolPath = this;
            }
        }

        public float GetDistanceToNode(Vector3 origin, int destinationNodeIndex)
        {
            if (IsInvalidNodeIndex(destinationNodeIndex))
            {
                return -1f;
            }

            return (PathNodes[destinationNodeIndex].position - origin).magnitude;
        }

        public Vector3 GetPositionOfPathNode(int nodeIndex)
        {
            if (IsInvalidNodeIndex(nodeIndex))
            {
                return Vector3.zero;
            }

            return PathNodes[nodeIndex].position;
        }

        bool IsInvalidNodeIndex(int index)
        {
            return index < 0 || index >= PathNodes.Count || PathNodes[index] == null;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < PathNodes.Count; i++)
            {
                int nextIndex = i + 1;
                if (nextIndex >= PathNodes.Count)
                {
                    nextIndex -= PathNodes.Count;
                }

                Gizmos.DrawLine(PathNodes[i].position, PathNodes[nextIndex].position);
                Gizmos.DrawSphere(PathNodes[i].position, 0.1f);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < PathNodes.Count; i++)
            {
                int nextIndex = i + 1;
                if (nextIndex >= PathNodes.Count)
                {
                    nextIndex -= PathNodes.Count;
                }

                Gizmos.DrawLine(PathNodes[i].position, PathNodes[nextIndex].position);
                Gizmos.DrawSphere(PathNodes[i].position, 0.1f);
            }
        }
    }
}