using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Other managers: set in inspector
    [SerializeField] private PathManager _pathManager;
    [SerializeField] private EnemyManager _enemyManager;

    // Player stats
    private int _playerLives = 0;
    [SerializeField] private int _defaultPlayerLives; // Set in inspector


    // Initializiation
    private void Awake()
    {
        // Initialize the path manager
        _pathManager.InitializePathManager();

        // Set the player lives
        _playerLives = _defaultPlayerLives;

        // TEMP: Spawn an enemy
        _enemyManager.SpawnEnemy(_pathManager.ActivePath);
    }

    // Subscribe functions to events
    private void OnEnable()
    {
        EnemyManager.OnEnemyReachedEnd += RemoveLives;
    }
    private void OnDisable()
    {
        EnemyManager.OnEnemyReachedEnd -= RemoveLives;
    }

    private void RemoveLives(int damage)
    {
        _playerLives -= damage;
        Debug.Log($"Player lost {damage} lives!");

        // Check if the player is out of lives
    }

}