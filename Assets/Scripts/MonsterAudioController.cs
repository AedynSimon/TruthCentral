using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MonsterAudioController : MonoBehaviour
{
    public Transform player;             // Drag your player Transform here
    public float maxVolume = 1f;         // Max loudness
    public float maxDistance = 20f;      // Beyond this distance, volume = 0

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // Normalize distance (0 = close, 1 = far)
        float volume = Mathf.Clamp01(1 - (distance / maxDistance));

        // Apply volume curve for smoother transitions (optional)
        volume = Mathf.Pow(volume, 2); // make it increase faster near the monster

        audioSource.volume = volume;
    }
}