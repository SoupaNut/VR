using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Game.Interaction
{    public class HandPhysics : MonoBehaviour
    {
        public Transform Target;

        Rigidbody rb;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            // position
            rb.velocity = (Target.position - transform.position) / Time.fixedDeltaTime;

            // rotation
            Quaternion rotationDifference = Target.rotation * Quaternion.Inverse(transform.rotation);
            rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

            Vector3 rotationDifferenceInDegree = angleInDegree * rotationAxis;

            rb.angularVelocity = rotationDifferenceInDegree * Mathf.Deg2Rad / Time.fixedDeltaTime;
        }
    }
}


