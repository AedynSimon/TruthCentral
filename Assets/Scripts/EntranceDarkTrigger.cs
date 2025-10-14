using UnityEngine;

public class FogZone : MonoBehaviour
{
    public Color fogColorInside = new Color(0.02f, 0.02f, 0.02f); // almost black
    public float fogDensityInside = 0.08f; // thicker fog
    public Color fogColorOutside = new Color(0.1f, 0.1f, 0.15f); // bluish night fog
    public float fogDensityOutside = 0.04f; // lighter fog
    public float transitionSpeed = 1f;

    private bool inZone = false;
    private float currentLerpTime = 0f;

    private void Update()
    {
        // Smoothly transition fog values over time
        currentLerpTime += Time.deltaTime * transitionSpeed;
        float t = Mathf.Clamp01(currentLerpTime);

        if (inZone)
        {
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColorInside, t);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensityInside, t);
        }
        else
        {
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColorOutside, t);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensityOutside, t);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inZone = true;
            currentLerpTime = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inZone = false;
            currentLerpTime = 0f;
        }
    }
}