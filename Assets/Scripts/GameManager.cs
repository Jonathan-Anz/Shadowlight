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

    // Levels
    private int _currentLevel = 0;
    private Levels currentLevel;

    // Game
    private bool _isPaused = false;

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
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        // Set the player lives
        _playerLives = _defaultPlayerLives;

        // Initialize text to UI
        //InitializeUIText();
    }

    private void Start()
    {
        // TEMP: Load a level for the first time
        // Eventually change to load from saved game data
        LevelManager.Instance.LoadLevel(Levels.TestLevel);
    }

    // Scenes
    // private Levels GetCurrentLevel(string sceneName)
    // {
    //     if (Enum.TryParse(sceneName, out Levels level))
    //     {
    //         return level;
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"Could not match scene name {sceneName} to Levels enum!");
    //         return Levels.TitleScreen;
    //     }

    // }
    public void NextLevel()
    {
        _currentLevel++;
        string levelToLoad = LevelToSceneName(_currentLevel);
        Debug.Log($"Loading {levelToLoad}...");
        SceneManager.LoadScene(levelToLoad);
    }
    private string LevelToSceneName(int currentLevel)
    {
        Levels level = (Levels)currentLevel;
        return level.ToString();
    }
    // Called when a level is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log($"Loaded {scene.name}");

        // Figure out the current level
        //currentLevel = GetCurrentLevel(scene.name);

        // When a new level is loaded...
        // if (scene.name == Levels.TitleScreen.ToString())
        // {
        //     // If it's the title screen
        //     //Debug.Log("Title Screen!");

        //     // Show the title canvas
        //     TextUIManager.Instance.ToggleTitleCanvas(true);

        //     // Hide the game canvas
        //     TextUIManager.Instance.ToggleGameCanvas(false);
        // }
        // else
        // {
        //     // If it's a game level
        //     //Debug.Log("Game Level!");

        //     // Find and activate the path for the level
        //     PathManager.Instance.SetPath();

        //     // Reset the waves
        //     EnemyManager.Instance.ResetWaves();

        //     // Show the game canvas
        //     TextUIManager.Instance.ToggleGameCanvas(true);

        //     // Hide the title canvas
        //     TextUIManager.Instance.ToggleTitleCanvas(false);
        // }
    }

    // Game
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

        SceneManager.LoadScene("TitleScene");
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