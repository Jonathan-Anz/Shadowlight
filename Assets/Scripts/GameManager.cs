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

    // Getters
    public int PlayerLives => _playerLives;
    public int PlayerOrbs => _playerOrbs;


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
        // Set the player stats
        _playerLives = _defaultPlayerLives;
        _playerOrbs = _defaultPlayerOrbs;

        // Load the first level
        // TEMP: Change to dark forest
        LevelManager.Instance.LoadLevel(Levels.TestLevel);
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
            _playerLives = 0;

            Debug.Log("Player is dead!");
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