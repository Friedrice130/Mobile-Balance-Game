using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup sfxMixerGroup;
    public AudioMixerGroup uiMixerGroup;
    public AudioMixerGroup musicMixerGroup;

    [Header("Audio Pool")]
    [Tooltip("How many sounds can play at the exact same time before recycling older ones.")]
    public int poolSize = 15;

    // The SFX Pool
    private List<AudioSource> audioPool = new List<AudioSource>();
    private int poolIndex = 0;

    private AudioSource uiSource2D;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePool();
    }

    private void InitializePool()
    {
        // For UI sounds
        GameObject uiObj = new GameObject("AudioSource_UI");
        uiObj.transform.SetParent(transform);
        uiSource2D = uiObj.AddComponent<AudioSource>();
        if (uiMixerGroup != null) uiSource2D.outputAudioMixerGroup = uiMixerGroup;

        // For MUSIC sound
        GameObject musicObj = new GameObject("AudioSource_Music");
        musicObj.transform.SetParent(transform);
        musicSource = musicObj.AddComponent<AudioSource>();
        musicSource.loop = true;
        if (musicMixerGroup != null) musicSource.outputAudioMixerGroup = musicMixerGroup;

        // For 3D SFX pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject poolObj = new GameObject($"AudioSource_3D_{i}");
            poolObj.transform.SetParent(transform);
            
            AudioSource source = poolObj.AddComponent<AudioSource>();
            if (sfxMixerGroup != null) source.outputAudioMixerGroup = sfxMixerGroup;
            
            source.spatialBlend = 1f; // Fully 3D
            source.playOnAwake = false;

            audioPool.Add(source);
        }
    }

    // Play sound at specific location in 3D space
    public void PlayAtPoint(SoundData data, Vector3 position)
    {
        if (data == null) return;
        AudioClip clip = data.GetRandomClip();
        if (clip == null) return;

        AudioSource source = audioPool[poolIndex];
        source.transform.position = position;

        // Apply data settings
        source.clip = clip;
        source.volume = Random.Range(data.minVolume, data.maxVolume);
        source.pitch = Random.Range(data.minPitch, data.maxPitch);
        source.spatialBlend = data.spatialBlend;
        source.minDistance = data.minDistance;
        source.maxDistance = data.maxDistance;
        source.dopplerLevel = 0f;
        
        source.Play();

        poolIndex++;
        if (poolIndex >= poolSize)
        {
            poolIndex = 0;
        }
    }

    // Play looping 3D sound
    public AudioSource PlayLooping3DSound(GameObject targetObject, SoundData data)
    {
        if (data == null || data.clips == null || data.clips.Length == 0)
        {
            Debug.LogError($"[Audio Error] SoundData is missing or contains no clips on {targetObject.name}");
            return null;
        }

        AudioSource source = targetObject.AddComponent<AudioSource>();

        source.clip = data.clips[0];
        source.loop = true;

        source.spatialBlend = data.spatialBlend;
        source.minDistance = data.minDistance;
        source.maxDistance = data.maxDistance;
        source.volume = data.maxVolume;

        if (sfxMixerGroup != null) source.outputAudioMixerGroup = sfxMixerGroup;

        source.Play();
        
        return source;
    }

    // Play UI sound
    public void Play2D(SoundData data)
    {
        if (data == null) return;
        AudioClip clip = data.GetRandomClip();
        if (clip == null) return;

        uiSource2D.pitch = Random.Range(data.minPitch, data.maxPitch);
        uiSource2D.PlayOneShot(clip, Random.Range(data.minVolume, data.maxVolume));
    }

    // Play Music sound
    public void PlayMusic(SoundData data)
    {
        if (data == null || data.clips.Length == 0) return;
        
        musicSource.clip = data.clips[0];
        musicSource.volume = data.maxVolume; 
        musicSource.Play();
    }

    public void StopAllGameplayAudio()
    {
        if (musicSource != null && musicSource.isPlaying) musicSource.Stop();

        foreach (AudioSource source in audioPool)
        {
            if (source != null && source.isPlaying) source.Stop();
        }

        DynamicWeatherSystem.AudioModule weatherAudio = FindObjectOfType<DynamicWeatherSystem.AudioModule>();
        if (weatherAudio != null)
        {
            weatherAudio.StopWeatherAudio();
        }
        
        LightningGenerator lightning = FindObjectOfType<LightningGenerator>();
        if (lightning != null)
        {
            lightning.StopStorm();
        }
    }
}