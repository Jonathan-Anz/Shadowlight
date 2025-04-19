using UnityEngine;

public class PathManager : MonoBehaviour
{
    // Class that manages the path that enemies follow
    // Can eventually have different paths to choose from for each level?

    // Singleton
    public static PathManager Instance;

    // Different paths
    [SerializeField] private Path _testPath;
    [SerializeField] private Path _darkForestPath;
    [SerializeField] private Path _mushroomForestPath;
    [SerializeField] private Path _snowyForestPath;

    // Active path
    private Path _activePath = null;

    // Getters
    public Path ActivePath => _activePath;


    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    private Path GetLevelPath(Levels level)
    {
        switch (level)
        {
            case Levels.DarkForest: return _darkForestPath;
            case Levels.MushroomForest: return _mushroomForestPath;
            case Levels.SnowyForest: return _snowyForestPath;
            default: return _testPath;
        }
    }

    public void SetPath(Levels level)
    {
        // Get the path in the scene
        _activePath = GetLevelPath(level);

        if (_activePath == null)
        {
            Debug.LogWarning($"Path for {level} not found!");
            return;
        }

        _activePath.InitializePath();
    }

}