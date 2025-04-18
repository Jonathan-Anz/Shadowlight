using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;

    // Player stats
    [Header("Player Stats")]
    private int _playerLives = 0;
    private int _playerOrbs = 0;
    [SerializeField] private int _defaultPlayerLives; // Set in inspector

    // Level
    private int _currentLevel = 0;

    // Waves (Moved to Enemy Manager)
    //[Header("Waves")]
    //[SerializeField] private int _maxWaves;
    //private int _currentWave = 1;

    // Getters
    public int PlayerLives => _playerLives;
    public int PlayerOrbs => _playerOrbs;
    public int CurrentLevel => _currentLevel;
    //public int CurrentWave => _currentWave;


    // Initializiation
    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Set the player lives
        _playerLives = _defaultPlayerLives;

        // Initialize text to UI
        InitializeUIText();
    }

    // Scenes
    public void NextLevel()
    {
        // TODO: go to next level
    }

    private void Start()
    {
        // TEMP: Spawn some enemies
        //SpawnEnemy();
        //Invoke("SpawnEnemy", 0.5f);
        //Invoke("SpawnEnemy", 1.5f);
    }
    // TEMP: Invoke doesn't allow parameters
    private void SpawnEnemy()
    {
        //EnemyManager.Instance.SpawnEnemy();
    }

    // Subscribe functions to events
    private void OnEnable()
    {
        EnemyManager.OnEnemyReachedEnd += RemoveLives;
        EnemyManager.OnEnemyDied += AddOrbs;
    }
    private void OnDisable()
    {
        EnemyManager.OnEnemyReachedEnd -= RemoveLives;
        EnemyManager.OnEnemyDied -= AddOrbs;
    }

    // Update stats
    private void RemoveLives(int damage)
    {
        _playerLives -= damage;
        Debug.Log($"Player lost {damage} lives!");

        // Check if the player is out of lives
        if (_playerLives <= 0)
        {
            Debug.Log("Player is dead!");
        }

        // Updates the lives text
        TextUIManager.Instance.UpdateLivesText(_playerLives);
    }
    private void AddOrbs(int orbs)
    {
        _playerOrbs += orbs;
        //Debug.Log($"Player now has {_playerOrbs} orbs");

        // Update the orbs text
        TextUIManager.Instance.UpdateOrbsText(_playerOrbs);
    }

    // Puts text within the UI at the start of runtime.
    private void InitializeUIText()
    {
        //TextUIManager.Instance.UpdateWavesText(_currentWave, _maxWaves);
        TextUIManager.Instance.UpdateWavesText(EnemyManager.Instance.CurrentWave, EnemyManager.Instance.MaxWaves);
        TextUIManager.Instance.UpdateLivesText(_playerLives);
        TextUIManager.Instance.UpdateOrbsText(_playerOrbs);
    }

}