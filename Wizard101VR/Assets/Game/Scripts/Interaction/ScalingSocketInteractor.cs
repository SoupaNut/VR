using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Unity.Game.Interaction
{
    public class ScalingSocketInteractor : XRSocketInteractor
    {
        public float ScaleFactor = 1f;

        float m_InverseScaleFactor;
        GameObject m_ScalingInteractable;
        Collider m_ScalingCollider;
        Mesh m_OriginalMesh;
        Mesh m_ResizedMesh;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            m_InverseScaleFactor = 1f / ScaleFactor;
        }

        protected override void OnDestroy()
        {
            if(m_ResizedMesh != null)
                Destroy(m_ResizedMesh);
                
            base.OnDestroy();
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            // Scale object
            var scalingTransform = args.interactableObject.transform.GetComponentInChildren<ScalingSocketTransform>();

            if (scalingTransform != null)
            {
                m_ScalingInteractable = scalingTransform.gameObject;
                m_ScalingCollider = args.interactableObject.transform.GetComponentInChildren<Collider>();

                ResizeCollider();
                m_ScalingInteractable.transform.localScale *= ScaleFactor;
            }
        }
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (m_ScalingInteractable != null)
            {
                RestoreOriginalCollider();
                m_ScalingInteractable.transform.localScale *= m_InverseScaleFactor;
            }

            m_ScalingInteractable = null;
        }

        void ResizeCollider()
        {
            // make sure we have a collider to scale
            if (m_ScalingCollider == null)
                return;

            if (m_ScalingCollider is BoxCollider)
            {
                ResizeBoxCollider();
            }
            else if (m_ScalingCollider is SphereCollider)
            {
                ResizeSphereCollider();
            }
            else if (m_ScalingCollider is CapsuleCollider)
            {
                ResizeCapsuleCollider();
            }
            else if (m_ScalingCollider is MeshCollider)
            {
                ResizeMeshCollider();
            }
        }

        void ResizeBoxCollider()
        {
            BoxCollider boxCollider = (BoxCollider)m_ScalingCollider;
            boxCollider.size *= ScaleFactor;
            boxCollider.center *= ScaleFactor;
        }

        void ResizeSphereCollider()
        {
            SphereCollider sphereCollider = (SphereCollider)m_ScalingCollider;
            sphereCollider.radius *= ScaleFactor;
            sphereCollider.center *= ScaleFactor;
        }

        void ResizeCapsuleCollider()
        {
            CapsuleCollider capsuleCollider = (CapsuleCollider)m_ScalingCollider;
            capsuleCollider.radius *= ScaleFactor;
            capsuleCollider.height *= ScaleFactor;
            capsuleCollider.center *= ScaleFactor;
        }

        void ResizeMeshCollider()
        {
            MeshCollider meshCollider = (MeshCollider)m_ScalingCollider;

            m_OriginalMesh = meshCollider.sharedMesh;
            m_ResizedMesh = Instantiate(m_OriginalMesh);

            Vector3[] vertices = m_ResizedMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] *= ScaleFactor;
            }
            m_ResizedMesh.vertices = vertices;

            meshCollider.sharedMesh = m_ResizedMesh;
        }

        void RestoreOriginalCollider()
        {
            // make sure we have a collider to scale
            if (m_ScalingCollider == null)
                return;

            if (m_ScalingCollider is BoxCollider)
            {
                RestoreOriginalBoxCollider();
            }
            else if (m_ScalingCollider is SphereCollider)
            {
                RestoreOriginalSphereCollider();
            }
            else if (m_ScalingCollider is CapsuleCollider)
            {
                RestoreOriginalCapsuleCollider();
            }
            else if (m_ScalingCollider is MeshCollider)
            {
                RestoreOriginalMeshCollider();
            }
        }

        void RestoreOriginalBoxCollider()
        {
            BoxCollider boxCollider = (BoxCollider)m_ScalingCollider;
            boxCollider.size *= m_InverseScaleFactor;
            boxCollider.center *= m_InverseScaleFactor;
        }

        void RestoreOriginalSphereCollider()
        {
            SphereCollider sphereCollider = (SphereCollider)m_ScalingCollider;
            sphereCollider.radius *= m_InverseScaleFactor;
            sphereCollider.center *= m_InverseScaleFactor;
        }

        void RestoreOriginalCapsuleCollider()
        {
            CapsuleCollider capsuleCollider = (CapsuleCollider)m_ScalingCollider;
            capsuleCollider.radius *= m_InverseScaleFactor;
            capsuleCollider.height *= m_InverseScaleFactor;
            capsuleCollider.center *= m_InverseScaleFactor;
        }

        void RestoreOriginalMeshCollider()
        {
            MeshCollider meshCollider = (MeshCollider)m_ScalingCollider;
            meshCollider.sharedMesh = m_OriginalMesh;
            Destroy(m_ResizedMesh);
            m_ResizedMesh = null;
        }
    }
}


