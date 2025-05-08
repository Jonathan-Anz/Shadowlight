using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType 
{
    // Example
    EmberEnemy, LavaGolem, MushroomEnemy
}

public class EnemyManager : MonoBehaviour
{
    // Singleton
    public static EnemyManager Instance;

    // Enemy prefabs
    [SerializeField] private GameObject _testEnemyPrefab;
    [SerializeField] private GameObject _emberEnemyPrefab;
    [SerializeField] private GameObject _lavaGolemPrefab;
    [SerializeField] private GameObject _mushroomEnemyPrefab;

    private List<Enemy> _enemyList = new List<Enemy>();

    // Waves
    private WaveSpawner _waveSpawner;
    private Wave[] _currentLevelWaves;
    private int _currentWave = 0;
    private bool _waveStarted = false;
    private bool _finishedLevel = false;

    // Events
    public static event Action<int> OnEnemyReachedEnd;
    public static event Action<int> OnEnemyDied;

    // Getters
    public int CurrentWave => _currentWave;
    // Get the current level's max number of waves
    public int MaxWaves => _currentLevelWaves.Length;
    public bool FinishedLevel => _finishedLevel;


    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null) Destroy(gameObject);
        else Instance = this;

        _waveSpawner = GetComponent<WaveSpawner>();
    }

    // Call everytime a level is loaded
    public void ResetWaves()
    {
        _currentWave = 1;
        _waveStarted = false;
        _currentLevelWaves = _waveSpawner.GetLevelWaves(LevelManager.Instance.CurrentLevel);
        _finishedLevel = false;

        // Update UI
        TextUIManager.Instance.ToggleNextButton(true);
        TextUIManager.Instance.UpdateWavesText(_currentWave, MaxWaves);
    }

    // Waves
    public void StartNextWave()
    {
        // Do nothing if wave already started
        if (_waveStarted) return;

        _waveStarted = true;

        // Decrement currentWave by one to account for arrays starting from 0
        Wave wave = _currentLevelWaves[_currentWave - 1];

        _waveSpawner.StartWave(wave);

        // Update the UI
        TextUIManager.Instance.ToggleNextButton(false);
    }
    private void EndWave()
    {
        _waveStarted = false;

        // Check if all the waves are complete
        if (_currentWave == _currentLevelWaves.Length)
        {
            _finishedLevel = true;
            GameManager.Instance.LevelCompleted();
        }

        _currentWave++;

        // Update the UI
        TextUIManager.Instance.ToggleNextButton(true);
        TextUIManager.Instance.UpdateWavesText(_currentWave, MaxWaves);

        // Check if the last level is finished
        if (LevelManager.Instance.CurrentLevel == LevelManager.Instance.LastLevel &&
            _finishedLevel &&
            !GameManager.Instance.GameOver)
        {
            GameManager.Instance.GameCompleted();
        }
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

    public void ClearAllEnemies()
    {
        for (int i = 0; i < _enemyList.Count; i++)
        {
            Destroy(_enemyList[i].gameObject);
        }

        _enemyList.Clear();
    }

    private GameObject EnemyTypeToPrefab(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.EmberEnemy: return _emberEnemyPrefab;
            case EnemyType.LavaGolem: return _lavaGolemPrefab;
            case EnemyType.MushroomEnemy: return _mushroomEnemyPrefab;
            default: return _testEnemyPrefab;
        }
    }

    // Enemies that reach the end of the path will call this function
    public void EnemyReachedEndOfPath(Enemy enemy)
    {
        //Debug.Log($"{enemy.name} reached the end");

        // Trigger the OnEnemyReachedEnd event
        OnEnemyReachedEnd?.Invoke(enemy.Damage);

        // Remove enemy from the enemy list and destroy it
        _enemyList.Remove(enemy);
        Destroy(enemy.gameObject);

        CheckEndOfWave();
    }

    // Enemies that die will call this function
    public void EnemyDied(Enemy enemy)
    {
        // Trigger the OnEnemyDied event
        OnEnemyDied?.Invoke(enemy.OrbAmount);

        _enemyList.Remove(enemy);
        Destroy(enemy.gameObject);

        CheckEndOfWave();
    }

    // Checks if a wave is over
    private void CheckEndOfWave()
    {
        // Check if the wave is over
        if (_waveSpawner.SpawnedAllEnemies && _enemyList.Count == 0)
        {
            Debug.Log($"Wave {_currentWave} finished");
            EndWave();
        }
    }

}