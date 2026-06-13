using UnityEngine;
using System.Collections;

public class LightningGenerator : MonoBehaviour
{
    [Header("Lightning Visuals")]
    public Light flashLight;
    
    [Header("Thunder Audio")]
    public SoundData thunderSound;

    [Header("Timing")]
    public float minSecondsBetweenFlashes = 3f;
    public float maxSecondsBetweenFlashes = 8f;

    private bool isStorming = false;

    void Start()
    {
        if (flashLight != null) flashLight.enabled = false;
    }

    public void StartStorm()
    {
        if (isStorming) return;
        isStorming = true;
        StartCoroutine(LightningRoutine());
    }

    private IEnumerator LightningRoutine()
    {
        while (isStorming)
        {
            yield return new WaitForSeconds(Random.Range(minSecondsBetweenFlashes, maxSecondsBetweenFlashes));

            if (flashLight != null)
            {
                flashLight.enabled = true;
                yield return new WaitForSeconds(0.05f); // Quick flash
                flashLight.enabled = false;
                yield return new WaitForSeconds(0.05f); // Dark gap
                flashLight.enabled = true;
                yield return new WaitForSeconds(0.1f);  // Longer flash
                flashLight.enabled = false;
            }

            if (AudioManager.Instance != null && thunderSound != null)
            {
                AudioManager.Instance.Play2D(thunderSound); 
            }
        }
    }
}