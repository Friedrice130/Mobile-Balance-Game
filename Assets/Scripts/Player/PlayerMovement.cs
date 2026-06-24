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

    [Header("Collision Settings")]
    public LayerMask obstacleLayer;
    public float playerRadius = 0.4f;
    public float trayReach = 0.6f;

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
    private Vector3 pathCenter;
    private float targetStrafeOffset;
    private float currentStrafeOffset;
    private bool isTurning = false;
    private Quaternion turnStartRotation;
    private Quaternion turnTargetRotation;
    private float turnElapsed = 0f;
    private float turnDuration = 1f;

    public void StartAutoTurn(float angle, float duration)
    {
        if (isTurning) return;
        
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        turnStartRotation = Quaternion.LookRotation(flatForward, Vector3.up);
        turnTargetRotation = turnStartRotation * Quaternion.Euler(0, angle, 0);
        turnDuration = duration;
        turnElapsed = 0f;
        isTurning = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Track center line
        pathCenter = transform.position;
        targetStrafeOffset = 0f;
        currentStrafeOffset = 0f;
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
        targetStrafeOffset += inputDeltaX * strafeSensitivity;
        targetStrafeOffset = Mathf.Clamp(targetStrafeOffset, -maxStrafeX, maxStrafeX);
    }

    void FixedUpdate()
    {
        // Move Forward
        float targetSpeed = isHolding ? topSpeed : 0f;
        currentForwardSpeed = Mathf.Lerp(currentForwardSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        // Auto Turnm
        Vector3 referenceForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        
        if (isTurning)
        {
            turnElapsed += Time.fixedDeltaTime;
            float t = turnElapsed / turnDuration;
            if (t > 1f) t = 1f;
            
            float smoothT = t * t * (3f - 2f * t);
            Quaternion currentYaw = Quaternion.Slerp(turnStartRotation, turnTargetRotation, smoothT);
            referenceForward = currentYaw * Vector3.forward;

            if (t >= 1f) isTurning = false;
        }

        // Terrain Detection
        float targetY = rb.position.y;
        Quaternion targetRotation = rb.rotation;
        Vector3 rayStart = rb.position + (Vector3.up * groundCheckHeight);

        // Debug
        Debug.DrawRay(rayStart, Vector3.down * (groundCheckHeight * 2f), Color.red);

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, groundCheckHeight * 2f, groundLayer))
        {
            targetY = hit.point.y + groundOffset;

            if (alignToSlopes)
            {
                Vector3 projectedForward = Vector3.ProjectOnPlane(referenceForward, hit.normal).normalized;
                Quaternion slopeRotation = Quaternion.LookRotation(projectedForward, hit.normal);
                targetRotation = Quaternion.Slerp(rb.rotation, slopeRotation, slopeSmoothness * Time.fixedDeltaTime);
            }
        }
        else
        {
            Vector3 flatForward = Vector3.ProjectOnPlane(referenceForward, Vector3.up).normalized;
            Quaternion flatRotation = Quaternion.LookRotation(flatForward, Vector3.up);
            targetRotation = Quaternion.Slerp(rb.rotation, flatRotation, slopeSmoothness * Time.fixedDeltaTime);
        }

        Vector3 currentForward = targetRotation * Vector3.forward;
        Vector3 currentRight = targetRotation * Vector3.right;

        Vector3 castStart = rb.position + (Vector3.up * groundOffset);
        Vector3 forwardMove = currentForward * currentForwardSpeed * Time.fixedDeltaTime;
        if (forwardMove.magnitude > 0.001f)
        {
            Vector3 p1 = castStart + forwardMove + (currentForward * 0.05f);
            Vector3 p2 = castStart + (currentForward * trayReach) + forwardMove + (currentForward * 0.05f);

            if (Physics.CheckCapsule(p1, p2, playerRadius, obstacleLayer, QueryTriggerInteraction.Ignore))
            {
                forwardMove = Vector3.zero; 

                // AUTO-SLIDE: If the tray hits an inside corner during a turn
                targetStrafeOffset = Mathf.MoveTowards(targetStrafeOffset, 0f, topSpeed * Time.fixedDeltaTime);
                currentStrafeOffset = Mathf.MoveTowards(currentStrafeOffset, 0f, topSpeed * Time.fixedDeltaTime);
            }
        }

        float nextStrafeOffset = Mathf.Lerp(currentStrafeOffset, targetStrafeOffset, strafeSmoothness * Time.fixedDeltaTime);
        float strafeDelta = nextStrafeOffset - currentStrafeOffset;
        Vector3 strafeMove = transform.right * strafeDelta;

        if (strafeMove.magnitude > 0.001f)
        {
            Vector3 p1 = castStart + strafeMove + (strafeMove.normalized * 0.05f);
            Vector3 p2 = castStart + (transform.forward * trayReach) + strafeMove + (strafeMove.normalized * 0.05f);
            
            if (Physics.CheckCapsule(p1, p2, playerRadius, obstacleLayer, QueryTriggerInteraction.Ignore))
            {
                // Cancel strafe momentum
                nextStrafeOffset = currentStrafeOffset;
                targetStrafeOffset = currentStrafeOffset; 
            }
        }

        pathCenter += forwardMove;
        currentStrafeOffset = nextStrafeOffset;

        Vector3 newPosition = pathCenter + (transform.right * currentStrafeOffset);
        newPosition.y = Mathf.Lerp(rb.position.y, targetY, bumpHarshness * Time.fixedDeltaTime);

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
