using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using PDollarGestureRecognizer;
using System.IO;

namespace Unity.Game.Gameplay
{
    public class MovementRecognizer : MonoBehaviour
    {
        [Header("General")]
        public InputActionProperty ActivateButton;
        public float ActivationThreshold = 0.1f;
        public Transform MovementSource;

        [Header("Line Rendering Parameters")]
        public float NewPositionThresholdDistance = 0.05f;
        public float LineWidth = 0.03f;
        public Material LineMaterial;

        [Header("Gesture Recognition")]
        public bool CreationMode = true;
        public string NewGestureName;
        public float RecognitionThreshold = 0.9f;


        public OnRecognizedEvent onRecognized;

        [System.Serializable]
        public class OnRecognizedEvent : UnityEvent<string> { }


        List<Gesture> m_TrainingSet = new List<Gesture>();
        bool m_IsMoving = false;
        List<Vector3> m_PositionsList = new List<Vector3>();
        LineRenderer m_CurrentLine;


        // Start is called before the first frame update
        void Start()
        {
            // Get all the stored gestures
            string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
            foreach (var item in gestureFiles)
            {
                m_TrainingSet.Add(GestureIO.ReadGestureFromFile(item));
            }
        }

        // Update is called once per frame
        void Update()
        {
            bool isPressed = ActivateButton.action.ReadValue<float>() > ActivationThreshold;

            // Start the Movement
            if (!m_IsMoving && isPressed)
            {
                StartMovement();
            }
            // Ending the Movement
            else if (m_IsMoving && !isPressed)
            {
                EndMovement();
            }
            // Updating the Movement
            else if (m_IsMoving && isPressed)
            {
                UpdateMovement();
            }
        }

        public void SetLineMaterial(Material newMaterial)
        {
            LineMaterial = newMaterial;
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
            //m_PositionsList.Add(MovementSource.position);

            // Create Line
            GameObject lineGameObject = new GameObject("Line");
            m_CurrentLine = lineGameObject.AddComponent<LineRenderer>();

            UpdateLine();
        }

        void EndMovement()
        {
            m_IsMoving = false;

            // Create the Gesture from the position list
            Point[] pointArray = new Point[m_PositionsList.Count];

            for (int i = 0; i < m_PositionsList.Count; i++)
            {
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(m_PositionsList[i]);
                pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
            }

            Gesture newGesture = new Gesture(pointArray);

            // Add a new gesture to training set
            if (CreationMode)
            {
                newGesture.Name = NewGestureName;
                m_TrainingSet.Add(newGesture);

                string fileName = Application.persistentDataPath + "/" + NewGestureName + ".xml";
                GestureIO.WriteGesture(pointArray, NewGestureName, fileName);
            }
            // recognize
            else
            {
                Result result = PointCloudRecognizer.Classify(newGesture, m_TrainingSet.ToArray());
                Debug.Log(result.GestureClass + result.Score);

                if (result.Score > RecognitionThreshold)
                {
                    onRecognized.Invoke(result.GestureClass);
                }
            }

            Destroy(m_CurrentLine.gameObject, 2f);

            m_CurrentLine = null;
        }

        void UpdateMovement()
        {
            // Check if we have a line
            if (!m_CurrentLine || m_PositionsList.Count == 0)
                return;


            Vector3 lastPosition = m_PositionsList[m_PositionsList.Count - 1];

            if (Vector3.Distance(MovementSource.position, lastPosition) > NewPositionThresholdDistance)
            {
                UpdateLine();
            }
        }
    }
}
