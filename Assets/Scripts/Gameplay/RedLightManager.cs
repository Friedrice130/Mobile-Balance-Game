using System.Collections;
using UnityEngine;

public class RedLightManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isGameActive = false;
    private bool isScanning = false;
    private bool isFailing = false;

    [Header("Settings")]
    public Transform respawnPoint;
    public float movementTolerance = 0.1f;
    public float greenLightDuration = 4f;
    public float redLightDuration = 3f;

    [Header("Doll Visuals")]
    public Transform dollPivot;
    public float turnSpeed = 0.2f;

    [Header("UI Notifications")]
    public TypewriterUI introUI;
    [TextArea] 
    public string introText = "Let's play a game.\nGreen light, you walk. Red light, you die.";

    [Header("Audio")]
    public AudioSource singingSource;
    public SoundData scanSound;
    public SoundData caughtSound;

    [Header("Cleanup")]
    public GameObject[] objectsToDisableOnWin;

    private GameObject player;
    private Rigidbody playerRb;
    private Vector3 lastPlayerPos;

    private Coroutine gameCycleCoroutine;
    private Coroutine dollRotateCoroutine;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        if (!isGameActive || isFailing || playerRb == null) return;

        if (isScanning)
        {
            float distanceMoved = Vector3.Distance(playerRb.position, lastPlayerPos);
            float speed = distanceMoved / Time.deltaTime;

            if (speed > movementTolerance)
            {
                StartCoroutine(FailSequence());
            }
        }

        lastPlayerPos = playerRb.position;
    }

    public void StartMiniGame()
    {
        if (isGameActive) return;
        
        isGameActive = true;
        isFailing = false;

        if (introUI != null && !string.IsNullOrEmpty(introText))
        {
            introUI.ShowMessage(introText);
        }

        gameCycleCoroutine = StartCoroutine(GameCycleRoutine());
    }

    public void StopMiniGame()
    {
        isGameActive = false;
        isScanning = false;

        if (gameCycleCoroutine != null) StopCoroutine(gameCycleCoroutine);
        if (dollRotateCoroutine != null) StopCoroutine(dollRotateCoroutine);

        if (singingSource != null) singingSource.Stop();
    }

    public void WinMiniGame()
    {
        if (!isGameActive) return;

        StopMiniGame();

        foreach (GameObject obj in objectsToDisableOnWin)
        {
            if (obj != null) obj.SetActive(false);
        }
    }

    private IEnumerator GameCycleRoutine()
    {
        while (isGameActive)
        {
            isScanning = false;

            if (dollPivot != null) 
            {
                if (dollRotateCoroutine != null) StopCoroutine(dollRotateCoroutine);
                dollRotateCoroutine = StartCoroutine(RotateDoll(0f));
            }
            
            if (singingSource != null)
            {
                singingSource.pitch = Random.Range(0.9f, 1.2f);
                singingSource.Play();
            }

            yield return new WaitForSeconds(greenLightDuration);
            if (singingSource != null) singingSource.Stop();

            if (AudioManager.Instance != null && scanSound != null)
            {
                AudioManager.Instance.Play2D(scanSound);
            }

            if (dollPivot != null) 
            {
                if (dollRotateCoroutine != null) StopCoroutine(dollRotateCoroutine);
                dollRotateCoroutine = StartCoroutine(RotateDoll(180f));
            }
            
            yield return new WaitForSeconds(0.2f);
            
            lastPlayerPos = playerRb.position;
            isScanning = true;

            yield return new WaitForSeconds(redLightDuration);
        }
    }

    private IEnumerator RotateDoll(float targetYAngle)
    {
        Quaternion startRot = dollPivot.localRotation;
        Quaternion endRot = Quaternion.Euler(0, targetYAngle, 0);
        float elapsed = 0f;

        while (elapsed < turnSpeed)
        {
            dollPivot.localRotation = Quaternion.Slerp(startRot, endRot, elapsed / turnSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        dollPivot.localRotation = endRot;
    }

    private IEnumerator FailSequence()
    {
        isFailing = true;
        isScanning = false;

        if (gameCycleCoroutine != null) StopCoroutine(gameCycleCoroutine);
        if (dollRotateCoroutine != null) StopCoroutine(dollRotateCoroutine);

        if (singingSource != null) singingSource.Stop();

        if (AudioManager.Instance != null && caughtSound != null)
        {
            AudioManager.Instance.Play2D(caughtSound);
        }

        yield return new WaitForSeconds(0.5f);

        if (ScreenFader.Instance != null)
        {
            yield return StartCoroutine(ScreenFader.Instance.FadeOutIn(() =>
            {
                TeleportPlayer();
                gameCycleCoroutine = StartCoroutine(GameCycleRoutine());
                isFailing = false;
            }));
        }
        else
        {
            TeleportPlayer();
            gameCycleCoroutine = StartCoroutine(GameCycleRoutine());
            isFailing = false;
        }
    }

    private void TeleportPlayer()
    {
        if (player != null && respawnPoint != null)
        {
            PlayerMovement pMovement = player.GetComponent<PlayerMovement>();
            if (pMovement != null)
            {
                pMovement.Teleport(respawnPoint.position, respawnPoint.rotation);
            }
        }
    }
}
