using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScannerUnlockUI : MonoBehaviour
{
    public UnityEngine.UI.Text unlockText;
    public float fadeDuration = 1f;
    public float displayDuration = 2f;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f; // Initially hidden
        gameObject.SetActive(false);
    }

    public void ShowUnlockUI()
    {
        gameObject.SetActive(true);
        StopAllCoroutines(); // Stop any running coroutines
        StartCoroutine(FadeInAndOut());
    }

    IEnumerator FadeInAndOut()
    {
        // Fade In
        float timeElapsed = 0;
        while (timeElapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1; // Ensure fully visible

        // Display Duration
        yield return new WaitForSeconds(displayDuration);

        // Fade Out
        timeElapsed = 0;
        while (timeElapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0; // Ensure fully invisible
        gameObject.SetActive(false);
    }
}