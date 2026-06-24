using UnityEngine;
using System.Collections;

public enum DoorType { NormalOpen, ScareSlam }

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public DoorType doorType = DoorType.NormalOpen;
    public float normalOpenSpeed = 1f;
    public float scareOpenSlowSpeed = 0.3f;
    public float scareSlamSpeed = -5f;
    [Tooltip("How many seconds to wait before the slam.")]
    public float delayBeforeSlam = 4f;
    public Animator doorAnimator;

    [Header("Audio Settings")]
    public SoundData openSound;
    public SoundData slamSound;

    private AudioSource creakSource;
    private bool hasTriggered = false;

    void Start()
    {
        creakSource = gameObject.AddComponent<AudioSource>();
        creakSource.spatialBlend = 1f;
        
        if (AudioManager.Instance != null && AudioManager.Instance.sfxMixerGroup != null)
        {
            creakSource.outputAudioMixerGroup = AudioManager.Instance.sfxMixerGroup;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            if (doorType == DoorType.NormalOpen)
            {
                OpenNormal();
            }
            else if (doorType == DoorType.ScareSlam)
            {
                StartCoroutine(ScareSlamRoutine());
            }
        }
    }

    private void PlayDoorCreak(float speedMultiplier)
    {
        if (openSound != null && creakSource != null)
        {
            AudioClip clip = openSound.GetRandomClip();
            if (clip != null)
            {
                creakSource.clip = clip;
                
                creakSource.volume = Random.Range(openSound.minVolume, openSound.maxVolume);
                creakSource.pitch = speedMultiplier; 
                
                creakSource.Play();
            }
        }
    }

    private void OpenNormal()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetFloat("SpeedMultiplier", normalOpenSpeed);
            doorAnimator.CrossFade("DoorSwing", 0.1f);

            PlayDoorCreak(normalOpenSpeed);
        }
    }

    private IEnumerator ScareSlamRoutine()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetFloat("SpeedMultiplier", scareOpenSlowSpeed);
            doorAnimator.CrossFade("DoorSwing", 0.1f);

            PlayDoorCreak(scareOpenSlowSpeed);

            yield return new WaitForSeconds(delayBeforeSlam);

            if (creakSource != null && creakSource.isPlaying)
            {
                creakSource.Stop();
            }

            doorAnimator.SetFloat("SpeedMultiplier", scareSlamSpeed);

            float currentPlaybackProgress = doorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (currentPlaybackProgress > 1f) currentPlaybackProgress = 1f;

            doorAnimator.Play("DoorSwing", 0, currentPlaybackProgress);

            if (AudioManager.Instance != null && slamSound != null)
            {
                AudioManager.Instance.PlayAtPoint(slamSound, transform.position);
            }
        }
    }
}