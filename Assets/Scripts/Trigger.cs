using UnityEngine;

public class FloorTrigger : MonoBehaviour
{
    public MonsterAI monster; // Assign in Inspector
    public Transform player;  // Assign player in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (monster != null && player != null)
            {
                monster.GoToPlayerPosition(player.position);
                Debug.Log("<color=yellow>[Trigger]</color> Player entered floor trigger — monster moving toward new player position.");
            }
        }
    }
}