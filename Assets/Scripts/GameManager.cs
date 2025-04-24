using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance;

    // Player stats
    [Header("Player Stats")]
    private int _playerLives = 0;
    private int _playerOrbs = 0;
    [SerializeField] private int _defaultPlayerLives; // Set in inspector
    [SerializeField] private int _defaultPlayerOrbs; // Set in inspector

    // Game
    private bool _isPaused = false;
    private bool _gameOver = false;

    // Getters
    public int PlayerLives => _playerLives;
    public int PlayerOrbs => _playerOrbs;
    public bool GameOver => _gameOver;


    // Initializiation
    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        // TEMP: Load a level for the first time
        // Eventually change to load from saved game data
        StartNewGame();
    }


    // Game
    public void StartNewGame()
    {
        _gameOver = false;

        // Set the player stats
        _playerLives = _defaultPlayerLives;
        _playerOrbs = _defaultPlayerOrbs;

        // Load the first level
        LevelManager.Instance.LoadLevel(LevelManager.Instance.FirstLevel);

        // TEMP: Add base towers to player
        TowerSlotManager.Instance.AddTower(TowerType.Bear, 10);
        TowerSlotManager.Instance.AddTower(TowerType.Bat, 5);
        TowerSlotManager.Instance.AddTower(TowerType.Bird, 5);
    }
    public void RetryLevel()
    {
        _gameOver = false;
        LevelManager.Instance.ResetLevel();
    }
    public void PauseGame()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0f;
            //Debug.Log("Game paused");
        }
        else
        {
            Time.timeScale = 1f;
            //Debug.Log("Game resumed");
        }

        TextUIManager.Instance.TogglePauseMenu(_isPaused);
    }
    public void ExitToTitleScreen()
    {
        //Debug.Log("Exit to title screen");

        // Make sure the game is unpaused
        _isPaused = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene("TitleScene");
    }
    public void NextButton()
    {
        if (EnemyManager.Instance.FinishedLevel)
        {
            // TEMP: Go to the next level
            // Add shop menu/scene in between?
            LevelManager.Instance.LoadLevel(LevelManager.Instance.GetNextLevel());
        }
        else
        {
            // Start the next wave
            EnemyManager.Instance.StartNextWave();
        }
    }
    public void LevelCompleted()
    {
        // Give end of level rewards
        LevelManager.Instance.GetLevelRewards();

        TextUIManager.Instance.ToggleLevelCompleteMenu(true);
    }
    public void GameCompleted()
    {
        // Make sure the game is paused
        Time.timeScale = 0f;

        TextUIManager.Instance.ToggleGameWinMenu(true);
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
    public void SetPlayerLives(int lives) => _playerLives = lives;
    public void SetPlayerOrbs(int orbs) => _playerOrbs = orbs;
    private void RemoveLives(int damage)
    {
        _playerLives -= damage;
        Debug.Log($"Player lost {damage} lives!");

        // Check if the player is out of lives
        if (_playerLives <= 0)
        {
            _playerLives = 0;

            //Debug.Log("Player is dead!");

            _gameOver = true;

            // Pause the game
            Time.timeScale = 0f;

            // Trigger game over screen
            TextUIManager.Instance.ToggleGameLostMenu(true);
        }

        // Updates the lives text
        TextUIManager.Instance.UpdateLivesText(_playerLives);
    }
    public void AddOrbs(int orbs)
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