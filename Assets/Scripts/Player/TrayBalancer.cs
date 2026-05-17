using UnityEngine;
using UnityEngine.InputSystem;

public class TrayBalancer : MonoBehaviour
{
    [Header("Physics Anchor")]
    public Transform targetAnchor;

    [Header("Tilt Settings")]
    public float maxTiltAngle = 45f;
    public float tiltSmoothness = 15f;
    public bool invertTilt = false;

    [Header("Player Preferences")]
    [Range(0.5f, 3f)]
    [Tooltip("How much physical wrist movement is required. Higher = less wrist turning needed.")]
    public float tiltSensitivity = 1.5f;

    private Rigidbody rb;
    private float currentTiltAngle = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }
    }

    void FixedUpdate()
    {
        if (targetAnchor == null) return;
        
        float targetTilt = 0f;

        // Mobile Accelerometer Input
        if (Accelerometer.current != null)
        {
            // Get the raw phone tilt (-1 to 1)
            float rawAccelX = Accelerometer.current.acceleration.ReadValue().x;
            
            // Sensitivity
            float sensitiveAccelX = rawAccelX * tiltSensitivity;
            // Avoid the tray to spin upside down
            sensitiveAccelX = Mathf.Clamp(sensitiveAccelX, -1f, 1f);

            targetTilt = sensitiveAccelX * maxTiltAngle;
        }
        // PC Editor Fallback Input
        else if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                targetTilt = -maxTiltAngle;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                targetTilt = maxTiltAngle;
        }

        if (invertTilt) targetTilt *= -1f;

        currentTiltAngle = Mathf.Lerp(currentTiltAngle, targetTilt, tiltSmoothness * Time.fixedDeltaTime);

        rb.MovePosition(targetAnchor.position);
        Quaternion localTilt = Quaternion.Euler(0f, 0f, -currentTiltAngle);
        rb.MoveRotation(targetAnchor.rotation * localTilt);
    }
}
