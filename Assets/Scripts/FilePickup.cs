using UnityEngine;
using UnityEngine.InputSystem; // works with new input system

public class FilePickup : MonoBehaviour
{
    [Header("File Settings")]
    public string fileName = "File";
    public float pickupRange = 3f;

    [Header("Optional Effects")]
    public GameObject pickupEffect;     // Assign a particle prefab if you have one
    public AudioClip pickupSound;       // Assign a pickup sound if you want one

    private Transform player;
    private bool inRange;
    private AudioSource audioSource;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        inRange = distance <= pickupRange;

        // Using new input system (Keyboard.current)
        if (inRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            PickUp();
        }
    }

    void PickUp()
    {
        Debug.Log($"Picked up {fileName}");
        GameManager.instance.CollectFile(this);

        // Play sound (optional)
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // Play particle effect
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        // Hide mesh and collider so it looks gone
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        if (mesh != null)
            mesh.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Destroy after short delay (gives time for sound/particles)
        Destroy(gameObject, 1f);
    }

    // Optional: visualize pickup range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}