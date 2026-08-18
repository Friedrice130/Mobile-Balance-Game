using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TypewriterUI : MonoBehaviour
{
    private TextMeshProUGUI tmpText;

    [Header("Typewriter Settings")]
    public float typingSpeed = 0.05f; 
    public float delayBeforeFade = 2f; 
    public float fadeDuration = 1f;

    private Coroutine currentRoutine;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        ClearText();
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(TypeAndFadeRoutine(message));
    }

    private IEnumerator TypeAndFadeRoutine(string message)
    {
        Color c = tmpText.color;
        c.a = 1f;
        tmpText.color = c;
        tmpText.text = "";

        foreach (char letter in message.ToCharArray())
        {
            tmpText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(delayBeforeFade);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            tmpText.color = c;
            yield return null;
        }
        ClearText();
    }
    
    private void ClearText()
    {
        tmpText.text = "";
        Color c = tmpText.color;
        c.a = 0f;
        tmpText.color = c;
    }
}
