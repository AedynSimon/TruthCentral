using UnityEngine;
using UnityEngine.InputSystem;   // 👈 Required for Keyboard.current

public class Hideable : MonoBehaviour
{
    public Transform hidePoint;
    private bool playerNearby = false;
    private PlayerHide playerHide;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            playerHide = other.GetComponent<PlayerHide>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            playerHide = null;
        }
    }

    void Update()
    {
        // new-Input-System key check:
        if (playerNearby && playerHide != null && Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            playerHide.ToggleHide(this);
        }
    }
}