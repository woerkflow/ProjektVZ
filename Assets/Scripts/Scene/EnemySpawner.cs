using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour {
    
    public static EnemySpawner Instance { get; private set; }

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
    
    public List<SwarmManager> swarmManagers = new();
    
    public State state { get; private set; }

    private ObjectPool<Enemy> _pool;
    private SpawnPoint[] _currentSpawnPoints;
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
        
        if (Instance != null && Instance != this) {
            Debug.LogError("More than one EnemySpawner at once;");
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        InitializePool();
        _currentRound = 0;
        PrepareForNewRound();
    }

    private void Update() {
        
        if (!_isActive) {
            timer.RefreshTimer(_buildCountDown);
            return;
        }

        switch (state) {
            case State.Build:
                HandleBuildState();
                break;
            case State.Fight:
                HandleFightState();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion

    
    #region Initialization

    private void InitializePool() {
        _pool = new ObjectPool<Enemy>(
            CreatePooledItem, 
            OnTakeFromPool, 
            OnReturnedToPool, 
            OnDestroyPoolObject,
            true,
            10,
            maxCurrentEnemyAmount
        );
    }

    #endregion

    
    #region Timer Methods

    public float GetTime() 
        => _buildCountDown;

    public void SetTimer(float newTime) {
        _buildCountDown = newTime;
        timer.RefreshTimer(_buildCountDown);
    }

    public void SetActive(bool value) {
        _isActive = value;
    }

    #endregion

    
    #region Object Pooling

    private Enemy CreatePooledItem()
        => Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);

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

    
    #region Round Logic

    private static SpawnPoint[] GetRandomSpawnPoints(SpawnPoint[] spawnPoints) {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return new[] {
            spawnPoints[(randomIndex - 1 + spawnPoints.Length) % spawnPoints.Length],
            spawnPoints[randomIndex],
            spawnPoints[(randomIndex + 1) % spawnPoints.Length]
        };
    }

    private static int GetEnemyRoundAmount(int maxWaveAmount, int round, int maxWaves)
        => maxWaveAmount * Mathf.Min(round, maxWaves);

    private void PrepareForNewRound() {
        _alreadySpawnedEnemyAmount = 0;
        _currentEnemyAmount = 0;
        _buildCountDown = maxCountDown;
        _currentRound++;

        if (_currentRound % 5 == 1) {
            _pool.Clear();
        }
        _currentSpawnPoints = GetRandomSpawnPoints(spawnPoints);
        _roundEnemyAmount = GetEnemyRoundAmount(maxWaveAmount, _currentRound, maxWaves);
        
        timer.ActivateTimer(_currentSpawnPoints[1].transform, _roundEnemyAmount);
        SetActive(false);
        state = State.Build;
    }

    #endregion

    
    #region Spawn Loop

    private void HandleBuildState() {
        
        if (_buildCountDown > 0f) {
            _buildCountDown -= Time.deltaTime;
            _buildCountDown = Mathf.Clamp(_buildCountDown, 0f, Mathf.Infinity);
            timer.RefreshTimer(_buildCountDown);
        } else {
            timer.DeactivateTimer();
            state = State.Fight;
        }
    }

    private void HandleFightState() {
        
        if (_currentEnemyAmount >= maxCurrentEnemyAmount) {
            return;
        }

        if (_alreadySpawnedEnemyAmount < _roundEnemyAmount 
            && _alreadySpawnedEnemyAmount - maxWaveAmount <= _currentEnemyAmount) {
            SpawnPoint spawnPoint = _currentSpawnPoints[Random.Range(0, _currentSpawnPoints.Length)];
            SwarmManager swarmManager = Instantiate(swarmManagerPrefab);
            
            StartCoroutine(
                SpawnWave(
                    maxWaveAmount,
                    _pool,
                    swarmManagers,
                    swarmManager,
                    spawnPoint
                )
            );
            _currentEnemyAmount += maxWaveAmount;
            _alreadySpawnedEnemyAmount += maxWaveAmount;
        }
        swarmManagers.RemoveAll(sm => sm == null);

        if (swarmManagers.Count == 0) {
            PrepareForNewRound();
        }
    }

    private static IEnumerator SpawnWave(
        int enemyAmount,
        ObjectPool<Enemy> pool,
        List<SwarmManager> swarmManagers, 
        SwarmManager swarmManager, 
        SpawnPoint currentSpawnPoint
    ) {
        swarmManager.SetSpawnPoint(currentSpawnPoint);
        swarmManagers.Add(swarmManager);

        for (int i = 0; i < enemyAmount; i++) {
            Enemy enemy = pool.Get();
            enemy.transform.position = GetRandomPosition(currentSpawnPoint);
            enemy.transform.rotation = currentSpawnPoint.transform.rotation;
            enemy.SetSwarmManager(swarmManager);
            swarmManager.Join(enemy);
            yield return new WaitForSeconds(1f);
        }
    }

    private static Vector3 GetRandomPosition(SpawnPoint currentSpawnPoint) {
        Vector3 position = currentSpawnPoint.transform.position;
        float range = currentSpawnPoint.spawnRange;
        return new Vector3(
            position.x + Random.Range(-range, range),
            position.y,
            position.z + Random.Range(-range, range)
        );
    }

    #endregion
}