using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public GameObject pauseButton;
    public Slider sensitivitySlider;

    [Header("Tilt UI Settings")]
    public Slider angleSliderLR; // Left/Right Angle
    public Slider angleSliderFB; // Front/Back Angle
    public Toggle invertToggleLR; // Left/Right Invert
    public Toggle invertToggleFB; // Front/Back Invert

    [Header("Audio Settings")]
    public AudioMixer mainMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Screens & Text")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;

    [Header("Game References")]
    public TrayBalancer trayBalancer;
    public int mainMenuBuildIndex = 0;

    void Start()
    {
        if (trayBalancer != null)
        {
            // Load Saved Settings (Left/Right)
            trayBalancer.tiltSensitivity = PlayerPrefs.GetFloat("SavedSensitivity", trayBalancer.tiltSensitivity);
            trayBalancer.maxTiltAngleLR = PlayerPrefs.GetFloat("SavedAngleLR", trayBalancer.maxTiltAngleLR);
            int defaultInvertLR = trayBalancer.invertTiltLR ? 1 : 0;
            trayBalancer.invertTiltLR = PlayerPrefs.GetInt("SavedInvertLR", defaultInvertLR) == 1;

            // Load Saved Settings (Front/Back)
            trayBalancer.maxTiltAngleFB = PlayerPrefs.GetFloat("SavedAngleFB", trayBalancer.maxTiltAngleFB);
            int defaultInvertFB = trayBalancer.invertTiltFB ? 1 : 0;
            trayBalancer.invertTiltFB = PlayerPrefs.GetInt("SavedInvertFB", defaultInvertFB) == 1;

            // Load Audio Settings
            float savedMaster = PlayerPrefs.GetFloat("SavedMasterVol", 1f);
            float savedMusic = PlayerPrefs.GetFloat("SavedMusicVol", 1f);
            float savedSFX = PlayerPrefs.GetFloat("SavedSFXVol", 1f);

            // Sync UI
            sensitivitySlider.value = trayBalancer.tiltSensitivity;
            masterSlider.value = savedMaster;
            musicSlider.value = savedMusic;
            sfxSlider.value = savedSFX;

            if (angleSliderLR != null) angleSliderLR.value = trayBalancer.maxTiltAngleLR;
            if (angleSliderFB != null) angleSliderFB.value = trayBalancer.maxTiltAngleFB;
            
            if (invertToggleLR != null) invertToggleLR.isOn = trayBalancer.invertTiltLR;
            if (invertToggleFB != null) invertToggleFB.isOn = trayBalancer.invertTiltFB;

            StartCoroutine(InitializeAudioMixer(savedMaster, savedMusic, savedSFX));
        }

        Time.timeScale = 1f; 
        pauseMenuPanel.SetActive(false);
        gameWinPanel.SetActive(false);
    }

    public IEnumerator PlayCountdownUI()
    {
        pauseButton.SetActive(false);
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.text = "START!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        // Turn buttons back on
        pauseButton.SetActive(true);
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

    public void ShowGameOver(string reason)
    {
        pauseButton.SetActive(false);
        
        gameOverPanel.SetActive(true);
    }

    public void ShowGameWin(int score, string rank)
    {
        pauseButton.SetActive(false);
        
        if (gameWinPanel != null)
        {
            gameWinPanel.SetActive(true);
            Debug.Log($"Score: {score}");
            Debug.Log($"Rank: {rank}");
        }
    }

    public void UpdateTimerDisplay(float timeInSeconds)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeInSeconds <= 10f)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    public void SetInfiniteTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = "∞";
            timerText.color = Color.white;
        }
    }

    public void OnSensitivityChanged(float sliderValue)
    {
        if (trayBalancer != null)
        {
            trayBalancer.tiltSensitivity = sliderValue;

            PlayerPrefs.SetFloat("SavedSensitivity", sliderValue);
            PlayerPrefs.Save();
        }
    }

    public void OnAngleLRChanged(float sliderValue)
    {
        if (trayBalancer != null)
        {
            trayBalancer.maxTiltAngleLR = sliderValue;
            PlayerPrefs.SetFloat("SavedAngleLR", sliderValue);
            PlayerPrefs.Save();
        }
    }

    public void OnAngleFBChanged(float sliderValue)
    {
        if (trayBalancer != null)
        {
            trayBalancer.maxTiltAngleFB = sliderValue;
            PlayerPrefs.SetFloat("SavedAngleFB", sliderValue);
            PlayerPrefs.Save();
        }
    }

    public void OnInvertLRToggled(bool isToggled)
    {
        if (trayBalancer != null)
        {
            trayBalancer.invertTiltLR = isToggled;
            PlayerPrefs.SetInt("SavedInvertLR", isToggled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void OnInvertFBToggled(bool isToggled)
    {
        if (trayBalancer != null)
        {
            trayBalancer.invertTiltFB = isToggled;
            PlayerPrefs.SetInt("SavedInvertFB", isToggled ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(mainMenuBuildIndex);
    }

    public void PlayNextLevel()
    {
        Time.timeScale = 1f; 
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene available in Build Settings");
        }
    }
    private IEnumerator InitializeAudioMixer(float master, float music, float sfx)
    {
        yield return null;
        
        SetMixerVolume("MasterVol", master);
        SetMixerVolume("MusicVol", music);
        SetMixerVolume("SFXVol", sfx);
    }

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        if (mainMixer == null) return;
        
        // Convert linear slider (0 to 1) to Logarithmic Decibels (-80dB to 0dB)
        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f); 
        float decibelValue = Mathf.Log10(clampedValue) * 20f;
        
        mainMixer.SetFloat(parameterName, decibelValue);
    }
    
    public void OnMasterVolumeChanged(float value)
    {
        SetMixerVolume("MasterVol", value);
        PlayerPrefs.SetFloat("SavedMasterVol", value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        SetMixerVolume("MusicVol", value);
        PlayerPrefs.SetFloat("SavedMusicVol", value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        SetMixerVolume("SFXVol", value);
        PlayerPrefs.SetFloat("SavedSFXVol", value);
    }



}
