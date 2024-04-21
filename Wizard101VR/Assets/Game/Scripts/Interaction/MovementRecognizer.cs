using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PDollarGestureRecognizer;
using System.IO;
using System.Linq;

namespace Unity.Game.Interaction
{
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

        [Tooltip("Set this to a value greater than 0, to get rounded corners on each end of the line")]
        public int EndCapVertices = 8; 

        [Header("Gesture Recognition")]
        [Tooltip("True = Create a new gesture, False = Compare with recorded gestures")]
        public bool CreationMode = true;

        [Tooltip("Name of the new gesture when in Creation Mode")]
        public string NewGestureName;

        [Tooltip("How accurate the gesture should be. Turn down to 0 to let the SpellcastManager handle the recognition threshold.")]
        [Range(0f, 1f)]
        public float RecognitionThreshold = 0.9f;
        


        // - - - - - - - - - - E V E N T S - - - - - - - - - - //
        [Tooltip("Event for when gesture is recognized with an accuracy equal to or greater than the Recognition Threshold.")]
        public OnRecognizedEvent onRecognized;

        [System.Serializable]
        public class OnRecognizedEvent : UnityEvent<string, float> { }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        // - - - - - - - - - - P R I V A T E - - - - - - - - - - //
        List<Gesture> m_GestureList = new List<Gesture>();
        List<Vector3> m_PositionsList = new List<Vector3>();
        LineRenderer m_CurrentLine;
        string m_GestureFileExtension = ".xml";

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -//

        void Start()
        {
            // Get all recorded gestures
            string directoryPath = Path.Combine(Application.dataPath, "Game", "Gestures");
            string[] gestureFiles = Directory.GetFiles(directoryPath, "*" + m_GestureFileExtension);
            foreach (var item in gestureFiles)
            {
                m_GestureList.Add(GestureIO.ReadGestureFromFile(item));
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

        public void StartMovement()
        {
            m_PositionsList.Clear();

            // Create Line
            GameObject lineGameObject = new GameObject("Line");
            m_CurrentLine = lineGameObject.AddComponent<LineRenderer>();

            m_CurrentLine.numCapVertices = EndCapVertices;

            UpdateLine();
        }

        public void UpdateMovement()
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

        public void EndMovement(string gestureToCompare="")
        {
            // Destroy the line we drew
            Destroy(m_CurrentLine.gameObject, LineDespawnTime);
            m_CurrentLine = null;

            List<Gesture> gestures = m_GestureList;

            //If given a gesture, we only compare against this gesture
            if (gestureToCompare != "" && gestureToCompare != null)
            {
                gestures = m_GestureList.Where(gesture => gesture.Name == gestureToCompare.ToLower()).ToList();
            }

            Point[] pointArray = new Point[m_PositionsList.Count];

            for (int i = 0; i < pointArray.Length; i++)
            {
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(m_PositionsList[i]);
                pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
            }

            Gesture newGesture = new Gesture(pointArray);

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
                Result result = PointCloudRecognizer.Classify(newGesture, gestures.ToArray());
                Debug.Log(result.GestureClass + result.Score);

                if (result.Score > RecognitionThreshold)
                {
                    onRecognized.Invoke(result.GestureClass, result.Score);
                }
            }
        }
    }
}