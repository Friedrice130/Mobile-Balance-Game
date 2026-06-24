using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("Visual References")]
    public Light lampLight;
    public Renderer bulbRenderer;
    public int bulbMaterialIndex = 0;

    [Header("Light Flicker")]
    public float minIntensity = 0.2f;
    public float maxIntensity = 1.5f;
    public float flickerSpeed = 10f;
    
    [Header("Sudden Glitch")]
    [Range(0f, 0.1f)]
    public float glitchChance = 0.01f;
    public float maxGlitchDuration = 0.2f;

    [Header("Audio Settings")]
    public SoundData buzzSound;
    
    private AudioSource buzzSource;
    private Material bulbMat;
    private Color baseEmissionColor;
    
    private float glitchTimer = 0f;
    private float randomOffset;

    void Start()
    {
        randomOffset = Random.Range(0f, 100f); 

        // Bulb material
        if (bulbRenderer != null)
        {
            bulbMat = bulbRenderer.materials[bulbMaterialIndex];
            
            if (bulbMat.HasProperty("_EmissionColor"))
            {
                baseEmissionColor = bulbMat.GetColor("_EmissionColor");
            }
            else
            {
                baseEmissionColor = Color.white; 
            }
        }

        // Audio
        if (buzzSound != null && AudioManager.Instance != null)
        {
            buzzSource = gameObject.AddComponent<AudioSource>();
            buzzSource.clip = buzzSound.GetRandomClip();
            buzzSource.loop = true;
            buzzSource.spatialBlend = 1f;
            buzzSource.minDistance = buzzSound.minDistance;
            buzzSource.maxDistance = buzzSound.maxDistance;
            buzzSource.outputAudioMixerGroup = AudioManager.Instance.sfxMixerGroup;
            buzzSource.Play();
        }
    }

    void Update()
    {
        float currentIntensity = 0f;

        if (glitchTimer > 0f)
        {
            glitchTimer -= Time.deltaTime;
            currentIntensity = 0f;
        }
        else
        {
            float noise = Mathf.PerlinNoise((Time.time + randomOffset) * flickerSpeed, 0f);
            currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

            if (Random.value < glitchChance)
            {
                glitchTimer = Random.Range(0.05f, maxGlitchDuration);
                currentIntensity = 0f;
            }
        }

        if (lampLight != null)
        {
            lampLight.intensity = currentIntensity;
        }

        if (bulbMat != null)
        {
            bulbMat.SetColor("_EmissionColor", baseEmissionColor * currentIntensity);
        }

        if (buzzSource != null)
        {
            float normalizedPower = currentIntensity / maxIntensity;
            buzzSource.volume = buzzSound.maxVolume * normalizedPower;
            buzzSource.pitch = Mathf.Lerp(0.8f, 1.2f, normalizedPower); 
        }
    }
}