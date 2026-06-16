using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    [Header("Hazard Setting")]
    public Rigidbody targetObject;
    public float outwardPushForce = 10f;
    public float downwardSmashForce = 15f;
    public float spinForce = 50f;

    [Header("Audio")]
    public SoundData snapSound;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            if (targetObject != null)
            {
                targetObject.isKinematic = false;
                
                // Combine forces
                Vector3 smashDirection = (targetObject.transform.forward * outwardPushForce) + (Vector3.down * downwardSmashForce);
                targetObject.AddForce(smashDirection, ForceMode.Impulse);

                targetObject.AddTorque(new Vector3(
                    Random.Range(-spinForce, spinForce), 
                    Random.Range(-spinForce, spinForce), 
                    Random.Range(-spinForce, spinForce)), 
                    ForceMode.Impulse);
            }

            if (AudioManager.Instance != null && snapSound != null)
            {
                AudioManager.Instance.PlayAtPoint(snapSound, transform.position);
            }
        }
    }
}
