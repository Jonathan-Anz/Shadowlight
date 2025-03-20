using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Enemy prefabs
    [SerializeField] private GameObject _testEnemyPrefab;

    private List<Enemy> _enemyList = new List<Enemy>();

    // Events
    public static event Action<int> OnEnemyReachedEnd;


    public void SpawnEnemy(Path path)
    {
        Enemy spawnedEnemy = Instantiate(_testEnemyPrefab, Vector3.zero, Quaternion.identity)
                            .GetComponent<Enemy>();

        spawnedEnemy.InitializeEnemy(this, path);
        _enemyList.Add(spawnedEnemy);
    }

    // Enemies that reach the end of the path will call this function
    public void ReachedEndOfPath(Enemy enemy)
    {
        // Trigger the OnEnemyReachedEnd event
        OnEnemyReachedEnd?.Invoke(enemy.Damage);

        // Remove enemy from the enemy list and destroy it
        _enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }

}