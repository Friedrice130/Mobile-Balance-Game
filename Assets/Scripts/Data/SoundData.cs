using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundData", menuName = "Game/Sound Data")]
public class SoundData : ScriptableObject
{
    [Header("Audio Clips")]
    public AudioClip[] clips;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float minVolume = 0.8f;
    [Range(0f, 1f)] public float maxVolume = 1f;

    [Header("Pitch Settings")]
    [Range(0.1f, 3f)] public float minPitch = 0.9f;
    [Range(0.1f, 3f)] public float maxPitch = 1.1f;

    [Header("Spatial Settings (3D)")]
    [Range(0f, 1f)] public float spatialBlend = 1f; // 1 = fully 3d (directional)
    public float minDistance = 5f;
    public float maxDistance = 30f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}
