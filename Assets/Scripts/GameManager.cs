using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Other managers: set in inspector
    [SerializeField] private PathManager _pathManager;
    [SerializeField] private EnemyManager _enemyManager;

    private void Awake()
    {
        // Initialize the path manager
        _pathManager.InitializePathManager();

        // TEMP: Spawn an enemy
        _enemyManager.SpawnEnemy(_pathManager.ActivePath);
    }

}