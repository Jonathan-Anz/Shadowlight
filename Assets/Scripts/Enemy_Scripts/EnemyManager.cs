using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType 
{
    // Example
    Small, Medium, Large
}

public class EnemyManager : MonoBehaviour
{
    // Singleton
    public static EnemyManager Instance;

    // Enemy prefabs
    [SerializeField] private GameObject _testEnemyPrefab;

    private List<Enemy> _enemyList = new List<Enemy>();

    // Waves
    private WaveSpawner _waveSpawner;
    private int _currentWave = 0;
    private bool _waveStarted = false;

    // Events
    public static event Action<int> OnEnemyReachedEnd;
    public static event Action<int> OnEnemyDied;

    // Getters
    public int CurrentWave => _currentWave;
    // Get the current level's max number of waves
    public int MaxWaves => _waveSpawner.GetLevelWaves(GameManager.Instance.CurrentLevel).Length;


    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        _waveSpawner = GetComponent<WaveSpawner>();

        _currentWave = 0;
    }

    // Call everytime a level is loaded
    public void ResetWaves()
    {
        _currentWave = 0;
        _waveStarted = false;
        TextUIManager.Instance.ToggleNextButton(true);
    }

    // Waves
    public void StartWave()
    {
        // Do nothing if wave already started
        if (_waveStarted) return;

        // Check if there are no more waves
        if (_currentWave >= MaxWaves)
        {
            Debug.Log($"Finished level");
            GameManager.Instance.NextLevel();
            return;
        }

        _waveStarted = true;

        // Increment the current wave number
        _currentWave++;

        // Get the wave based on the level and the current wave number
        // Decrement currentWave by one to account for arrays starting from 0
        Wave wave = _waveSpawner.GetLevelWaves(GameManager.Instance.CurrentLevel)[CurrentWave - 1];

        _waveSpawner.StartWave(wave);

        // Update the UI
        TextUIManager.Instance.ToggleNextButton(false);
        TextUIManager.Instance.UpdateWavesText(_currentWave, MaxWaves);
    }
    private void EndWave()
    {
        _waveStarted = false;

        TextUIManager.Instance.ToggleNextButton(true);
    }

    public void SpawnEnemy(EnemyType type)
    {
        Enemy spawnedEnemy = Instantiate(   EnemyTypeToPrefab(type),
                                            Vector3.zero, 
                                            Quaternion.identity)
                            .GetComponent<Enemy>();

        spawnedEnemy.InitializeEnemy();
        _enemyList.Add(spawnedEnemy);
    }

    private GameObject EnemyTypeToPrefab(EnemyType type)
    {
        // Eventually add real enemy types and prefabs
        switch (type)
        {
            case EnemyType.Small: return _testEnemyPrefab;
            case EnemyType.Medium: return _testEnemyPrefab;
            case EnemyType.Large: return _testEnemyPrefab;
            default: return _testEnemyPrefab;
        }
    }

    // Enemies that reach the end of the path will call this function
    public void EnemyReachedEndOfPath(Enemy enemy)
    {
        // Trigger the OnEnemyReachedEnd event
        OnEnemyReachedEnd?.Invoke(enemy.Damage);

        // Remove enemy from the enemy list and destroy it
        _enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    // Enemies that die will call this function
    public void EnemyDied(Enemy enemy)
    {
        // Trigger the OnEnemyDied event
        OnEnemyDied?.Invoke(enemy.OrbAmount);

        _enemyList.Remove(enemy);
        Destroy(enemy.gameObject);

        // Check if the wave is over
        if (_waveSpawner.SpawnedAllEnemies && _enemyList.Count == 0)
        {
            Debug.Log($"Wave {_currentWave} finished");
            EndWave();
        }
    }

}