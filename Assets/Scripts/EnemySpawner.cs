using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private float _maxInterval = 1;
    [SerializeField] private float _minInterval = 0;
    [SerializeField] private int _enemyCount = 20;
    [SerializeField] private GameObject[] _enemyPrefabs;
    [SerializeField] private Transform[] _spawners;

    public int EnemiesLeftToSpawn => _enemyCount;
    public int EnemiesAlive {private set; get; } = 0;

    public void StartSpawning() =>
        StartCoroutine(SpawnerCoroutine());

    private IEnumerator SpawnerCoroutine()
    {
        while(_enemyCount > 0)
        {
            SpawnEnemy();
            _enemyCount--;
            yield return new WaitForSeconds(Random.Range(_minInterval, _maxInterval));
        }
    }

    private void SpawnEnemy()
    {
        var point = _spawners[Random.Range(0, _spawners.Length)];
        var enemy = Instantiate(_enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)]);
        enemy.transform.position = point.transform.position;
        EnemiesAlive++;
        enemy.GetComponent<CharacterHealth>().OnDeath += () => EnemiesAlive--;
    }

}
