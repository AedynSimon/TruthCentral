using System.Collections;
using UnityEngine;
using TMPro;

public class PickupUI : MonoBehaviour
{
    public static PickupUI instance;
    public TextMeshProUGUI messageText;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        instance = this;
        messageText.gameObject.SetActive(false);
        messageText.alpha = 0;
    }

    public void ShowMessage(string text)
    {
        StopAllCoroutines();
        StartCoroutine(FadeMessage(text));
    }

    IEnumerator FadeMessage(string text)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            messageText.alpha = t / fadeDuration;
            yield return null;
        }
        messageText.alpha = 1;

        // Wait before fading out
        yield return new WaitForSeconds(2f);

        // Fade out
        for (float t = fadeDuration; t > 0; t -= Time.deltaTime)
        {
            messageText.alpha = t / fadeDuration;
            yield return null;
        }
        messageText.alpha = 0;
        messageText.gameObject.SetActive(false);
    }
}