using UnityEngine;
using DynamicWeatherSystem;

public class WeatherTrigger : MonoBehaviour
{
    [Header("Weather Settings")]
    public WeatherStateData targetWeather;
    public float transitionDuration = 5f;

    [Header("Optional")]
    public LightningGenerator optionalLightning;

    private bool hasTriggered = false;
    private WeatherManager cachedManager;

    void Start()
    {
        cachedManager = FindObjectOfType<WeatherManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            if (cachedManager != null && targetWeather != null)
            {
                cachedManager.SetWeather(targetWeather, transitionDuration);
            }

            if (optionalLightning != null)
            {
                optionalLightning.StartStorm();
            }
        }
    }
}
