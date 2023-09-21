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
    private AudioSource _hawkSound;

    [SerializeField]
    private AudioSource _healSound;

    [SerializeField]
    private AudioSource _shieldSound;

    [SerializeField]
    private AudioSource _warpSound;

    [SerializeField]
    private AudioSource _energizerSound;

    [SerializeField]
    private AudioSource _amogusSound;

    [SerializeField]
    private Sprite _deadBirdSprite;

    [SerializeField]
    private GameObject _poopPrefab;

    [SerializeField]
    private TMP_Text _playerNumber;

    [SerializeField]
    private GameObject _shieldCooldown;

    [SerializeField]
    private GameObject _energizerCooldown;

    [SerializeField]
    private GameObject _amogusCooldown;

    [SerializeField]
    private Sprite _getReadyGreenSprite;

    [SerializeField]
    private Sprite _getReadyOrangeSprite;

    [SerializeField]
    private Image _getReadyTitleImage;

    [SerializeField]
    private Button _getReadyButton;

    [SerializeField]
    private GameObject _gameOverPanel;

    [SerializeField]
    private GameObject _getReadyPanel;

    [SerializeField]
    private GameObject _statsPanel;

    [SerializeField]
    private TMP_Text _jumpKey;

    private int _score;

    private bool _jumped = false;
    
    private Rigidbody2D _rb;

    private PlayerInput _playerInput;

    private PlayerControls _controls;

    private bool _isDead = false;

    private GameObject _endGameMenu;

    private PlayerConfiguration _player;

    public HealthDisplay healthDisplay;

    public Material damagePlayerMaterial;

    public Material healPlayerMaterial;

    public Material shieldPlayerMaterial;

    public Material energizerPlayerMaterial;

    private SpriteRenderer _spriteRenderer;

    private Material _defaultMaterial;

    private float _distanceToReduceWarp = 0;

    private GameObject[] _players;

    private bool _isGameStarted = false;

    private int _playerLayer;

    private bool _isShieldActive = false;
    private float _shieldDuration = 6f;
    private float _shieldTimer = 0f;

    private bool _isEnergizerActive = false;
    private float _energizerDuration = 6f;
    private float _energizerTimer = 0f;

    private bool _isAmogusActive = false;
    private float _amogusDuration = 6f;
    private float _amogusTimer = 0f;

    private Coroutine _startGameCoroutine;

    private float ignoreInputTime = 2f;
    private bool inputEnabled;

    private void Awake(){
        Random.InitState((int)System.DateTime.Now.Ticks);
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

        _energizerCooldown.SetActive(false);
        _shieldCooldown.SetActive(false);
        _amogusCooldown.SetActive(false);

        _getReadyButton.onClick.AddListener(ToggleReady);
    }

    private void Start(){
        _endGameMenu.SetActive(false);
        StartCoroutine(SpawnPoop());
        _playerLayer = LayerMask.NameToLayer($"Player{_playerInput.playerIndex}Layer");
        _players = GameObject.FindGameObjectsWithTag("Player");
        _rb.simulated = false;
        _jumpKey.text = _playerInput.currentActionMap["Jump"].GetBindingDisplayString(_player.PlayerIndex); // Если брать индекс из PlayerInput, то будет неправильная привязка отображение биндингов с игроком на экране
    }

    private void Update(){

        if (Time.time > ignoreInputTime){
            inputEnabled = true;
        }

        if (_jumped){
            _rb.velocity = Vector2.up * _velocity;
            _wingSound.Play();
            if (!_isEnergizerActive)
                _jumped = false;
        }

        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }

        if (transform.position.y < -1f){
            if (!_isDead)
                KillPlayer();
        }

        if (transform.position.y > 4f){
            if (!_isDead)
                KillPlayer(killedByHawk: true);
        }

        if (_isShieldActive)
        {
            _shieldTimer += Time.deltaTime;

            float remainingTime = Mathf.Max(0f, _shieldDuration - _shieldTimer);
            float fillAmount = remainingTime / _shieldDuration;
            
            _shieldCooldown.GetComponent<Image>().fillAmount = fillAmount;
            if (_shieldTimer >= _shieldDuration)
            {
                DeactivateShield();
            }
        }

        if (_isEnergizerActive)
        {
            _energizerTimer += Time.deltaTime;

            float remainingTime = Mathf.Max(0f, _energizerDuration - _energizerTimer);
            float fillAmount = remainingTime / _energizerDuration;
            _energizerCooldown.GetComponent<Image>().fillAmount = fillAmount;
            if (_energizerTimer >= _energizerDuration)
            {
                DeactivateEnergizer();
            }
        }  

        if (_isAmogusActive)
        {
            _amogusTimer += Time.deltaTime;

            float remainingTime = Mathf.Max(0f, _amogusDuration - _amogusTimer);
            float fillAmount = remainingTime / _amogusDuration;
            _amogusCooldown.GetComponent<Image>().fillAmount = fillAmount;

            if (_amogusTimer >= _amogusDuration)
            {
                DeactivateAmogus();
            }
        }

        if (!_isGameStarted && (PlayerConfigurationManager.Instance.readyPlayerCount == _players.Length))
        {
            _isGameStarted = true; 
            _startGameCoroutine = StartCoroutine(StartGame());
        }
        else if (_isGameStarted && (PlayerConfigurationManager.Instance.readyPlayerCount != _players.Length))
        {
            if (_startGameCoroutine != null)
            {
                StopCoroutine(_startGameCoroutine);
                _startGameCoroutine = null; 
            }
            _isGameStarted = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context){
        if (!inputEnabled) { return; }
        if (_rb.simulated){
            _jumped = context.action.triggered;
            return;
        }
        if (context.action.ReadValue<float>() == 1f) 
        {
            if (!_player.isReady){
                _getReadyButton.image.color = _getReadyButton.colors.pressedColor;
            }
            else {
                _getReadyButton.image.color = new Color(115f / 255f, 115f / 255f, 115f / 255f);
            }
        }
        else if (context.action.ReadValue<float>() == 0f) 
        {
            _getReadyButton.image.color = _getReadyButton.colors.normalColor;
            ToggleReady();
        }
    }

    public void ToggleReady(){
        _player.isReady = !_player.isReady;      
        if (_player.isReady){
            _getReadyTitleImage.sprite = _getReadyGreenSprite;
            _getReadyButton.image.color = Color.grey;
            _getReadyTitleImage.rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else {
            _getReadyTitleImage.sprite = _getReadyOrangeSprite;
            _getReadyButton.image.color = _getReadyButton.colors.normalColor;
            _getReadyTitleImage.rectTransform.localScale = Vector3.one;
        }
        PlayerConfigurationManager.Instance.CheckForAllPlayersReady();
    }

    private void FixedUpdate(){
        transform.rotation = Quaternion.Euler(0, 0, _rb.velocity.y * _rotationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!other.gameObject.CompareTag("Player") && !_isDead){

            if (other.gameObject.CompareTag("Pipe")){
                HitPlayer();
                StartCoroutine(PipeCollision(2f));
            }

            if (_player.Lives == 0 || other.gameObject.CompareTag("Ground")) {
                KillPlayer();
            }

            if (other.gameObject.CompareTag("Amogus") && !_isAmogusActive){
                ActivateAmogus();
                Destroy(other.gameObject);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (!_isDead){
            if (other.gameObject.CompareTag("PipePoint")){
                _player.Points += 1;
                UpdateScore();
                _pipeSound.Play();
            }

            if (other.gameObject.CompareTag("HealthBuff")){
                if (_player.Lives != _player.MaxLives){
                    HealPlayer();
                    Destroy(other.gameObject);
                }
            }

            if (other.gameObject.CompareTag("ShieldBuff") && !_isShieldActive)
            {
                ActivateShield();
                Destroy(other.gameObject); 
            }

            if (other.gameObject.CompareTag("EnergyBuff") && !_isEnergizerActive)
            {
                ActivateEnergizer();
                Destroy(other.gameObject); 
            }

            if (other.gameObject.CompareTag("WarpBuff"))
            {
                if (_player.WarpBuffs > 0){
                    ActivateWarp();
                    Destroy(other.gameObject); 
                }
            }

            if (other.CompareTag("Poop") && other.gameObject.layer != _playerLayer)
            {
                HitPlayer();
                Destroy(other.gameObject);
                if (_player.Lives == 0)
                    KillPlayer();
            }
        }

    }

    private void UpdateScore(){
        _score++;
        _scoreText.text = _score.ToString();
    }

    private void CheckForLivingPlayers(){
        int livingPlayersCount = 0;

        foreach (var playerConfiguration in PlayerConfigurationManager.Instance.playerConfigs)
        {
            if (playerConfiguration.isAlive){
                livingPlayersCount++;
            }
        }

        if (livingPlayersCount == 0){
            StartCoroutine(OpenEndGameMenu());
        }
    }

    private void KillPlayer(bool killedByHawk = false){
        if (killedByHawk){
            _hawkSound.Play();
        }
        else{
            _hitSound.Play();
            _deathSound.Play();
        }
        DeactivateShield();
        DeactivateEnergizer();
        GetComponent<Animator>().enabled = false;
        GetComponent<SpriteRenderer>().sprite = _deadBirdSprite;
        _player.isAlive = false;
        _gameOverPanel.SetActive(true);
        _playerInput.DeactivateInput();
        _isDead = true;
        CheckForLivingPlayers();
        healthDisplay.EraseLives();
        GetComponent<Collider2D>().isTrigger = false;
        StartCoroutine(DeactivatePlayerCamera());
    }

    private void HitPlayer(){
        if (!_isShieldActive){
            _hitSound.Play();
            _spriteRenderer.material = damagePlayerMaterial;
            StartCoroutine(RestorePlayerColor());
            _player.Lives -= 1;
            _distanceToReduceWarp += 0.2f;
            if (_distanceToReduceWarp >= 1){
                _player.WarpBuffs += 1;
                _distanceToReduceWarp = 0;
            }
            healthDisplay.TakeDamage();
            Vector2 currentPosition = _rb.position;
            Vector2 newPosition = new Vector2(currentPosition.x - 0.2f, currentPosition.y);
            _rb.MovePosition(newPosition);
        }
    }

    private void HealPlayer(){
        _player.Lives += 1;
        healthDisplay.Heal();
        _healSound.Play();
        _spriteRenderer.material = healPlayerMaterial;
        StartCoroutine(RestorePlayerColor());
    }

    private void ActivateShield()
    {
        _shieldCooldown.SetActive(true);
        _isShieldActive = true;
        _shieldTimer = 0f;
        _shieldSound.Play();
        StartCoroutine(ShieldBlink()); 
    }

    private void DeactivateShield()
    {
        _shieldCooldown.SetActive(false);
        _isShieldActive = false;
        StopCoroutine(ShieldBlink());
        _spriteRenderer.material = _defaultMaterial; 
    }   


    private void ActivateEnergizer()
    {
        _energizerCooldown.SetActive(true);
        _isEnergizerActive = true;
        _energizerTimer = 0f;
        _energizerSound.Play(); 
        _velocity = 1.4f;
        _rb.gravityScale = 0.8f;
        _rb.mass = 0.9f;
        StartCoroutine(EnergizerBlink()); 
    }

    private void DeactivateEnergizer()
    {
        _energizerCooldown.SetActive(false);
        _isEnergizerActive = false;
        _velocity = 1.5f;
        _rb.gravityScale = 0.65f;
        _rb.mass = 1f;
        StopCoroutine(EnergizerBlink()); 
        _spriteRenderer.material = _defaultMaterial;
    }

    private void ActivateAmogus()
    {
        _amogusCooldown.SetActive(true);
        _isAmogusActive = true;
        _amogusSound.Play();
        _amogusTimer = 0f;
        foreach (var player in _players)
        {
            if (!player.GetComponent<PlayerController>()._isDead && player != null){
                player.GetComponent<Animator>().runtimeAnimatorController = _player.PlayerBirdSpriteAnimation;
                player.GetComponent<SpriteRenderer>().sprite = _player.PlayerBirdSprite;
            }
        }
        StartCoroutine(RestoreOriginalPlayerBirdSprites(_amogusDuration));
    }

    private void DeactivateAmogus()
    {
        _amogusCooldown.SetActive(false);
        _isAmogusActive = false;
    }

    private void ActivateWarp()
    {   
        _warpSound.Play();
        Vector2 currentPosition = _rb.position;
        Vector2 newPosition = new Vector2(currentPosition.x + 1f, currentPosition.y);
        _rb.MovePosition(newPosition);
        _distanceToReduceWarp = 0;
        _player.WarpBuffs -= 1;
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

    private IEnumerator ShieldBlink()
    {
        while (_isShieldActive)
        {
            _spriteRenderer.material = shieldPlayerMaterial;
            yield return new WaitForSeconds(0.2f); 
            _spriteRenderer.material = _defaultMaterial; 
            yield return new WaitForSeconds(0.2f); 
        }
    }

    private IEnumerator RestoreOriginalPlayerBirdSprites(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var player in _players)
        {
            var index = player.GetComponent<PlayerController>()._playerInput.playerIndex;
            if (!player.GetComponent<PlayerController>()._isDead && player != null){
                player.GetComponent<Animator>().runtimeAnimatorController = PlayerConfigurationManager.Instance.playerConfigs[index].PlayerBirdSpriteAnimation;
                player.GetComponent<SpriteRenderer>().sprite = PlayerConfigurationManager.Instance.playerConfigs[index].PlayerBirdSprite;
            }
        }
    }

    private IEnumerator EnergizerBlink()
    {
        while (_isEnergizerActive)
        {
            _spriteRenderer.material = energizerPlayerMaterial; 
            yield return new WaitForSeconds(0.2f); 
            _spriteRenderer.material = _defaultMaterial;
            yield return new WaitForSeconds(0.2f); 
        }
    }

    private IEnumerator SpawnPoop()
    {
        while (true)
        {
            float delay = Random.Range(3f, 10f);
            yield return new WaitForSeconds(delay);

            var poop = Instantiate(_poopPrefab, transform.position, Quaternion.identity);
            poop.layer = _playerLayer;
            Destroy(poop, 3f);
        }
    }

    private IEnumerator OpenEndGameMenu()
    {
        yield return new WaitForSeconds(3f);
        DetermineWinner.Instance.winnerIndex.text = DetermineWinner.Instance.GetWinner().PlayerIndex.ToString();
        DetermineWinner.Instance.score.text = DetermineWinner.Instance.GetWinner().Points.ToString();
        DetermineWinner.Instance.winnerSprite.sprite = DetermineWinner.Instance.GetWinner().PlayerBirdSprite;
        _endGameMenu.SetActive(true);
        foreach (var player in _players)
        {
            Destroy(player);
        }
        
    }

    private IEnumerator StartGame(){
        int delayInSeconds = (_players.Length != 1) ? 3 : 0;
        yield return new WaitForSeconds(delayInSeconds);
        _getReadyPanel.SetActive(false);
        _statsPanel.SetActive(true);
        _rb.simulated = true;
        _rb.velocity = Vector2.up * _velocity;
        _wingSound.Play();
        PipeSpawner.Instance.StartSpawn();
    }

    private IEnumerator DeactivatePlayerCamera(){
        yield return new WaitForSeconds(30f);
        _playerInput.camera.enabled = false;
    }
}

