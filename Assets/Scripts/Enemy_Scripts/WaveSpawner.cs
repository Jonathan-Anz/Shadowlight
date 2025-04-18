using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float _minSpawnVariation = 0.25f;
    [SerializeField] private float _maxSpawnVariation = 1f;
    [SerializeField] private Wave[] _sampleSceneWaves;

    private void Start()
    {
        //StartCoroutine(SpawnWave(_sampleSceneWaves[0]));   
    }

    public Wave[] GetLevelWaves(int currentLevel)
    {
        // Eventually add real levels and waves
        switch (currentLevel)
        {
            case 0: return _sampleSceneWaves;
            // case 1: return _levelOneWaves; // Example
            default: return _sampleSceneWaves;
        }
    }

    // Another function used because coroutines need "StartCoroutine()" to start
    public void StartWave(Wave wave) => StartCoroutine(SpawnWave(wave));
    private IEnumerator SpawnWave(Wave wave)
    {
        for (int i = 0; i < wave.Enemies.Length; i++)
        {
            //Debug.Log($"Spawning: {wave.Enemies[i]}");

            EnemyManager.Instance.SpawnEnemy(wave.Enemies[i]);

            yield return new WaitForSeconds(Random.Range(_minSpawnVariation, _maxSpawnVariation));
        }
    }
}

[System.Serializable]
public class Wave
{
    public EnemyType[] Enemies;
}