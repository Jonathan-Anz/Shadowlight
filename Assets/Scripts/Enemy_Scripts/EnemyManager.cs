using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Singleton
    public static EnemyManager Instance;

    // Enemy prefabs
    [SerializeField] private GameObject _testEnemyPrefab;

    private List<Enemy> _enemyList = new List<Enemy>();

    // Events
    public static event Action<int> OnEnemyReachedEnd;
    public static event Action<int> OnEnemyDied;

    private void Awake()
    {
        // Make sure there is only one instance
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public void SpawnEnemy(Path path)
    {
        Enemy spawnedEnemy = Instantiate(_testEnemyPrefab, Vector3.zero, Quaternion.identity)
                            .GetComponent<Enemy>();

        spawnedEnemy.InitializeEnemy(this, path);
        _enemyList.Add(spawnedEnemy);
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
    }

}