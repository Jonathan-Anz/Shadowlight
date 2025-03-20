using UnityEngine;

public class PathManager : MonoBehaviour
{
    // Class that manages the path that enemies follow
    // Can eventually have different paths to choose from for each level?

    // Different paths
    [SerializeField] private Path _testPath;
    //[SerializeField] private Path _path1;
    //[SerializeField] private Path _path2;

    // Active path
    private Path _activePath = null;

    // Getters
    public Path ActivePath => _activePath;


    // Call on initialization
    public void InitializePathManager()
    {
        // Set the active path and initialize it
        _activePath = _testPath;
        _activePath.InitializePath();
    }
    
}