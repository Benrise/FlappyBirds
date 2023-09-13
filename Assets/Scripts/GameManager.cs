using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    GameObject playerPrefab;
    
    private void Start(){
        var p1 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "KB1", pairWithDevices: Keyboard.current);
        var p2 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "KB2", pairWithDevices: Keyboard.current);
        var p3 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "KB3", pairWithDevices: Keyboard.current);
        var p4 = PlayerInput.Instantiate(playerPrefab,
            controlScheme: "KB4");
    }    

    private void Awake(){
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        if (playerInputManager != null){
            playerPrefab = playerInputManager.playerPrefab;
        }
    }
}
