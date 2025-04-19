using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float _minSpawnVariation = 0.25f;
    [SerializeField] private float _maxSpawnVariation = 1f;
    [SerializeField] private Wave[] _testLevelWaves;
    [SerializeField] private Wave[] _darkForestWaves;
    [SerializeField] private Wave[] _mushroomForestWaves;
    [SerializeField] private Wave[] _snowyForestWaves;
    private bool _spawnedAllEnemies = false;

    // Getters
    public bool SpawnedAllEnemies => _spawnedAllEnemies;

    public Wave[] GetLevelWaves(Levels currentLevel)
    {
        switch (currentLevel)
        {
            case Levels.DarkForest: return _darkForestWaves;
            case Levels.MushroomForest: return _mushroomForestWaves;
            case Levels.SnowyForest: return _snowyForestWaves;
            default: return _testLevelWaves;
        }
    }

    // Another function used because coroutines need "StartCoroutine()" to start
    public void StartWave(Wave wave) => StartCoroutine(SpawnWave(wave));
    private IEnumerator SpawnWave(Wave wave)
    {
        _spawnedAllEnemies = false;

        for (int i = 0; i < wave.Enemies.Length; i++)
        {
            yield return new WaitForSeconds(Random.Range(_minSpawnVariation, _maxSpawnVariation));

            //Debug.Log($"Spawning: {wave.Enemies[i]}");
            EnemyManager.Instance.SpawnEnemy(wave.Enemies[i]);
        }

        _spawnedAllEnemies = true;
        //Debug.Log($"Spawned all enemies");
    }
}

[System.Serializable]
public class Wave
{
    public EnemyType[] Enemies;
}