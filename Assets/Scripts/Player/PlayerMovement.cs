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

        // Apply final movement
        Vector3 forwardMove = Vector3.forward * currentForwardSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = new Vector3(newXPosition, rb.position.y, rb.position.z) + forwardMove;

        rb.MovePosition(newPosition);
    }
}
