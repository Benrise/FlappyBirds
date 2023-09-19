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
    private AudioSource _deathSound;

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

    private PlayerConfiguration _player;

    public HealthDisplay healthDisplay;

    public Material redPlayerMaterial;

    private SpriteRenderer _spriteRenderer;

    private Material _defaultMaterial;

    private void Awake(){
        gameObject.name = $"Player {GetComponent<PlayerInput>().playerIndex.ToString()}";
        _controls = new PlayerControls();

        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _endGameMenu = GameObject.Find("EndGameMenu");
        _scoreText.text = _score.ToString();
        _playerNumber.text = _playerInput.playerIndex.ToString();
        _player = PlayerConfigurationManager.Instance.playerConfigs[_playerInput.playerIndex];
        healthDisplay.maxHealth = _player.Lives;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultMaterial = _spriteRenderer.material;
    }

    private void Start(){
        _endGameMenu.SetActive(false);
    }

    private void Update(){
        if (_jumped){
            _rb.velocity = Vector2.up * _velocity;
            _wingSound.Play();
        }

        if (transform.position.y < -10f)
        {
            Destroy(gameObject); 
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
            _hitSound.Play();
            _spriteRenderer.material = redPlayerMaterial;
            StartCoroutine(RestorePlayerColor());
            _player.Lives -= 1;
            healthDisplay.TakeDamage();
            if (_player.Lives == 0 || other.gameObject.CompareTag("Ground")) {
                GetComponent<Animator>().enabled = false;
                GetComponent<SpriteRenderer>().sprite = _deadBirdSprite;
                _player.isAlive = false;
                _gameOverPanel.SetActive(true);
                _playerInput.DeactivateInput();
                _isDead = true;
                CheckForLivingPlayers();
                _deathSound.Play();
                healthDisplay.Kill();
                GetComponent<Collider2D>().isTrigger = true;
            }
        }

        if (other.gameObject.CompareTag("Pipe") && !_isDead){
            StartCoroutine(PipeCollision(2f));
        }


        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("PipePoint")){
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

    private IEnumerator RestorePlayerColor()
    {
        yield return new WaitForSeconds(0.2f);

        _spriteRenderer.material = _defaultMaterial;
    }

    private IEnumerator PipeCollision(float duration)
    {
        GetComponent<Collider2D>().isTrigger = true;
        yield return new WaitForSeconds(duration);
        GetComponent<Collider2D>().isTrigger = false;
    }
}

