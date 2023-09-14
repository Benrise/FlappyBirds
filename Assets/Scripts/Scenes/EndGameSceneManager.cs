using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameSceneManager : MonoBehaviour
{
    public void RestartGameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuScene()
    {
        Destroy(PlayerConfigurationManager.Instance);
        SceneManager.LoadScene("PlayerSetup");
    }
}