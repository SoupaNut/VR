using UnityEngine;
using UnityEngine.InputSystem;

public class DeckDisplay : MonoBehaviour
{
    [Header("Displays")]
    [Tooltip("Game Object that holds all the cards to display.")]
    public GameObject Display;

    [Header("Display positions")]
    [Tooltip("The position of the player's base. Can think of this as the ground point.")]
    public Transform FixedPoint;

    [Tooltip("The camera will affect the position and rotation of the deck.")]
    public Transform Camera;

    [Tooltip("X doesn't do anything. Y controls how high above the ground. Z controls how far away from the camera.")]
    public Vector3 Offset;

    [Tooltip("Max angle the player can look away from the deck display before deck display updates its position.")]
    public float MaxAngleThreshold = 45f;

    [Tooltip("How smoothly the deck display will change position.")]
    public float Smoothness = 5f;

    [Tooltip("Minimum distance the player can get to the deck display before it updates its position.")]
    public float MinDisplayDistance = 0.3f;

    public bool DeckEnabled { get; set; }

    Vector3 m_DisplayPosition;

    // Start is called before the first frame update
    void Start()
    {
        if(MinDisplayDistance > Offset.sqrMagnitude)
        {
            MinDisplayDistance = Offset.magnitude;
            Debug.LogWarning("Minimum Display Distance is too large. It has been set equal to: " + MinDisplayDistance);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Display.activeSelf)
        {
            // calculate distance from deck display to player's fixed point
            float sqrDistance = (transform.position - Camera.position).sqrMagnitude;


            // calculate the angle from the deck display forward to the player camera forward
            Vector3 displayForward = transform.forward;
            Vector3 cameraForward = Camera.forward;
            displayForward.y = 0;
            cameraForward.y = 0;

            // Set new position if the angle is too big, player is too far away or too close
            float angle = Vector3.Angle(displayForward, cameraForward);
            if (angle > MaxAngleThreshold || sqrDistance > Offset.sqrMagnitude || sqrDistance < (MinDisplayDistance * MinDisplayDistance))
            {
                UpdateDisplayPosition();
            }

            // Move the deck display if it is not at the intended position
            if (transform.position != m_DisplayPosition)
            {
                transform.position = Vector3.Lerp(transform.position, m_DisplayPosition, Smoothness * Time.deltaTime);
            }

            transform.LookAt(Camera);
            transform.forward *= -1;
        }
    }

    public void Toggle()
    {
        Display.SetActive(!Display.activeSelf);
        if (Display.activeSelf)
        {
            // Set initial deck display position to be in front of player cam
            UpdateDisplayPosition();
            transform.position = m_DisplayPosition;
        }
    }

    void UpdateDisplayPosition()
    {
        Vector3 newPosition = Camera.position + Camera.forward * Offset.z;
        newPosition.y = FixedPoint.position.y + Offset.y;
        m_DisplayPosition = newPosition;
    }
}
