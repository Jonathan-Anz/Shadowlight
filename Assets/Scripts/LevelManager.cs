using UnityEngine;

public enum Levels
{
    TestLevel, DarkForest, MushroomForest, SnowyForest
}

public class LevelManager : MonoBehaviour
{
    // Singleton
    public static LevelManager Instance;

    // Level gameobjects (maybe use array instead?)
    private GameObject _currentLevel;
    [SerializeField] private GameObject _testLevel;
    [SerializeField] private GameObject _darkForestLevel;
    [SerializeField] private GameObject _mushroomForestLevel;
    [SerializeField] private GameObject _snowyForestLevel;

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

        // Disable the current loaded level
        _currentLevel?.SetActive(false);

        // Set the current level
        _currentLevel = LevelToGameObject(level);

        // Enable the new level
        _currentLevel.SetActive(true);

        // Enable the path for that level
        PathManager.Instance.SetPath(level);
    }
}