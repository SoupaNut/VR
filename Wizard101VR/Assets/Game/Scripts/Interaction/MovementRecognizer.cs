using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PDollarGestureRecognizer;
using System.IO;
using Unity.Game.Shared;

namespace Unity.Game.Interaction
{
    [RequireComponent(typeof(WeaponGrabInteractable))]
    public class MovementRecognizer : MonoBehaviour
    {
        [Header("General")]
        [Tooltip("The position of where to draw the line.")]
        public Transform MovementSource;

        [Header("Line Rendering")]
        [Tooltip("Distance that the new position should at a minimum be to update the line.")]
        public float NewPositionThresholdDistance = 0.05f;

        [Tooltip("Width of the line drawn.")]
        public float LineWidth = 0.03f;

        [Tooltip("Material of the line drawn.")]
        public Material LineMaterial;

        [Tooltip("How long it takes for the line to despawn after player stops drawing")]
        public float LineDespawnTime = 2f;

        [Header("Gesture Recognition")]
        [Tooltip("True = Create a new gesture, False = Compare with recorded gestures")]
        public bool CreationMode = true;

        [Tooltip("Name of the new gesture when in Creation Mode")]
        public string NewGestureName;

        [Tooltip("How accurate the gesture should be.")]
        [Range(0f, 1f)]
        public float RecognitionThreshold = 0.9f;


        // - - - - - - - - - - E V E N T S - - - - - - - - - - //
        [Tooltip("Event for when gesture is recognized with an accuracy equal to or greater than the Recognition Threshold.")]
        public OnRecognizedEvent onRecognized;

        [System.Serializable]
        public class OnRecognizedEvent : UnityEvent<string> { }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        // - - - - - - - - - - P R I V A T E - - - - - - - - - - //
        WeaponGrabInteractable m_WeaponGrabInteractable;
        List<Gesture> m_GestureList = new List<Gesture>();
        List<Vector3> m_PositionsList = new List<Vector3>();
        string m_GestureFileExtension = "_gesture.xml";
        bool m_IsMoving = false;
        LineRenderer m_CurrentLine;

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        void Start()
        {
            m_WeaponGrabInteractable = GetComponent<WeaponGrabInteractable>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponGrabInteractable, WandMovementRecognizer>(m_WeaponGrabInteractable, this, gameObject);

            // Get all recorded gestures
            string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*" + m_GestureFileExtension);
            foreach (var item in gestureFiles)
            {
                m_GestureList.Add(GestureIO.ReadGestureFromFile(item));
            }
        }

        void Update()
        {
            // if wand is being grabbed
            if (m_WeaponGrabInteractable.IsWeaponEnabled)
            {
                bool isPressed = m_WeaponGrabInteractable.InteractorManager.IsActivated;

                // Start the Movement
                if (!m_IsMoving && isPressed)
                {
                    StartMovement();
                }
                // Updating the Movement
                else if (m_IsMoving && isPressed)
                {
                    UpdateMovement();
                }
                // Ending the Movement
                else if (m_IsMoving && !isPressed)
                {
                    EndMovement();
                }
            }
        }

        void UpdateLine()
        {
            // Update Line Position
            m_CurrentLine.positionCount = m_PositionsList.Count;
            m_CurrentLine.SetPositions(m_PositionsList.ToArray());

            // Update Line Visual
            m_CurrentLine.material = LineMaterial;
            m_CurrentLine.startWidth = LineWidth;

            // Add position
            m_PositionsList.Add(MovementSource.position);
        }

        void StartMovement()
        {
            m_IsMoving = true;
            m_PositionsList.Clear();

            // Create Line
            GameObject lineGameObject = new GameObject("Line");
            m_CurrentLine = lineGameObject.AddComponent<LineRenderer>();

            UpdateLine();
        }

        void UpdateMovement()
        {
            // Check if we have a line
            if (!m_CurrentLine || m_PositionsList.Count == 0)
                return;


            Vector3 lastPosition = m_PositionsList[m_PositionsList.Count - 1];

            // faster to use sqrMagnitude than Vector3.Distance
            float sqrNewPositionThresholdDistance = NewPositionThresholdDistance * NewPositionThresholdDistance;
            float sqrDistance = (MovementSource.position - lastPosition).sqrMagnitude;

            if (sqrDistance > sqrNewPositionThresholdDistance)
            {
                UpdateLine();
            }
        }

        void EndMovement()
        {
            m_IsMoving = false;

            // Create a new gesture based on the positions list we have
            {
                Point[] pointArray = new Point[m_PositionsList.Count];

                for (int i = 0; i < pointArray.Length; i++)
                {
                    Vector2 screenPoint = Camera.main.WorldToScreenPoint(m_PositionsList[i]);
                    pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
                }

                Gesture newGesture = new Gesture(pointArray);
            }

            // Either add a new gesture or recognize the gesture
            {
                // Add a new gesture to gesture list
                if (CreationMode)
                {
                    newGesture.Name = NewGestureName;
                    m_GestureList.Add(newGesture);

                    string fileName = Application.persistentDataPath + "/" + NewGestureName + m_GestureFileExtension;
                    GestureIO.WriteGesture(pointArray, NewGestureName, fileName);
                }
                // Recognize
                else
                {
                    Result result = PointCloudRecognizer.Classify(newGesture, m_GestureList.ToArray());
                    Debug.Log(result.GestureClass + result.Score);

                    if (result.Score > RecognitionThreshold)
                    {
                        onRecognized.Invoke(result.GestureClass);
                    }
                }
            }


            // Destroy the line we drew
            Destroy(m_CurrentLine.gameObject, LineDespawnTime);
            m_CurrentLine = null;
        }
    }
}