using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Enemy prefabs
    [SerializeField] private GameObject _testEnemyPrefab;

    private List<Enemy> _enemyList = new List<Enemy>();


    public void SpawnEnemy(Path path)
    {
        Enemy spawnedEnemy = Instantiate(_testEnemyPrefab, Vector3.zero, Quaternion.identity)
                            .GetComponent<Enemy>();

        spawnedEnemy.InitializeEnemy(path);
        _enemyList.Add(spawnedEnemy);
    }
    
}