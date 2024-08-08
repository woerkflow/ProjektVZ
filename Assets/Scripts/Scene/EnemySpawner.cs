using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour {
    
    public static EnemySpawner Instance;
    
    [Header("Spawning")]
    public Enemy[] enemyPrefabs = new Enemy[2];
    public SpawnPoint[] spawnPoints = new SpawnPoint[8];
    public SwarmManager swarmManagerPrefab;
    public int maxWaves;
    public int maxWaveAmount;
    public int maxCurrentEnemyAmount;
    public float maxCountDown;
    
    public enum State {
        Build,
        Fight
    }
    
    [HideInInspector]
    public List<SwarmManager> swarmManagers;
    public State state { get; private set; }
    
    private ObjectPool<Enemy> _pool;
    private SpawnPoint[] _currentSpawnPoints;
    private SpawnPoint _currentSpawnPoint;
    private float _buildCountDown;
    private int _alreadySpawnedEnemyAmount;
    private int _roundEnemyAmount;
    private int _currentEnemyAmount;
    private int _currentRound;
    
    [Header("Timer")]
    public UITimer timer;
    
    private bool _isActive;
    
    
    #region Unity Methods
    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("More than one EnemySpawner at once;");
        } else {
            Instance = this;
        }
    }

    private void Start() {
        
        _pool = new ObjectPool<Enemy>(
            CreatePooledItem, 
            OnTakeFromPool, 
            OnReturnedToPool, 
            OnDestroyPoolObject,
            true,
            10,
            maxCurrentEnemyAmount
        );
        _currentRound = 0;
        ResetRound();
        state = State.Build;
    }
    
    private void Update() {
        
        if (state == State.Build) {

            if (!_isActive) {
                timer.RefreshTimer(_buildCountDown);
                return;
            }

            if (_buildCountDown > 0f) {
                _buildCountDown -= Time.deltaTime;
                _buildCountDown = Mathf.Clamp(_buildCountDown, 0f, Mathf.Infinity);
                timer.RefreshTimer(_buildCountDown);
                return;
            }
            timer.DeactivateTimer();
            state = State.Fight;
        }

        if (state == State.Fight) {
            
            if (_currentEnemyAmount >= maxCurrentEnemyAmount) {
                return;
            }

            if (_alreadySpawnedEnemyAmount < _roundEnemyAmount 
                && _alreadySpawnedEnemyAmount - maxWaveAmount <= _currentEnemyAmount
            ) {
                _currentSpawnPoint = _currentSpawnPoints[Random.Range(0, _currentSpawnPoints.Length)];
                StartCoroutine(
                    SpawnWave(
                        maxWaveAmount,
                        Instantiate(swarmManagerPrefab),
                        swarmManagers,
                        _currentSpawnPoint,
                        _pool
                    )
                );
                _currentEnemyAmount += maxWaveAmount;
                _alreadySpawnedEnemyAmount += maxWaveAmount;
            }
            swarmManagers.RemoveAll(sm => sm == null);
            
            if (swarmManagers.Count > 0) {
                return;
            }
            ResetRound();
            state = State.Build;
        }
    }

    #endregion
    
    
    #region Timer Methods

    public float GetTime() {
        return _buildCountDown;
    }

    public void SetTimer(float newTime) {
        _buildCountDown = newTime;
        timer.RefreshTimer(_buildCountDown);
    }

    public void SetActive(bool value) {
        _isActive = value;
    }
    
    #endregion
    
    
    #region Object Pooling

    private static float GetRandomPoint(float point, float range) 
        => point + Random.Range(-range, range);

    private static Vector3 GetRandomPosition(SpawnPoint currentSpawnPoint) 
        => new (
            GetRandomPoint(currentSpawnPoint.transform.position.x, currentSpawnPoint.spawnRange), 
            currentSpawnPoint.transform.position.y, 
            GetRandomPoint(currentSpawnPoint.transform.position.z, currentSpawnPoint.spawnRange)
        );
    
    private Enemy CreatePooledItem() {
        return Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
    }
    
    private void OnTakeFromPool(Enemy enemy) {
        enemy.ResetValues();
        enemy.SetPool(_pool);
        enemy.gameObject.SetActive(true);
    }
    
    private void OnReturnedToPool(Enemy enemy) {
        _currentEnemyAmount--;
        enemy.gameObject.SetActive(false);
    }
    
    private static void OnDestroyPoolObject(Enemy enemy) {
        Destroy(enemy.gameObject);
    }
    
    #endregion
    
    
    #region Spawn Loop
    
    private static SpawnPoint[] GetRandomSpawnPoints(SpawnPoint[] spawnPoints) {
        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);
        return new[] {
            spawnPoints[randomSpawnPointIndex - 1 < 0 ? spawnPoints.Length - 1 : randomSpawnPointIndex - 1],
            spawnPoints[randomSpawnPointIndex],
            spawnPoints[randomSpawnPointIndex + 1 > spawnPoints.Length - 1 ? 0 : randomSpawnPointIndex + 1]
        };
    }
    
    private static int GetEnemyRoundAmount(int maxWaveAmount, int round, int maxWaves)
        => maxWaveAmount * Math.Min(round, maxWaves);
    
    private void ResetRound() {
        _alreadySpawnedEnemyAmount = 0;
        _currentEnemyAmount = 0;
        _buildCountDown = maxCountDown;
        _currentRound++;
        _currentSpawnPoints = GetRandomSpawnPoints(spawnPoints);
        _roundEnemyAmount = GetEnemyRoundAmount(maxWaveAmount, _currentRound, maxWaves);
        timer.ActivateTimer(_currentSpawnPoints[1].transform, _roundEnemyAmount);
        SetActive(true);
    }
    
    private static IEnumerator SpawnWave(
        int enemyAmount,
        SwarmManager swarmManager,
        List<SwarmManager> swarmManagers,
        SpawnPoint currentSpawnPoint,
        ObjectPool<Enemy> pool
    ) {
        swarmManager.SetSpawnPoint(currentSpawnPoint);
        swarmManagers.Add(swarmManager);
        
        for (int i = 0; i < enemyAmount; i++) {
            Enemy zombie = pool.Get();
            zombie.SetSwarmManager(swarmManager);
            zombie.transform.position = GetRandomPosition(currentSpawnPoint);
            zombie.transform.rotation = currentSpawnPoint.transform.rotation;
            swarmManager.Join(zombie);
            yield return new WaitForSeconds(1f);
        }
    }
    
    #endregion
}