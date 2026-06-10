using UnityEngine;

public class ImpactSound : MonoBehaviour
{
    [Header("Impact Audio")]
    public SoundData smashSound;
    [Tooltip("How hard it needs to hit the ground to play the sound")]
    public float minForceToPlay = 2f;

    private bool hasSmashed = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!hasSmashed && collision.relativeVelocity.magnitude > minForceToPlay)
        {
            hasSmashed = true;

            if (AudioManager.Instance != null && smashSound != null)
            {
                AudioManager.Instance.PlayAtPoint(smashSound, transform.position);
            }
        }
    }
}
