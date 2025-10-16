using UnityEngine;

public class WinZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance.allFilesCollected)
            {
                Debug.Log("You Win! All files delivered!");
                if (PickupUI.instance != null)
                    PickupUI.instance.ShowMessage("You Win! All files delivered!");

                // Optional: trigger victory sequence or load win scene
                // Example: SceneManager.LoadScene("WinScene");
            }
            else
            {
                Debug.Log("You still need to collect all the files!");
                if (PickupUI.instance != null)
                    PickupUI.instance.ShowMessage("Get all 5 Epstein files!");
            }
        }
    }
}