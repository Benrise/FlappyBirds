using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float _velocity = 1.5f;

    [SerializeField]
    private float _rotationSpeed = 10f;

    [SerializeField]
    private TMP_Text _currentScoreText;

    [SerializeField]
    private TMP_Text _highScoreText;

    [SerializeField]
    private AudioSource _pipeSound;

    [SerializeField]
    private AudioSource _wingSound;

    [SerializeField]
    private AudioSource _dieSound;

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

    private void Awake(){
        gameObject.name = $"Player {GetComponent<PlayerInput>().playerIndex.ToString()}";
        _controls = new PlayerControls();
    }

    private void Start(){
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _currentScoreText.text = _score.ToString();
        _playerNumber.text = _playerInput.playerIndex.ToString();
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
        if (!other.gameObject.CompareTag("Player")){
            GetComponent<SpriteRenderer>().sprite = _deadBirdSprite;
            _gameOverPanel.SetActive(true);
            _playerInput.DeactivateInput();
            _dieSound.Play();
        }
        else {
            _hitSound.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Pipe")){
            UpdateScore();
            _pipeSound.Play();
        }
    }

    public void UpdateScore(){
        _score++;
        _currentScoreText.text = _score.ToString();
    }

}
