using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    [Header("Hazard Setting")]
    public Rigidbody targetObject;
    public float outwardPushForce = 2f;
    public float downwardSmashForce = 15f;

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
                targetObject.AddForce(targetObject.transform.forward * outwardPushForce, ForceMode.Impulse);
                targetObject.AddForce(Vector3.down * downwardSmashForce, ForceMode.Impulse);
                targetObject.AddTorque(new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)), ForceMode.Impulse);
            }

            if (AudioManager.Instance != null && snapSound != null)
            {
                AudioManager.Instance.PlayAtPoint(snapSound, transform.position);
            }
        }
    }
}
