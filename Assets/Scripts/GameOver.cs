using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverCanvas;

    public void TriggerGameOver()
    {
        gameOverCanvas.SetActive(true);
        StartCoroutine(ReturnToStartMenu());
    }

    IEnumerator ReturnToStartMenu()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("StartMenu");
    }
}
