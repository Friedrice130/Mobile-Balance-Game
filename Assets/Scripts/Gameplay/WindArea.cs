using UnityEngine;

public class WindArea : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 windDirection = Vector3.right;
    public float windStrength = 10f;

    [Header("Audio Settings")]
    public SoundData windLoopSound;
    private AudioSource windAudioSource;

    void Start()
    {
        if (AudioManager.Instance != null && windLoopSound != null)
        {
            windAudioSource = AudioManager.Instance.PlayLooping3DSound(gameObject, windLoopSound);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Rigidbody itemRb = other.GetComponent<Rigidbody>();
            
            if (itemRb != null)
            {
                itemRb.AddForce(windDirection.normalized * windStrength, ForceMode.Force);
            }
        }
    }
}
