using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float topSpeed = 5f;
    public float acceleration = 5f;

    [Header("Strafe Settings")]
    public float strafeSensitivity = 15f; // How far a drag moves the player
    public float maxStrafeX = 4f;
    public float strafeSmoothness = 10f;

    [Header("Terrain Interaction")]
    public LayerMask groundLayer;
    [Tooltip("How high from the player base the laser starts")]
    public float groundCheckHeight = 1f; 
    [Tooltip("The normal height of the player center from the floor")]
    public float groundOffset = 1f; 
    
    [Tooltip("Higher = sharper, more violent bumps. Lower = smooth, wave-like bumps")]
    public float bumpHarshness = 20f; 
    
    public bool alignToSlopes = true;
    [Tooltip("How fast the tray tilts when entering a ramp")]
    public float slopeSmoothness = 10f;

    [Header("Audio Settings")]
    public SoundData footstepSound; 
    public float footstepInterval = 0.4f; // How fast the player takes a step
    private float footstepTimer = 0f;
    
    private Rigidbody rb;
    private float targetXPosition;
    private float currentForwardSpeed;
    private bool isHolding;
    private float inputDeltaX;
    private bool ignoreCurrentInput = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Track starting X position
        targetXPosition = transform.position.x;
    }

    void Update()
    {
        isHolding = false;
        inputDeltaX = 0f;

        // Only allow input if the game state is Playing
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing) return;

        // Do not process any input when game paused
        if (Time.timeScale == 0f) return;

        bool isPressing = false;
        bool justPressed = false;

        // Mobile Touch Input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            isPressing = true;
            justPressed = Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

            // Get the change in finger position and normalize it across diff phone screen size
            inputDeltaX = Touchscreen.current.primaryTouch.delta.ReadValue().x / Screen.width; 
        }
        // PC Editor Fallback Input (for Editor Testing)
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            isPressing = true;
            justPressed = Mouse.current.leftButton.wasPressedThisFrame;

            // Get the change in mouse position
            inputDeltaX = Mouse.current.delta.ReadValue().x * 0.05f;
        }

        if (!isPressing)
        {
            ignoreCurrentInput = false;
            return;
        }

        if (justPressed)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                ignoreCurrentInput = true;
            }
        }

        // Ignore movement if this touch started on a UI element
        if (ignoreCurrentInput) return;

        // Valid movement input
        isHolding = true;
        targetXPosition += inputDeltaX * strafeSensitivity;
        targetXPosition = Mathf.Clamp(targetXPosition, -maxStrafeX, maxStrafeX);
    }

    void FixedUpdate()
    {
        // Move Forward
        float targetSpeed = isHolding ? topSpeed : 0f;
        currentForwardSpeed = Mathf.Lerp(currentForwardSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // Strafe Sideways
        float newXPosition = Mathf.Lerp(rb.position.x, targetXPosition, strafeSmoothness * Time.fixedDeltaTime);

        // Terrain Detection
        float newYPosition = rb.position.y;
        Quaternion targetRotation = rb.rotation;

        Vector3 rayStart = new Vector3(newXPosition, rb.position.y + groundCheckHeight, rb.position.z);

        Debug.DrawRay(rayStart, Vector3.down * (groundCheckHeight * 2f), Color.red);

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, groundCheckHeight * 2f, groundLayer))
        {
            // Handle Bumps (Adjust Y position based on terrain height)
            float targetY = hit.point.y + groundOffset;
            newYPosition = Mathf.Lerp(rb.position.y, targetY, bumpHarshness * Time.fixedDeltaTime);

            // Handle Ramps (Tilt the player based on the angle of the terrain)
            if (alignToSlopes)
            {
                Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                targetRotation = Quaternion.Slerp(rb.rotation, slopeRotation, slopeSmoothness * Time.fixedDeltaTime);
            }
        }
        else
        {
            targetRotation = Quaternion.Slerp(rb.rotation, Quaternion.identity, slopeSmoothness * Time.fixedDeltaTime);
        }

        // Apply final movement
        Vector3 forwardMove = Vector3.forward * currentForwardSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = new Vector3(newXPosition, newYPosition, rb.position.z) + forwardMove;

        rb.MovePosition(newPosition);
        rb.MoveRotation(targetRotation);

        if (isHolding && currentForwardSpeed > 0.5f)
        {
            footstepTimer -= Time.fixedDeltaTime;

            if (footstepTimer <= 0f)
            {
                if (AudioManager.Instance != null && footstepSound != null)
                {
                    AudioManager.Instance.PlayAtPoint(footstepSound, transform.position);
                }

                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
}
