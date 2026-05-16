using UnityEngine;
using UnityEngine.UI; 

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public GameObject pauseButton;
    public Slider sensitivitySlider;
    public Toggle invertToggle;

    [Header("Game References")]
    public TrayBalancer trayBalancer;

    void Start()
    {
        if (trayBalancer != null)
        {
            sensitivitySlider.value = trayBalancer.tiltSensitivity;
            invertToggle.isOn = trayBalancer.invertTilt;
        }

        ResumeGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; 
        pauseMenuPanel.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; 
        pauseMenuPanel.SetActive(false);
        pauseButton.SetActive(true);
    }

    public void OnInvertToggled(bool isToggled)
    {
        if (trayBalancer != null)
        {
            trayBalancer.invertTilt = isToggled;
        }
    }

    public void OnSensitivityChanged(float sliderValue)
    {
        if (trayBalancer != null)
        {
            trayBalancer.tiltSensitivity = sliderValue;
        }
    }
}
