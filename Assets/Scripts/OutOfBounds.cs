using UnityEngine;
using UnityEngine.SceneManagement;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the map! Game over");
            FindFirstObjectByType<GameOver>().TriggerGameOver();
        }
    }
}
