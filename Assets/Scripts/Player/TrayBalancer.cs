using UnityEngine;
using UnityEngine.InputSystem;

public class TrayBalancer : MonoBehaviour
{
    [Header("Physics Anchor")]
    public Transform targetAnchor;

    [Header("Tilt Settings - Left/Right")]
    public float maxTiltAngleLR = 45f;
    public bool invertTiltLR = false;

    [Header("Tilt Settings - Front/Back")]
    public float maxTiltAngleFB = 30f;
    public bool invertTiltFB = false;

    [Header("Shared Settings")]
    public float tiltSmoothness = 15f;

    [Header("Player Preferences")]
    [Range(0.5f, 3f)]
    [Tooltip("How much physical wrist movement is required. Higher = less wrist turning needed.")]
    public float tiltSensitivity = 1.5f;

    private Rigidbody rb;
    private float currentTiltAngleLR = 0f;
    private float currentTiltAngleFB = 0f;

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

        // Only allow tray to tilt if game state is Playing
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing) return;
        
        float targetTiltLR = 0f;
        float targetTiltFB = 0f;

        // Mobile Accelerometer Input
        if (Accelerometer.current != null)
        {
            // x is left/right tilt, y is front/back tilt
            Vector3 accel = Accelerometer.current.acceleration.ReadValue();

            // Left/Right (Roll)
            float sensitiveAccelX = Mathf.Clamp(accel.x * tiltSensitivity, -1f, 1f);
            targetTiltLR = sensitiveAccelX * maxTiltAngleLR;

            // Front/Back (Pitch)
            float sensitiveAccelY = Mathf.Clamp(accel.y * tiltSensitivity, -1f, 1f);
            targetTiltFB = sensitiveAccelY * maxTiltAngleFB;
        }
        // PC Editor Fallback Input
        else if (Keyboard.current != null)
        {
            // Left/Right
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                targetTiltLR = -maxTiltAngleLR;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                targetTiltLR = maxTiltAngleLR;

            // Front/Back
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                targetTiltFB = maxTiltAngleFB;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                targetTiltFB = -maxTiltAngleFB;
        }

        // Apply inversions if needed
        if (invertTiltLR) targetTiltLR *= -1f;
        if (invertTiltFB) targetTiltFB *= -1f;

        currentTiltAngleLR = Mathf.Lerp(currentTiltAngleLR, targetTiltLR, tiltSmoothness * Time.fixedDeltaTime);
        currentTiltAngleFB = Mathf.Lerp(currentTiltAngleFB, targetTiltFB, tiltSmoothness * Time.fixedDeltaTime);

        // Move & Rotate
        rb.MovePosition(targetAnchor.position);

        // Apply rotation
        Quaternion localTilt = Quaternion.Euler(currentTiltAngleFB, 0f, -currentTiltAngleLR);
        rb.MoveRotation(targetAnchor.rotation * localTilt);
    }
}
