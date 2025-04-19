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

    // Level cycle
    private Levels _currentLevel;

    // Level gameobjects (maybe use array instead?)
    private GameObject _currentLevelObject;
    [SerializeField] private GameObject _testLevel;
    [SerializeField] private GameObject _darkForestLevel;
    [SerializeField] private GameObject _mushroomForestLevel;
    [SerializeField] private GameObject _snowyForestLevel;

    // Getters
    public Levels CurrentLevel => _currentLevel;


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

        // TODO: Add level transition (fade to black?)

        // Clear the placed towers
        GridManager.Instance.ClearGrid();

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

        // Update the UI
        TextUIManager.Instance.UpdateLivesText(GameManager.Instance.PlayerLives);
        TextUIManager.Instance.UpdateOrbsText(GameManager.Instance.PlayerOrbs);

        // TODO: Add level transition (fade from black?)
    }

    public Levels GetNextLevel()
    {
        int index = (int)_currentLevel;

        index++;

        if (index >= Enum.GetNames(typeof(Levels)).Length)
        {
            // Finished all levels
            
            // TODO: Trigger "player wins" event and go back to title screen

            index = 0;
        }

        return (Levels)index;
    }
}