using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour {
    
    [Header("Spawning")]
    [SerializeField] private SpawnPoint[] spawnPoints = new SpawnPoint[8];
    [SerializeField] private int maxWaves;
    [SerializeField] private int maxWaveAmount;
    [SerializeField] private int maxCurrentEnemyAmount;
    [SerializeField] private float maxCountDown;
    
    public RoundState state { get; private set; }
    public float buildCountDown { get; private set; }

    private EnemyPoolManager _enemyPoolManager;
    private ObjectPool<Enemy> _pool;
    private SpawnPoint[] _currentSpawnPoints;
    private int _roundEnemyAmount;
    private int _currentRoundCount;
    
    [Header("Swarm Management")]
    [SerializeField] private SwarmManager swarmManagerPrefab;
    
    private readonly List<SwarmManager> _swarmManagers = new();

    [Header("Timer")]
    [SerializeField] private TimerMenu timer;

    private bool _isActive;

    [Header("Round")] 
    [SerializeField] private AudioClip fightStartClip;

    private FXManager _fxManager;
    
    
    #region Unity Methods
    
    private void Start() {
        InitializeManagers();
        PrepareForNewRound();
    }

    private void Update() {
        
        if (!_isActive) {
            timer.Refresh(buildCountDown);
            return;
        }
        state.UpdateState(this);
    }

    #endregion

    
    #region Timer Methods

    public void SetTimer(float newTime) {
        buildCountDown = newTime;
        timer.Refresh(buildCountDown);
    }

    public void SetActive(bool value) {
        _isActive = value;
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

    private static int GetRoundEnemyAmount(int maxWaveAmount, int round, int maxWaves)
        => maxWaveAmount * Mathf.Min(round, maxWaves);

    private void PrepareForNewRound() {
        buildCountDown = maxCountDown;
        
        _currentRoundCount++;
        _currentSpawnPoints = GetRandomSpawnPoints(spawnPoints);
        _roundEnemyAmount = GetRoundEnemyAmount(maxWaveAmount, _currentRoundCount, maxWaves);

        _enemyPoolManager.ClearPool();
        _enemyPoolManager.CreateEnemyWave(_currentRoundCount, maxWaveAmount, _roundEnemyAmount);
        _enemyPoolManager.bossSpawned = false;
        
        timer.SetPosition(_currentSpawnPoints[1].transform);
        timer.SetValues(_currentRoundCount, _roundEnemyAmount, buildCountDown);
        timer.Activate();
        
        SetActive(false);
        state = new BuildState();
    }

    #endregion

    
    #region Spawn Loop

    public void HandleBuildState() {
        
        if (buildCountDown > 0f) {
            buildCountDown -= Time.deltaTime;
            buildCountDown = Mathf.Clamp(buildCountDown, 0f, Mathf.Infinity);
            timer.Refresh(buildCountDown);
        } else {
            timer.Deactivate();
            PlayFightStartSound();
            state = new FightState();
        }
    }

    public void HandleFightState() {
        
        if (_enemyPoolManager.currentEnemyAmount >= maxCurrentEnemyAmount) {
            return;
        }

        if (_enemyPoolManager.CanSpawnWave()) {
            SpawnPoint spawnPoint = _currentSpawnPoints[Random.Range(0, _currentSpawnPoints.Length)];
            SwarmManager swarmManager = Instantiate(swarmManagerPrefab);
            swarmManager.SetSpawnPoint(spawnPoint);
            _swarmManagers.Add(swarmManager);
            
            StartCoroutine(
                SpawnWave(
                    _enemyPoolManager,
                    swarmManager,
                    spawnPoint,
                    maxWaveAmount
                )
            );
            _enemyPoolManager.currentEnemyAmount += maxWaveAmount;
            _enemyPoolManager.currentSpawnedEnemyAmount += maxWaveAmount;
        }
        _swarmManagers.RemoveAll(sm => !sm);

        if (_swarmManagers.Count == 0) {
            PrepareForNewRound();
        }
    }

    private IEnumerator SpawnWave(
        EnemyPoolManager enemyPoolManager, 
        SwarmManager swarmManager, 
        SpawnPoint spawnPoint, 
        int waveAmount
    ) {
        if (!enemyPoolManager.bossSpawned) {
            Enemy boss = enemyPoolManager.GetBoss();
            
            if (boss) {
                boss.transform.position = spawnPoint.transform.position;
                boss.transform.rotation = spawnPoint.transform.rotation;
                boss.SetSwarmManager(swarmManager);
                swarmManager.Register(boss);
                enemyPoolManager.bossSpawned = true;
                yield return new WaitForSeconds(1f);
            }
        }

        for (int i = 0; i < waveAmount; i++) {
            _enemyPoolManager.currentWaveIndex = i;
            Enemy enemy = _enemyPoolManager.GetEnemyFromPool();
            enemy.transform.position = GetRandomPosition(spawnPoint);
            enemy.transform.rotation = spawnPoint.transform.rotation;
            enemy.SetSwarmManager(swarmManager);
            swarmManager.Register(enemy);
            yield return new WaitForSeconds(1f);
        }
    }

    private static Vector3 GetRandomPosition(SpawnPoint currentSpawnPoint) {
        Vector3 position = currentSpawnPoint.transform.position;
        float range = currentSpawnPoint.GetSpawnRange();
        return new Vector3(
            position.x + Random.Range(-range, range),
            position.y,
            position.z + Random.Range(-range, range)
        );
    }

    #endregion
    
    
    #region Private Class Methods

    private void PlayFightStartSound() {
        _fxManager.PlaySound(fightStartClip, transform.position, 0.5f);
    }

    private void InitializeManagers() {
        _enemyPoolManager = FindObjectOfType<EnemyPoolManager>();

        if (!_enemyPoolManager) {
            Debug.LogError("EnemyPoolManager not found in the scene.");
            return;
        }
        _enemyPoolManager.Initialize(maxCurrentEnemyAmount);
        _fxManager = FindObjectOfType<FXManager>();
    }
    
    #endregion
}