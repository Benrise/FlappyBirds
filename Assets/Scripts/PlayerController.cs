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

    private int _score;

    private bool _jumped = false;
    
    private Rigidbody2D _rb;

    private PlayerInput _playerInput;

    private PlayerControls _controls;



    [SerializeField]
    private GameObject _gameOverPanel;

    private void Awake(){
        gameObject.name = $"Player {GetComponent<PlayerInput>().playerIndex.ToString()}";
        _controls = new PlayerControls();
    }

    private void Start(){
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();

        _currentScoreText.text = _score.ToString();
    }

    private void Update(){
        if (_jumped){
            _rb.velocity = Vector2.up * _velocity;
        }
    }

    private void OnEnable(){
        _controls.Enable();
    }

    private void OnDisable(){
        _controls.Disable();
    }

    public void OnJump(InputAction.CallbackContext context){
        _jumped = context.action.triggered;
    }

    private void FixedUpdate() {
        transform.rotation = Quaternion.Euler(0, 0, _rb.velocity.y * _rotationSpeed); 
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("Player"))
        {
            // _gameOverPanel.SetActive(true);
            // _playerInput.DeactivateInput();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Pipe"))
        {
            UpdateScore();
        }
    }

    public void UpdateScore(){
        _score++;
        _currentScoreText.text = _score.ToString();
    }

}
