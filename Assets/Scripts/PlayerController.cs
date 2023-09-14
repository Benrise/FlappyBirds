using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float _velocity = 1.5f;

    [SerializeField]
    private float _rotationSpeed = 10f;

    [SerializeField]
    private TMP_Text _scoreText;

    [SerializeField]
    private AudioSource _pipeSound;

    [SerializeField]
    private AudioSource _wingSound;

    [SerializeField]
    private AudioSource _hitSound;

    [SerializeField]
    private Sprite _deadBirdSprite;

    [SerializeField]
    private GameObject _gameOverPanel;

    [SerializeField]
    private TMP_Text _playerNumber;

    private int _score;

    private bool _jumped = false;
    
    private Rigidbody2D _rb;

    private PlayerInput _playerInput;

    private PlayerControls _controls;

    private bool _isDead = false;

    private GameObject _endGameMenu;

    private void Awake(){
        gameObject.name = $"Player {GetComponent<PlayerInput>().playerIndex.ToString()}";
        _controls = new PlayerControls();

        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _endGameMenu = GameObject.Find("EndGameMenu");
        _scoreText.text = _score.ToString();
        _playerNumber.text = _playerInput.playerIndex.ToString();
    }

    private void Start(){
        _endGameMenu.SetActive(false);
    }

    private void Update(){
        if (_jumped){
            _rb.velocity = Vector2.up * _velocity;
            _wingSound.Play();
        }

    }

    public void OnJump(InputAction.CallbackContext context){
        _jumped = context.action.triggered;
    }

    private void FixedUpdate(){
        transform.rotation = Quaternion.Euler(0, 0, _rb.velocity.y * _rotationSpeed); 
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("Player") && !_isDead){
            _isDead = true;
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = _deadBirdSprite;
            _gameOverPanel.SetActive(true);
            _playerInput.DeactivateInput();
            _hitSound.Play();
            PlayerConfigurationManager.Instance.playerConfigs[_playerInput.playerIndex].isAlive = false;
            CheckForLivingPlayers();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Pipe")){
            UpdateScore();
            _pipeSound.Play();
        }
    }

    private void UpdateScore(){
        _score++;
        _scoreText.text = _score.ToString();
    }

    private void CheckForLivingPlayers()
    {
        int livingPlayersCount = 0;

        foreach (var playerConfiguration in PlayerConfigurationManager.Instance.playerConfigs)
        {
            if (playerConfiguration.isAlive)
            {
                livingPlayersCount++;
            }
        }

        if (livingPlayersCount == 0)
            StartCoroutine(OpenEndGameMenu());
    }

    private IEnumerator OpenEndGameMenu()
    {
        yield return new WaitForSeconds(3.2f);
        _endGameMenu.SetActive(true);
    }
}

