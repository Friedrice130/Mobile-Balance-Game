using UnityEngine;

public class KidTrigger : MonoBehaviour
{
    public RunningKid kidToTrigger;
    
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            if (kidToTrigger != null)
            {
                kidToTrigger.StartMoving();
            }
        }
    }
}