using UnityEngine;
using System.Collections;

public enum GameState { Countdown, Playing, GameOver, GameWin }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game State")]
    public GameState currentState;
    public UIManager uiManager;

    [Header("Timer Settings")]
    public bool isInfiniteTime = false;
    public float levelTimeInSeconds = 60f;

    [Header("Audio Settings")]
    public SoundData TempBGM;

    private float currentTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (AudioManager.Instance != null && TempBGM != null)
        {
            AudioManager.Instance.PlayMusic(TempBGM);
        }
        
        StartCoroutine(StartLevelRoutine());
    }

    private IEnumerator StartLevelRoutine()
    {
        currentState = GameState.Countdown;

        if (uiManager != null)
        {
            if (isInfiniteTime) uiManager.SetInfiniteTimerDisplay();
            else uiManager.UpdateTimerDisplay(levelTimeInSeconds);
        }
        
        yield return StartCoroutine(uiManager.PlayCountdownUI());

        currentTime = levelTimeInSeconds;
        currentState = GameState.Playing;
    }

    void Update()
    {
        if (currentState == GameState.Playing && !isInfiniteTime)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                TriggerGameOver("Time's Up!");
            }

            if (uiManager != null)
            {
                uiManager.UpdateTimerDisplay(currentTime);
            }
        }
    }

    public void TriggerGameOver(string reason)
    {
        // Avoid trigger Game Over twice
        if (currentState == GameState.GameOver) return;

        currentState = GameState.GameOver;
        Debug.LogWarning($"Game Over: {reason}");

        Time.timeScale = 0f;

        if (uiManager != null)
        {
            uiManager.ShowGameOver(reason);
        }
    }

    public void TriggerGameWin()
    {
        if (currentState == GameState.GameOver || currentState == GameState.GameWin) return;

        currentState = GameState.GameWin;
        Debug.Log("You win! Level Complete.");

        Time.timeScale = 0f;

        if (uiManager != null)
        {
            uiManager.ShowGameWin();
        }
    }
}
