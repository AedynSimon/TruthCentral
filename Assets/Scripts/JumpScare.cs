using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JumpScareTriggerUI : MonoBehaviour
{
    [Header("Assign your UI Image here")]
    public Image scareImage;
    public AudioSource scareSound;
    public float flashDuration = 0.3f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(FlashImage());
        }
    }

    private IEnumerator FlashImage()
    {
        // Play scream sound
        if (scareSound != null)
            scareSound.Play();

        // Enable image
        scareImage.gameObject.SetActive(true);

        // Wait for a short flash duration
        yield return new WaitForSeconds(flashDuration);

        // Disable image
        scareImage.gameObject.SetActive(false);
    }
}