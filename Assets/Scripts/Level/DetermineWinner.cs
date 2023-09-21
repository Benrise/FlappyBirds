using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetermineWinner : MonoBehaviour
{


    public static DetermineWinner Instance { get; private set; }

    [SerializeField]
    public Image winnerSprite;

    [SerializeField]
    public Sprite deadBirdSprite;

    [SerializeField]
    public TMP_Text winnerIndex;

    [SerializeField]
    public GameObject aboutWinnerTextContainer;

    [SerializeField]
    public GameObject noWinnerTextContainer;

    [SerializeField]
    public GameObject scoreContainer;

    [SerializeField]
    public TMP_Text score;


        
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public PlayerConfiguration GetWinner()
    {
        PlayerConfiguration winner = null;
        int maxPoints = -1;
    
        foreach (var playerConfig in PlayerConfigurationManager.Instance.playerConfigs)
        {
            if (playerConfig.Points > maxPoints)
            {
                maxPoints = playerConfig.Points;
                winner = playerConfig;
            }
        }

        if (maxPoints == 0 && PlayerConfigurationManager.Instance.playerConfigs.Count != 1){
            noWinnerTextContainer.SetActive(true);
            winner.PlayerBirdSprite = deadBirdSprite;
            scoreContainer.SetActive(false);
        }
        else if (PlayerConfigurationManager.Instance.playerConfigs.Count != 1)
        {
            aboutWinnerTextContainer.SetActive(true);
        }

        

        return winner;
    }
}
