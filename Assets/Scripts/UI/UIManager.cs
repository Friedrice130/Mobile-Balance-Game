using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
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
    public SoundData countdownBeepSound;
    public SoundData countdownStartSound;

    [Header("Screens & Text")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;
    public GameObject gameWinPanel;

    [Header("Game Win UI")]
    public TextMeshProUGUI scoreText;
    public GameObject rankSPlus;
    public GameObject rankA;
    public GameObject rankB;

    [Header("Game References")]
    public TrayBalancer trayBalancer;
    public int mainMenuBuildIndex = 0;

    void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameWinPanel != null) gameWinPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (rankSPlus != null) rankSPlus.SetActive(false);
        if (rankA != null) rankA.SetActive(false);
        if (rankB != null) rankB.SetActive(false);

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
            if (sensitivitySlider != null) sensitivitySlider.value = trayBalancer.tiltSensitivity;
            if (masterSlider != null) masterSlider.value = savedMaster;
            if (musicSlider != null) musicSlider.value = savedMusic;
            if (sfxSlider != null) sfxSlider.value = savedSFX;

            if (angleSliderLR != null) angleSliderLR.value = trayBalancer.maxTiltAngleLR;
            if (angleSliderFB != null) angleSliderFB.value = trayBalancer.maxTiltAngleFB;

            if (invertToggleLR != null) invertToggleLR.isOn = trayBalancer.invertTiltLR;
            if (invertToggleFB != null) invertToggleFB.isOn = trayBalancer.invertTiltFB;

            StartCoroutine(InitializeAudioMixer(savedMaster, savedMusic, savedSFX));
        }
    }

    public IEnumerator PlayCountdownUI()
    {
        pauseButton.SetActive(false);
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        if (AudioManager.Instance != null && countdownBeepSound != null) 
            AudioManager.Instance.Play2D(countdownBeepSound);
        yield return new WaitForSeconds(1f);

        if (countdownText == null) yield break;

        countdownText.text = "2";
        if (AudioManager.Instance != null && countdownBeepSound != null) 
            AudioManager.Instance.Play2D(countdownBeepSound);
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        if (AudioManager.Instance != null && countdownBeepSound != null) 
            AudioManager.Instance.Play2D(countdownBeepSound);
        yield return new WaitForSeconds(1f);

        countdownText.text = "START!";
        if (AudioManager.Instance != null && countdownStartSound != null) 
            AudioManager.Instance.Play2D(countdownStartSound);
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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllGameplayAudio();
        }

        pauseButton.SetActive(false);

        gameOverPanel.SetActive(true);
    }

    public void ShowGameWin(int score, Rank rank)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllGameplayAudio();
        }
        
        pauseButton.SetActive(false);

        if (gameWinPanel != null)
        {
            gameWinPanel.SetActive(true);
            rankSPlus.SetActive(false);
            rankA.SetActive(false);
            rankB.SetActive(false);
            scoreText.text = "";

            StartCoroutine(CinematicWinRoutine(score, rank));
            Debug.Log($"Score: {score} | Rank: {rank}");
        }
    }

    private IEnumerator CinematicWinRoutine(int finalScore, Rank rank)
    {
        yield return new WaitForSecondsRealtime(0.4f);

        float rollDuration = 2.2f;
        float elapsed = 0f;
        scoreText.text = "Score: 0";

        Transform scoreTransform = scoreText.transform;
        Vector3 nativeLocalPos = scoreTransform.localPosition; 

        // --- 1. SCORE ROLL ---
        while (elapsed < rollDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / rollDuration;

            float customEase = progress < 0.5f
                ? 4f * progress * progress * progress
                : 1f - Mathf.Pow(-2f * progress + 2f, 3f) / 2f;

            scoreText.text = $"Score: {Mathf.RoundToInt(Mathf.Lerp(0, finalScore, customEase))}";

            if (progress > 0.2f && progress < 0.8f)
            {
                float jitter = UnityEngine.Random.Range(-3f, 3f);
                scoreTransform.localPosition = nativeLocalPos + new Vector3(jitter, jitter, 0);
            }
            else
            {
                scoreTransform.localPosition = nativeLocalPos;
            }
            yield return null;
        }
        scoreTransform.localPosition = nativeLocalPos;
        scoreText.text = $"Score: {finalScore}";

        // --- 2. IMPACT POP ---
        float bounceElapsed = 0f;
        float popDuration = 0.08f;
        while (bounceElapsed < popDuration)
        {
            bounceElapsed += Time.unscaledDeltaTime;
            scoreTransform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.5f, 1.5f, 1f), bounceElapsed / popDuration);
            yield return null;
        }

        bounceElapsed = 0f;
        float settleDuration = 0.27f;
        while (bounceElapsed < settleDuration)
        {
            bounceElapsed += Time.unscaledDeltaTime;
            float t = bounceElapsed / settleDuration;
            float dynamicScale = 1f + (0.50f * Mathf.Cos(t * Mathf.PI * 2.5f) * Mathf.Exp(-t * 4f));
            scoreTransform.localScale = new Vector3(dynamicScale, dynamicScale, 1f);
            yield return null;
        }
        scoreTransform.localScale = Vector3.one;

        yield return new WaitForSecondsRealtime(0.4f);

        // --- 3. RANK SLAM ---
        GameObject targetRankObj = rank switch
        {
            Rank.SPlus => rankSPlus,
            Rank.A => rankA,
            Rank.B => rankB,
            _ => null
        };

        if (targetRankObj != null)
        {
            targetRankObj.SetActive(true);
            Transform rankTransform = targetRankObj.transform;
            Vector3 originalRankPos = rankTransform.localPosition;

            float slamDuration = 0.15f;
            float slamElapsed = 0f;

            while (slamElapsed < slamDuration)
            {
                slamElapsed += Time.unscaledDeltaTime;
                float t = slamElapsed / slamDuration;
                rankTransform.localScale = Vector3.Lerp(new Vector3(10f, 10f, 10f), Vector3.one, t * t * t);
                yield return null;
            }

            // High-Impact Impact Frame Freeze
            rankTransform.localScale = Vector3.one;
            yield return new WaitForSecondsRealtime(0.04f);

            // Aftershock structural echo tremor (Slammed look feeling)
            float earthquakeElapsed = 0f;
            float earthquakeDuration = 0.25f;

            while (earthquakeElapsed < earthquakeDuration)
            {
                earthquakeElapsed += Time.unscaledDeltaTime;
                float t = earthquakeElapsed / earthquakeDuration;

                // Rapidly dampening structural camera-shake style vibration
                float shakeIntensity = Mathf.Sin(t * Mathf.PI * 8f) * Mathf.Exp(-t * 5f) * 15f;
                rankTransform.localPosition = originalRankPos + new Vector3(UnityEngine.Random.Range(-shakeIntensity, shakeIntensity), UnityEngine.Random.Range(-shakeIntensity, shakeIntensity), 0);

                // Mild structural compression squish matching impact speed
                float dynamicSquish = Mathf.Sin(t * Mathf.PI * 2f) * Mathf.Exp(-t * 4f) * 0.25f;
                rankTransform.localScale = new Vector3(1f + dynamicSquish, 1f - dynamicSquish, 1f);

                yield return null;
            }
            rankTransform.localPosition = originalRankPos;
            rankTransform.localScale = Vector3.one;
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

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.LoadChapterWithFade(currentSceneIndex, "");
        }
        else
        {
            SceneManager.LoadScene(currentSceneIndex);
        }
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

    public void SetPauseButton(bool visible)
    {
        pauseButton.SetActive(visible);
    }
    public void SetTimerVisible(bool visible)
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(visible);
        }
    }
}
