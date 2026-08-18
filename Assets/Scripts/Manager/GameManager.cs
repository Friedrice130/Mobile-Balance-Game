using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum GameState { Countdown, Playing, GameOver, GameWin }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level Settings")]
    public bool useTutorial = false;
    public bool useCountdown = true;

    [Header("Game State")]
    public GameState currentState;
    public UIManager uiManager;

    [Header("Timer Settings")]
    public bool isInfiniteTime = false;
    public float levelTimeInSeconds = 60f;

    [Header("Audio Settings")]
    public SoundData TempBGM;

    private float currentTime;
    public float CurrentTime => currentTime;
    public float MaxTime => levelTimeInSeconds;

    /*void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }*/

    void Awake()
    {
        Debug.Log("GameManager Awake");

        if (Instance != null && Instance != this)
        {
            Debug.Log("Duplicate GameManager destroyed");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        Debug.Log("GameManager Destroyed");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopAllCoroutines();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
            return;
        }
        
        Time.timeScale = 1f;

        currentState = GameState.Countdown;
        currentTime = levelTimeInSeconds;

        uiManager = FindAnyObjectByType<UIManager>();

        if (AudioManager.Instance != null)
        {
            if (TempBGM != null)
            {
                AudioManager.Instance.PlayMusic(TempBGM);
            }
            else
            {
                AudioManager.Instance.StopAllGameplayAudio();
            }
        }

        StartCoroutine(StartLevelRoutine());
    }

    void Start()
    {
    }

    private IEnumerator StartLevelRoutine()
    {
        if (ScreenFader.Instance != null)
        {
            while (ScreenFader.Instance.isFading)
            {
                yield return null;
            }
        }

        Debug.Log("StartLevelRoutine");
        currentState = GameState.Countdown;

        if (uiManager != null)
        {
            if (isInfiniteTime) uiManager.SetInfiniteTimerDisplay();
            else uiManager.UpdateTimerDisplay(levelTimeInSeconds);
        }

        if (useTutorial)
        {
            FindAnyObjectByType<TutorialManager>().BeginTutorial();
            yield break; // Tutorial will start gameplay later
        }

        if (useCountdown)
        {
            yield return StartCoroutine(uiManager.PlayCountdownUI());
        }

        BeginGameplay();

    }

    public void BeginGameplay()
    {
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
            int displayScore;
            Rank rank;

            if (useTutorial)
            {
                // Tutorial level: force high score
                displayScore = 999;
                rank = Rank.SPlus;
            }
            else
            {
                displayScore = ScoreManager.Instance.DisplayScore;
                rank = ScoreManager.Instance.FinalRank;
            }

            uiManager.ShowGameWin(displayScore, rank);
        }
    }
}
