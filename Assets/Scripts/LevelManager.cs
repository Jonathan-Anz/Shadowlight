using System;
using UnityEngine;

public enum Levels
{
    TestLevel, DarkForest, MushroomForest, SnowyForest
}

public class LevelManager : MonoBehaviour
{
    // Singleton
    public static LevelManager Instance;

    // Save the player stats at the start of a level (in case of a retry)
    private int _playerLivesAtStart;
    private int _playerOrbsAtStart;

    // Level cycle
    private Levels _currentLevel;
    [SerializeField] private Levels _firstLevel = Levels.DarkForest;
    [SerializeField] private Levels _lastLevel = Levels.SnowyForest; // Used to determine the end of the game

    // Saved level
    //private Levels _savedLevel = Levels.DarkForest;

    // Level gameobjects (maybe use array instead?)
    private GameObject _currentLevelObject;
    [SerializeField] private GameObject _testLevel;
    [SerializeField] private GameObject _darkForestLevel;
    [SerializeField] private GameObject _mushroomForestLevel;
    [SerializeField] private GameObject _snowyForestLevel;

    // Level rewards
    [Header("Dark Forest Rewards")]
    [SerializeField] private int _darkForestOrbReward = 0;
    [SerializeField] private TowerType _darkForestTowerReward1;
    
    [Header("Mushroom Forest Rewards")]
    [SerializeField] private int _mushroomForestOrbReward = 0;
    [SerializeField] private TowerType _mushroomForestTowerReward1;
    [SerializeField] private TowerType _mushroomForestTowerReward2;

    // Getters
    public Levels CurrentLevel => _currentLevel;
    public Levels FirstLevel => _firstLevel;
    public Levels LastLevel => _lastLevel;
    //public Levels SavedLevel => _savedLevel;


    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        // Make sure all levels start inactive
        _testLevel.SetActive(false);
        _darkForestLevel.SetActive(false);
        _mushroomForestLevel.SetActive(false);
        _snowyForestLevel.SetActive(false);
    }

    private GameObject LevelToGameObject(Levels level)
    {
        switch (level)
        {
            case Levels.DarkForest: return _darkForestLevel;
            case Levels.MushroomForest: return _mushroomForestLevel;
            case Levels.SnowyForest: return _snowyForestLevel;
            default: return _testLevel;
        }
    }

    public void LoadLevel(Levels level)
    {
        Debug.Log($"Loading {level} level...");

        // Make sure time is running
        Time.timeScale = 1f;

        // TODO: Add level transition (fade to black?)

        // Clear the placed towers
        GridManager.Instance.ResetGrid();

        // Clear any enemies
        EnemyManager.Instance.ClearAllEnemies();

        // Disable the current loaded level
        _currentLevelObject?.SetActive(false);

        // Set the new level as the current one
        _currentLevel = level;
        _currentLevelObject = LevelToGameObject(level);

        // Enable the new level
        _currentLevelObject.SetActive(true);

        // Enable the path for the new level
        PathManager.Instance.SetPath(level);

        // Reset the waves for the new level
        EnemyManager.Instance.ResetWaves();

        // Save the player stats
        _playerLivesAtStart = GameManager.Instance.PlayerLives;
        _playerOrbsAtStart = GameManager.Instance.PlayerOrbs;

        // Update the UI
        TextUIManager.Instance.UpdateLivesText(GameManager.Instance.PlayerLives);
        TextUIManager.Instance.UpdateOrbsText(GameManager.Instance.PlayerOrbs);
        TextUIManager.Instance.ToggleTowerInfo(false);
        TextUIManager.Instance.TogglePauseMenu(false);
        TextUIManager.Instance.ToggleTowerPanels(true);
        TextUIManager.Instance.ToggleLevelCompleteMenu(false);
        TextUIManager.Instance.ToggleGameLostMenu(false);
        TextUIManager.Instance.ToggleGameWinMenu(false);

        // TODO: Add level transition (fade from black?)
    }

    public void ResetLevel()
    {
        // Reset the player values to what they where at the start of the level
        GameManager.Instance.SetPlayerLives(_playerLivesAtStart);
        GameManager.Instance.SetPlayerOrbs(_playerOrbsAtStart);

        LoadLevel(_currentLevel);
    }

    public Levels GetNextLevel()
    {
        int index = (int)_currentLevel;

        index++;

        if (index >= Enum.GetNames(typeof(Levels)).Length)
        {
            index = 0;
        }

        return (Levels)index;
    }

    public void GetLevelRewards()
    {
        // Add tower unlocks as level reward?
        switch(_currentLevel)
        {
            case Levels.DarkForest:
                GameManager.Instance.AddOrbs(_darkForestOrbReward);
                TowerSlotManager.Instance.AddTower(_darkForestTowerReward1);
                break;
            case Levels.MushroomForest:
                GameManager.Instance.AddOrbs(_mushroomForestOrbReward);
                TowerSlotManager.Instance.AddTower(_mushroomForestTowerReward1);
                TowerSlotManager.Instance.AddTower(_mushroomForestTowerReward2);
                break;
            default:
                break;
        }
    }

    // NOT USED BECAUSE SAVE DATA DOESN'T PERSIST BETWEEN SCENES
    // public void SaveCurrentLevel()
    // {
    //     _savedLevel = _currentLevel;
    //     Debug.Log($"Set {_currentLevel} as saved level");
    // }

}