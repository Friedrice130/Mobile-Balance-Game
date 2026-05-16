using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float strafeSpeed = 10f;
    public float maxStrafeX = 4f;
    
    private Rigidbody rb;
    private float currentXPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // Track starting X position
        currentXPosition = transform.position.x;
    }

    void FixedUpdate()
    {
        bool isHolding = false;
        float inputDeltaX = 0f;

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

        // Apply movement if holding
        if (isHolding)
        {
            // Move Forward
            Vector3 forwardMove = Vector3.forward * moveSpeed * Time.fixedDeltaTime;
            Vector3 newPosition = rb.position + forwardMove;

            // Strafe Sideways
            currentXPosition += inputDeltaX * strafeSpeed;
            currentXPosition = Mathf.Clamp(currentXPosition, -maxStrafeX, maxStrafeX);
            newPosition.x = currentXPosition;

            // Apply final position
            rb.MovePosition(newPosition);
        }
    }
}