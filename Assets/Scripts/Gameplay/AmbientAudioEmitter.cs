using UnityEngine;

public class AmbientAudioEmitter : MonoBehaviour
{
    [Header("Ambient Audio Settings")]
    public SoundData ambientSound;

    private AudioSource loopingSource;

    void Start()
    {
        if (AudioManager.Instance != null && ambientSound != null)
        {
            loopingSource = AudioManager.Instance.PlayLooping3DSound(this.gameObject, ambientSound);
        }
    }

    void OnDestroy()
    {
        if (loopingSource != null)
        {
            loopingSource.Stop();
        }
    }
}
