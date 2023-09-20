using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerConfigurationManager : MonoBehaviour
{
    public List<PlayerConfiguration> playerConfigs;

    public static PlayerConfigurationManager Instance { get; private set; }

    [SerializeField]
    private Button _startButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance); // Экземпляр класса не будет уничтожен при загрузке новой сцены
            playerConfigs = new List<PlayerConfiguration>();
        }

        if (_startButton != null && _startButton.GetComponent<Button>() != null)
        {
            _startButton.onClick.AddListener(LoadGameScene);
        }

        UpdateStartButtonInteractability(); 
    }

    public void UpdateStartButtonInteractability()
    {
        _startButton.interactable = playerConfigs.Any();
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void HandlePlayerJoin(PlayerInput playerInput)
    {
        Debug.Log("Player " + playerInput.playerIndex + " joined, using " + playerInput.currentControlScheme);
    }

    public void AddPlayerConfiguration(PlayerConfiguration playerConfig)
    {
        playerConfigs.Add(playerConfig);
    }

    public void RemovePlayerConfiguration(int playerIndex)
    {
        PlayerConfiguration playerToRemove = playerConfigs.FirstOrDefault(config => config.PlayerIndex == playerIndex);
        if (playerToRemove != null)
        {
            playerConfigs.Remove(playerToRemove);
        }
    }

    public PlayerConfiguration GetPlayerConfigByIndex(int playerIndex)
    {
        return playerConfigs.FirstOrDefault(config => config.PlayerIndex == playerIndex);
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(int idx, Sprite playerBirdSprite, RuntimeAnimatorController playerBirdSpriteAnimation, int maxLives = 3, int speedBuffs = 3)
    {
        PlayerIndex = idx;
        PlayerBirdSprite = playerBirdSprite;
        PlayerBirdSpriteAnimation = playerBirdSpriteAnimation;
        isAlive = true;
        MaxLives = maxLives;
        Lives = maxLives;
        SpeedBuffs = speedBuffs;
    }

    public int Lives { get; set; }

    public int Points { get; set; }

    public int SpeedBuffs { get; set; }

    public int MaxLives { get; set; }

    public bool isAlive { get; set; }

    public int PlayerIndex { get; set; }

    public Sprite PlayerBirdSprite { get; set; }

    public RuntimeAnimatorController PlayerBirdSpriteAnimation { get; set; }
}
