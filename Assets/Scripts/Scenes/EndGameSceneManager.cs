using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EndGameSceneManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _endGameMenu;

    public void RestartGameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuScene()
    {
        Destroy(PlayerConfigurationManager.Instance);
        SceneManager.LoadScene("PlayerSetup");
    }

    public void ActivateEndGameMenu()
    {
        _endGameMenu.SetActive(true);
    }
}