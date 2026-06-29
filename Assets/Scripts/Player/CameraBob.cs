using UnityEngine;

public class CameraBob : MonoBehaviour
{
    [Header("References")]
    public Animator cameraAnim;
    public Transform playerTransform;

    [Header("Settings")]
    [Tooltip("How fast the player must be moving to trigger the Walk animation")]
    public float movementThreshold = 0.5f;
    
    [Tooltip("How quickly it blends between Idle and Walk")]
    public float blendSpeed = 0.2f;

    private Vector3 lastPosition;
    private bool isWalking = false;

    void Start()
    {
        if (cameraAnim == null) cameraAnim = GetComponent<Animator>();
        if (playerTransform != null) lastPosition = playerTransform.position;
    }

    void Update()
    {
        if (playerTransform == null || cameraAnim == null) return;

        float distanceMoved = Vector3.Distance(playerTransform.position, lastPosition);
        float currentSpeed = distanceMoved / Time.deltaTime;

        bool playerIsMoving = currentSpeed > movementThreshold;

        if (playerIsMoving && !isWalking)
        {
            isWalking = true;
            cameraAnim.CrossFade("Walk", blendSpeed);
        }
        else if (!playerIsMoving && isWalking)
        {
            isWalking = false;
            cameraAnim.CrossFade("Idle", blendSpeed);
        }
        
        lastPosition = playerTransform.position;
    }
}