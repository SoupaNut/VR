using UnityEngine;

namespace Unity.Game.Shared
{
    public class MathUtility : MonoBehaviour
    {
        public static Vector3 CalculateNormal(Vector3 hitPoint, Collider collider)
        {
            if (collider is MeshCollider meshCollider)
            {
                return CalculateMeshNormal(hitPoint, meshCollider);
            }
            else if (collider is BoxCollider boxCollider)
            {
                return CalculateBoxNormal(hitPoint, boxCollider);
            }
            else if (collider is SphereCollider sphereCollider)
            {
                return CalculateSphereNormal(hitPoint, sphereCollider);
            }
            else if(collider is CapsuleCollider capsuleCollider)
            {
                return CalculateCapsuleNormal(hitPoint, capsuleCollider);
            }
            else
            {
                Debug.LogError("Collider type not supported for normal calculation.");
                return Vector3.zero;
            }
        }

        public static Vector3 CalculateMeshNormal(Vector3 hitPoint, MeshCollider collider)
        {
            Mesh mesh = collider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3[] normals = mesh.normals;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v1 = vertices[triangles[i]] - hitPoint;
                Vector3 v2 = vertices[triangles[i + 1]] - hitPoint;
                Vector3 v3 = vertices[triangles[i + 2]] - hitPoint;

                // Calculate the normal of the triangle
                Vector3 crossProduct = Vector3.Cross(v2 - v1, v3 - v1).normalized;

                // Check if the hit point is on this triangle (using dot product)
                if (Vector3.Dot(crossProduct, normals[triangles[i]]) > 0)
                {
                    // This triangle is facing the correct direction
                    // The normal of the hit point is the same as the normal of this triangle
                    Vector3 normal = normals[triangles[i]];

                    return normal;
                }
            }

            Debug.LogWarning("Failed to find a normal for the mesh collider. Returning Vector3.zero");
            return Vector3.zero;
        }

        public static Vector3 CalculateBoxNormal(Vector3 hitPoint, BoxCollider collider)
        {
            Vector3 center = collider.transform.TransformPoint(collider.center);
            Vector3 localHitPoint = collider.transform.InverseTransformPoint(hitPoint);
            Vector3 normal = Vector3.zero;

            float minDistance = Mathf.Infinity;
            for (int i = 0; i < 6; i++)
            {
                Vector3 faceNormal = Vector3.zero;
                switch (i)
                {
                    case 0: faceNormal = collider.transform.up; break;          // Top
                    case 1: faceNormal = -collider.transform.up; break;         // Bottom
                    case 2: faceNormal = collider.transform.right; break;       // Right
                    case 3: faceNormal = -collider.transform.right; break;      // Left
                    case 4: faceNormal = collider.transform.forward; break;     // Front
                    case 5: faceNormal = -collider.transform.forward; break;    // Back
                }

                float distance = Vector3.Distance(localHitPoint, Vector3.Scale(localHitPoint, faceNormal));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    normal = faceNormal;
                }
            }

            return normal;
        }

        public static Vector3 CalculateSphereNormal(Vector3 hitPoint, SphereCollider collider)
        {
            Vector3 center = collider.transform.TransformPoint(collider.center);
            Vector3 normal = (hitPoint - center).normalized;
            return normal;
        }

        public static Vector3 CalculateCapsuleNormal(Vector3 hitPoint, CapsuleCollider collider)
        {
            Vector3 center = collider.transform.TransformPoint(collider.center);
            Vector3 localHitPoint = collider.transform.InverseTransformPoint(hitPoint);
            float height = collider.height - collider.radius * 2f;
            Vector3 normal;

            // Determine which part of the capsule the hit point is on
            if (localHitPoint.y < -height / 2f) // Lower hemisphere
            {
                normal = (hitPoint - center).normalized;
            }
            else if (localHitPoint.y > height / 2f) // Upper hemisphere
            {
                normal = (hitPoint - (center + Vector3.up * height)).normalized;
            }
            else // Cylindrical body
            {
                Vector3 direction = (hitPoint - center).normalized;
                normal = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
            }

            return normal;
        }

    }
}

