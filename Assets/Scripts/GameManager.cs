using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Other managers: set in inspector
    [Header("Managers")]
    [SerializeField] private PathManager _pathManager;
    [SerializeField] private EnemyManager _enemyManager;

    // Player stats
    [Header("Player Stats")]
    private int _playerLives = 0;
    private int _playerOrbs = 0;
    [SerializeField] private int _defaultPlayerLives; // Set in inspector

    // Waves
    [Header("Waves")]
    [SerializeField] private int _maxWaves;
    private int _currentWave = 1;

    // UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private TextMeshProUGUI _orbsText;
    [SerializeField] private TextMeshProUGUI _wavesText;

    // Initializiation
    private void Awake()
    {
        // Initialize the path manager
        _pathManager.InitializePathManager();

        // Set the player lives
        _playerLives = _defaultPlayerLives;

        // TEMP: Spawn some enemies
        //_enemyManager.SpawnEnemy(_pathManager.ActivePath);
        SpawnEnemy();
        Invoke("SpawnEnemy", 0.5f);
        Invoke("SpawnEnemy", 1.5f);

        // Add text to UI
        UpdateUIText();
    }

    // TEMP: Invoke doesn't allow parameters
    private void SpawnEnemy()
    {
        _enemyManager.SpawnEnemy(_pathManager.ActivePath);
    }

    // Subscribe functions to events
    private void OnEnable()
    {
        EnemyManager.OnEnemyReachedEnd += RemoveLives;
    }
    private void OnDisable()
    {
        EnemyManager.OnEnemyReachedEnd -= RemoveLives;
    }

    private void RemoveLives(int damage)
    {
        _playerLives -= damage;
        Debug.Log($"Player lost {damage} lives!");

        // Check if the player is out of lives
        if (_playerLives <= 0)
        {
            Debug.Log("Player is dead!");
        }

        UpdateLivesText();
    }

    // UI TEXT FUNCTIONS
    private void UpdateUIText()
    {
        UpdateWavesText();
        UpdateLivesText();
        UpdateOrbsText();
    }

    private void UpdateWavesText()
    {
        _wavesText.text = $"<b>Wave</b>: {_currentWave}/{_maxWaves}";
    }

    private void UpdateLivesText()
    {
        _livesText.text = $"<b>Lives</b>: {_playerLives}";
    }

    private void UpdateOrbsText()
    {
        _orbsText.text = $"<b>Orbs</b>: {_playerOrbs}";
    }
}