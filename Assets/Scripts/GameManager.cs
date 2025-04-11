using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Other managers: set in inspector
    [Header("Managers")]
    [SerializeField] private PathManager _pathManager;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private TextUIManager _textUIManager;

    // Player stats
    [Header("Player Stats")]
    private int _playerLives = 0;
    private int _playerOrbs = 0;
    [SerializeField] private int _defaultPlayerLives; // Set in inspector

    // Waves
    [Header("Waves")]
    [SerializeField] private int _maxWaves;
    private int _currentWave = 1;

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

        // Initialize text to UI
        InstantiateUIText();
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

        // Updates the lives text.
        _textUIManager.UpdateLivesText(_playerLives);
    }

    // Puts text within the UI at the start of runtime.
    private void InstantiateUIText()
    {
        _textUIManager.UpdateWavesText(_currentWave, _maxWaves);
        _textUIManager.UpdateLivesText(_playerLives);
        _textUIManager.UpdateOrbsText(_playerOrbs);
    }
}