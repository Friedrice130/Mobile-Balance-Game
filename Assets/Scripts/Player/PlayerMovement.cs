using UnityEngine;
using UnityEngine.InputSystem;

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

        // Mobile Touch Input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            isHolding = true;

            // Get the change in finger position and normalize it across diff phone screen size
            inputDeltaX = Touchscreen.current.primaryTouch.delta.ReadValue().x / Screen.width; 
        }
        // PC Editor Fallback Input (for Editor Testing)
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            isHolding = true;
            // Get the change in mouse position
            inputDeltaX = Mouse.current.delta.ReadValue().x * 0.05f;
        }

        if (isHolding)
        {
            targetXPosition += inputDeltaX * strafeSensitivity;
            targetXPosition = Mathf.Clamp(targetXPosition, -maxStrafeX, maxStrafeX);
        }
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
