using UnityEngine;

public class AutoTurnCorner : MonoBehaviour
{
    [Header("Turn Settings")]
    public float turnAngle = 90f;
    public float turnDuration = 1.5f;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            
            if (player != null)
            {
                hasTriggered = true;
                player.StartAutoTurn(turnAngle, turnDuration);
            }
        }
    }
}