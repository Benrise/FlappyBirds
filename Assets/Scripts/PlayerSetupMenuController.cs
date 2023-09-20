using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    private int PlayerIndex;

    [SerializeField]
    private Button[] addPlayerButtons;

    [SerializeField]
    private Sprite[] playerBirdSprites;

    [SerializeField]
    private RuntimeAnimatorController[] playerBirdSpriteAnimations;

    [SerializeField]
    private int playersMaxHP = 3;

    private void Awake()
    {
        for (int i = 0; i < addPlayerButtons.Length; i++)
        {
            int index = i;
            Button addButton = addPlayerButtons[i];
            addButton.onClick.AddListener(() => TogglePlayer(index, playerBirdSprites[index], playerBirdSpriteAnimations[index], addButton));
        }
    }

    private void TogglePlayer(int playerIndex, Sprite playerBirdSprite, RuntimeAnimatorController playerBirdAnimation, Button button)
    {
        PlayerConfigurationManager configManager = PlayerConfigurationManager.Instance;

        if (configManager != null)
        {
            PlayerConfiguration playerConfig = configManager.GetPlayerConfigByIndex(playerIndex);

            if (playerConfig != null)
            {
                configManager.RemovePlayerConfiguration(playerIndex);
                button.GetComponent<Image>().color = Color.white;

            }
            else
            {
                configManager.AddPlayerConfiguration(new PlayerConfiguration(playerIndex, playerBirdSprite, playerBirdAnimation, playersMaxHP));
                button.GetComponent<Image>().color = Color.grey;

            }

            configManager.UpdateStartButtonInteractability();
        }
    }
}
