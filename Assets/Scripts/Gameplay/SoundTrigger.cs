using UnityEngine;
using System.Collections;

public class SoundTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    public SoundData scareSound;
    public bool playAs2D = false;

    [Header("Trigger Behavior")]
    public bool playOnlyOnce = true;
    
    [Tooltip("If 'Play Only Once' is false, how many seconds before it can trigger again?")]
    public float cooldownTime = 3f;

    private bool hasPlayed = false;
    private bool isCooldownActive = false;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playOnlyOnce && hasPlayed) return;

            if (!playOnlyOnce && isCooldownActive) return;

            TriggerScare();
        }
    }

    private void TriggerScare()
    {
        hasPlayed = true;

        if (AudioManager.Instance != null && scareSound != null)
        {
            if (playAs2D)
            {
                AudioManager.Instance.Play2D(scareSound); 
            }
            else
            {
                AudioManager.Instance.PlayAtPoint(scareSound, transform.position); 
            }
        }

        if (!playOnlyOnce)
        {
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldownActive = true;

        yield return new WaitForSeconds(cooldownTime);
        
        isCooldownActive = false;
    }
}