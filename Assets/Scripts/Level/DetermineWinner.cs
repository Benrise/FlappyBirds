using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DetermineWinner : MonoBehaviour
{


    public static DetermineWinner Instance { get; private set; }

    [SerializeField]
    public TMP_Text winnerIndex;

    [SerializeField]
    public Image winnerSprite;

    [SerializeField]
    public Sprite defaultSprite;

    [SerializeField]
    public GameObject WinnerContainer;

    [SerializeField]
    public GameObject NoWinnerContainer;
        
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

        if (maxPoints == 0){
           NoWinnerContainer.SetActive(true);
           WinnerContainer.SetActive(false);
        }
        
        return winner;
    }
}
