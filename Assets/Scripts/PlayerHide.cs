using UnityEngine;
using StarterAssets;

public class PlayerHide : MonoBehaviour
{
    private bool isHidden = false;
    private Hideable currentHideable;
    private FirstPersonController controller;
    private StarterAssetsInputs inputs;

    void Start()
    {
        controller = GetComponent<FirstPersonController>();
        inputs = GetComponent<StarterAssetsInputs>();
    }

    public void ToggleHide(Hideable hideable)
    {
        if (!isHidden)
            EnterHide(hideable);
        else
            ExitHide();
    }

    void EnterHide(Hideable hideable)
    {
        currentHideable = hideable;
        isHidden = true;

        // Move player to hide point
        transform.position = hideable.hidePoint.position;

        // Stop player motion immediately
        if (inputs != null)
        {
            inputs.move = Vector2.zero;
            inputs.sprint = false;
            inputs.jump = false;
        }

        // Disable controller's movement, keep camera active
        if (controller != null)
        {
            controller.enabled = true; // keep enabled for camera rotation
            controller.Grounded = true;
        }

        // Lock movement
        GetComponent<CharacterController>().enabled = false;

        // Hide visuals (optional)
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;

        Debug.Log("Player is now hidden");
    }

    void ExitHide()
    {
        isHidden = false;

        // Re-enable player collider & movement
        GetComponent<CharacterController>().enabled = true;

        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = true;

        currentHideable = null;

        Debug.Log("Player exited hiding spot");
    }

    public bool IsHidden() => isHidden;
}