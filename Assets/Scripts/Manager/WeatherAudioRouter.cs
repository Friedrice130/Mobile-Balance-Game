using UnityEngine;

public class WeatherAudioRouter : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.Instance == null) return;

        AudioSource[] weatherAudioSources = GetComponentsInChildren<AudioSource>();

        foreach (AudioSource src in weatherAudioSources)
        {
            src.outputAudioMixerGroup = AudioManager.Instance.musicMixerGroup;
        }
    }
}