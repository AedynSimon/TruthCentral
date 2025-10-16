using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("File Collection")]
    public int totalFiles = 5;
    private int filesCollected = 0;

    public bool allFilesCollected => filesCollected >= totalFiles;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectFile(FilePickup file)
    {
        filesCollected++;
        Debug.Log($"Collected {filesCollected}/{totalFiles} files");

        // Optional UI feedback
        if (PickupUI.instance != null)
        {
            PickupUI.instance.ShowMessage($"File Collected ({filesCollected}/{totalFiles})");
        }

        // Optional win logic if you want instant win on last file
        if (allFilesCollected)
        {
            Debug.Log("All files collected! Return to the drop zone!");
            if (PickupUI.instance != null)
                PickupUI.instance.ShowMessage("All files collected! Return to the front doors!");
        }
    }
}