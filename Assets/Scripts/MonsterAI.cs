using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("Jumpscare Settings")]
    public AudioClip jumpscareSound;         // Screech or roar sound
    public GameObject jumpscareVisual;       // Optional visual overlay or prefab
    public float jumpscareDuration = 2f;     // How long before ending game or resetting
    public Camera playerCamera;              // Reference to player camera for screen effects

    private AudioSource audioSource;
    private bool jumpscareTriggered = false;
    [Header("Target")]
    public Transform player;

    [Header("Vision Settings")]
    public float sightRange = 20f;
    public float fieldOfView = 90f;
    public LayerMask obstacleMask;

    [Header("Movement Settings")]
    public float chaseSpeed = 3.5f;
    public float roamSpeed = 2f;
    public float roamRadius = 15f;
    public float stopDistance = 1.5f;
    public float rotationSpeed = 5f;
    public float searchDuration = 5f; // seconds to investigate last known pos

    private NavMeshAgent agent;
    private bool playerCaught = false;
    private bool canSeePlayer = false;
    private bool searching = false;

    private Vector3 lastKnownPosition;
    private float searchTimer;
    private float roamTimer;
    public float roamInterval = 5f;

    // For debug visualization
    private string currentState = "Idle";

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = chaseSpeed;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (player == null)
        {
            Debug.LogError("<color=red>[MonsterAI]</color> No player assigned or found with tag 'Player'!");
        }

        roamTimer = roamInterval;
        searchTimer = 0f;

        Debug.Log("<color=yellow>[MonsterAI]</color> Initialized and ready.");
    }

    void Update()
    {
        if (playerCaught || player == null) return;

        bool previouslySeeingPlayer = canSeePlayer;
        canSeePlayer = CanSeePlayer();

        if (canSeePlayer)
        {
            lastKnownPosition = player.position;
            ChasePlayer();
            CheckIfCaughtPlayer();
            searching = false;
            currentState = "Chasing Player";
        }
        else if (previouslySeeingPlayer && !canSeePlayer)
        {
            searching = true;
            searchTimer = searchDuration;
            agent.SetDestination(lastKnownPosition);
            agent.speed = roamSpeed;
            currentState = "Searching Last Known Position";
            Debug.Log("<color=orange>[MonsterAI]</color> Lost sight of player — heading to last known position " + lastKnownPosition);
        }
        else if (searching)
        {
            SearchLastKnownPosition();
        }
        else
        {
            RoamNearPlayer();
        }

        // Debug line to current path target
        Debug.DrawLine(transform.position + Vector3.up, agent.destination, Color.cyan);
    }

    private bool CanSeePlayer()
    {
        Vector3 eyePosition = transform.position + Vector3.up * 1.5f;
        Vector3 directionToPlayer = (player.position + Vector3.up * 1f) - eyePosition;
        float distanceToPlayer = directionToPlayer.magnitude;
        directionToPlayer.Normalize();

        if (distanceToPlayer > sightRange)
        {
            Debug.DrawRay(eyePosition, directionToPlayer * sightRange, Color.gray);
            return false;
        }

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfView * 0.5f)
        {
            Debug.DrawRay(eyePosition, directionToPlayer * sightRange, Color.yellow);
            return false;
        }

        if (Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, sightRange))
        {
            Debug.DrawRay(eyePosition, directionToPlayer * hit.distance, Color.magenta);
            if (hit.transform == player || hit.transform.IsChildOf(player))
            {
                Debug.Log("<color=green>[MonsterAI]</color> Vision: Player visible at distance " + distanceToPlayer.ToString("F1"));
                return true;
            }
            else
            {
                Debug.Log("<color=gray>[MonsterAI]</color> Vision blocked by: " + hit.transform.name);
            }
        }

        return false;
    }

    private void ChasePlayer()
    {
        if (agent.isStopped) agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);

        // Smooth rotation toward player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        Debug.DrawLine(transform.position, player.position, Color.red);
    }

    private void SearchLastKnownPosition()
    {
        currentState = "Searching";

        if (agent.pathPending) return;

        // Reached last known position
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;

            // Slowly rotate 360° around to "scan" the area
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

            // Check if player is seen again
            if (CanSeePlayer())
            {
                searching = false;
                agent.isStopped = false;
                currentState = "Chasing Player";
                Debug.Log("<color=green>[MonsterAI]</color> Found player while scanning!");
                return;
            }

            searchTimer -= Time.deltaTime;

            if (searchTimer <= 0f)
            {
                searching = false;
                agent.isStopped = false;
                currentState = "Roaming Near Player";
                Debug.Log("<color=cyan>[MonsterAI]</color> Finished scanning — returning to roaming.");
            }
        }
    }

    private void RoamNearPlayer()
    {
        currentState = "Roaming Near Player";
        agent.speed = roamSpeed;
        roamTimer -= Time.deltaTime;

        if (roamTimer <= 0f)
        {
            // Stay horizontally near the player but on current floor
            Vector3 center = new Vector3(player.position.x, transform.position.y, player.position.z);

            float yWiggle = 3f; // how much vertical variation allowed (in meters)
            Vector3 randomDirection = new Vector3(
                Random.Range(-roamRadius, roamRadius),
                Random.Range(-yWiggle, yWiggle),
                Random.Range(-roamRadius, roamRadius)
            );

            Vector3 targetPosition = center + randomDirection;

            if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                Debug.Log("<color=blue>[MonsterAI]</color> Roaming on same floor near player. New destination: " + hit.position);
            }

            roamTimer = roamInterval;
        }
    }

    private void CheckIfCaughtPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= stopDistance)
        {
            CatchPlayer();
        }
    }

    // Called when a loud noise happens (like player landing)
    public void HearNoise(Vector3 noisePosition)
    {
        if (playerCaught) return;

        lastKnownPosition = noisePosition;
        searching = true;
        searchTimer = searchDuration;

        agent.isStopped = false;
        agent.speed = roamSpeed;
        agent.SetDestination(noisePosition);

        currentState = "Investigating Noise";
        Debug.Log("<color=orange>[MonsterAI]</color> Heard a noise! Investigating position: " + noisePosition);
    }

    private void CatchPlayer()
    {
        if (playerCaught || jumpscareTriggered) return;
        playerCaught = true;
        jumpscareTriggered = true;
        agent.isStopped = true;

        // Face the player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        Debug.Log("<color=red>[MonsterAI]</color> Player caught! Triggering jumpscare.");

        // Trigger jumpscare logic
        StartCoroutine(TriggerJumpscare());
    }

    private System.Collections.IEnumerator TriggerJumpscare()
    {
        // Play sound if available
        if (jumpscareSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpscareSound);
        }

        // Spawn visual effect if assigned (like a full-screen image or animation)
        if (jumpscareVisual != null)
        {
            jumpscareVisual.SetActive(true);
            Debug.Log("<color=purple>[MonsterAI]</color> Jumpscare visual activated!");
            yield return new WaitForSeconds(jumpscareDuration);
            jumpscareVisual.SetActive(false);
            Debug.Log("<color=purple>[MonsterAI]</color> Jumpscare visual deactivated!");
        }

        // Optional: camera shake for immersion
        if (playerCamera != null)
        {
            StartCoroutine(CameraShake(playerCamera, 0.5f, 0.4f));
        }

        // Optional: freeze player movement
        StarterAssets.FirstPersonController playerController = player.GetComponent<StarterAssets.FirstPersonController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        yield return new WaitForSeconds(jumpscareDuration);

        // TODO: Transition to game over screen or reset
        Debug.Log("<color=red>[MonsterAI]</color> Jumpscare finished! Implement Game Over logic here.");
    }
    private System.Collections.IEnumerator CameraShake(Camera cam, float duration, float magnitude)
    {
        Vector3 originalPos = cam.transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            cam.transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.transform.localPosition = originalPos;
    }

    public void GoToPlayerPosition(Vector3 position)
    {
        if (playerCaught) return;

        agent.isStopped = false;
        agent.speed = roamSpeed;
        agent.SetDestination(position);

        lastKnownPosition = position;
        searching = true;
        searchTimer = searchDuration;
        currentState = "Moving to Player Floor";

        Debug.Log("<color=orange>[MonsterAI]</color> Moving toward player floor at position: " + position);
    }

    private void OnDrawGizmosSelected()
    {
        // Vision range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // FOV visualization
        Vector3 leftLimit = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward;
        Vector3 rightLimit = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + leftLimit * sightRange);
        Gizmos.DrawLine(transform.position, transform.position + rightLimit * sightRange);

        // Last known position
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(lastKnownPosition, 0.3f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"<color=white>{currentState}</color>");
#endif
    }
}