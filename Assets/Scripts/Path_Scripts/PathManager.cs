using UnityEngine;

public class PathManager : MonoBehaviour
{
    // Class that manages the path that enemies follow
    // Can eventually have different paths to choose from for each level?

    // Singleton
    public static PathManager Instance;

    // Different paths
    [SerializeField] private Path _testPath;
    //[SerializeField] private Path _path1;
    //[SerializeField] private Path _path2;

    // Active path
    private Path _activePath = null;

    // Getters
    public Path ActivePath => _activePath;


    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Set the active path and initialize it
        _activePath = _testPath;
        _activePath.InitializePath();
    }

}