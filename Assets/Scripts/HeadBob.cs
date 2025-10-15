using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;  // assign your player capsule here

    [Header("Bob Settings")]
    public float bobFrequency = 2.5f;   // base speed of the bob
    public float bobAmplitude = 0.05f;  // base height of the bob
    public float sideAmplitude = 0.05f; // how much it moves side to side
    public float smooth = 10f;          // how quickly it transitions to the bob position

    private Vector3 startPos;
    private float timer;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // only bob when moving on ground
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            // scale bob speed & size by movement speed
            float speed = controller.velocity.magnitude;
            timer += Time.deltaTime * bobFrequency * (speed / 3f);

            // calculate up/down (sin) and side/side (cos)
            float newY = Mathf.Sin(timer) * bobAmplitude * (speed / 3f);
            float newX = Mathf.Cos(timer / 2f) * sideAmplitude * (speed / 3f);

            // smoothly move camera to new position
            Vector3 targetPos = startPos + new Vector3(newX, newY, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smooth);
        }
        else
        {
            // reset to neutral when stopped or airborne
            timer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * smooth);
        }
    }
}