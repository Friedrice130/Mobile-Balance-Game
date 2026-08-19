using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    public bool isFading = false;

    [Header("UI References")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private TextMeshProUGUI chapterText;

    [Header("Fade Settings")]
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float textDisplayDuration = 2.0f;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else 
        {
            Destroy(transform.root.gameObject);
            return;
        }

        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f); 
            fadeImage.raycastTarget = false; 
        }

        if (chapterText != null)
        {
            chapterText.text = "";
            chapterText.color = new Color(chapterText.color.r, chapterText.color.g, chapterText.color.b, 0f);
        }
    }

    public IEnumerator FadeOutIn(System.Action midFadeAction)
    {
        isFading = true;

        fadeImage.raycastTarget = true;
        yield return StartCoroutine(Fade(1f, fadeDuration));
        
        midFadeAction?.Invoke();
        
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(Fade(0f, fadeDuration));
        fadeImage.raycastTarget = false;

        isFading = false;
    }

    public void LoadChapterWithFade(int sceneIndex, string chapterName)
    {
        StartCoroutine(ChapterTransitionRoutine(sceneIndex, chapterName));
    }

    private IEnumerator ChapterTransitionRoutine(int sceneIndex, string chapterName)
    {
        isFading = true;

        fadeImage.raycastTarget = true;

        yield return StartCoroutine(Fade(1f, fadeDuration));

        if (chapterText != null && !string.IsNullOrEmpty(chapterName))
        {
            chapterText.text = chapterName;
            yield return StartCoroutine(FadeText(1f, 0.5f));
            yield return new WaitForSeconds(textDisplayDuration);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (chapterText != null && !string.IsNullOrEmpty(chapterName))
        {
            StartCoroutine(FadeText(0f, fadeDuration));
        }

        yield return StartCoroutine(Fade(0f, fadeDuration));
        
        if (chapterText != null) chapterText.text = "";
        fadeImage.raycastTarget = false;
        isFading = false;
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, targetAlpha);
    }

    private IEnumerator FadeText(float targetAlpha, float duration)
    {
        float startAlpha = chapterText.color.a;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            chapterText.color = new Color(chapterText.color.r, chapterText.color.g, chapterText.color.b, alpha);
            yield return null;
        }
        chapterText.color = new Color(chapterText.color.r, chapterText.color.g, chapterText.color.b, targetAlpha);
    }
}
