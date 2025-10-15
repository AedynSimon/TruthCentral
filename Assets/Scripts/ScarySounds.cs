using UnityEngine;

public class RandomScarySounds : MonoBehaviour
{
    [Header("Scary Sound Settings")]
    public AudioSource audioSource;
    public AudioClip[] scaryClips;

    [Tooltip("Minimum seconds between random sounds")]
    public float minDelay = 10f;

    [Tooltip("Maximum seconds between random sounds")]
    public float maxDelay = 30f;

    private float timer;

    void Start()
    {
        ScheduleNextSound();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PlayRandomSound();
            ScheduleNextSound();
        }
    }

    void ScheduleNextSound()
    {
        timer = Random.Range(minDelay, maxDelay);
    }

    void PlayRandomSound()
    {
        if (scaryClips.Length == 0) return;
        AudioClip clip = scaryClips[Random.Range(0, scaryClips.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(clip);
    }
}