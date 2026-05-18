using UnityEngine;
using System.Collections;

public enum GameState { Countdown, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState;
    public UIManager uiManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(StartLevelRoutine());
    }

    private IEnumerator StartLevelRoutine()
    {
        currentState = GameState.Countdown;
        
        yield return StartCoroutine(uiManager.PlayCountdownUI());

        currentState = GameState.Playing;
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
}
