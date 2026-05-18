using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public GameObject pauseButton;
    public Slider sensitivitySlider;
    public Slider angleSlider;
    public Toggle invertToggle;

    public GameObject ballButton;
    public GameObject bookButton;
    public GameObject drinkButton;

    public TextMeshProUGUI countdownText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverReasonText;

    [Header("Game References")]
    public TrayBalancer trayBalancer;

    void Start()
    {
        if (trayBalancer != null)
        {
            sensitivitySlider.value = trayBalancer.tiltSensitivity;
            angleSlider.value = trayBalancer.maxTiltAngle;
            invertToggle.isOn = trayBalancer.invertTilt;
        }

        Time.timeScale = 1f; 
        pauseMenuPanel.SetActive(false);
    }

    public IEnumerator PlayCountdownUI()
    {
        // Hide pause & spawn buttons during countdown
        pauseButton.SetActive(false);
        ballButton.SetActive(false);
        bookButton.SetActive(false);
        drinkButton.SetActive(false);

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
        ballButton.SetActive(true);
        bookButton.SetActive(true);
        drinkButton.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; 
        pauseMenuPanel.SetActive(true);
        pauseButton.SetActive(false);
        ballButton.SetActive(false);
        bookButton.SetActive(false);
        drinkButton.SetActive(false);

    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; 
        pauseMenuPanel.SetActive(false);
        pauseButton.SetActive(true);
        ballButton.SetActive(true);
        bookButton.SetActive(true);
        drinkButton.SetActive(true);
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

    public void OnAngleChanged(float sliderValue)
    {
        if (trayBalancer != null)
        {
            trayBalancer.maxTiltAngle = sliderValue;
        }
    }

    public void ShowGameOver(string reason)
    {
        pauseButton.SetActive(false);
        ballButton.SetActive(false);
        bookButton.SetActive(false);
        drinkButton.SetActive(false);

        if (gameOverReasonText != null)
        {
            gameOverReasonText.text = reason;
        }
        
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
