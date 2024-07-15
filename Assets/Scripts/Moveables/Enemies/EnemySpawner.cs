using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour {
    
    public static EnemySpawner Instance;
    
    [Header("Spawning")]
    public Enemy[] enemyPrefabs = new Enemy[4];
    public SpawnPoint[] spawnPoints = new SpawnPoint[8];
    public SwarmManager swarmManagerPrefab;
    public int maxEnemyAmount;
    public float maxCountDown;
    
    [Header("Timer")]
    public UITimer timer;
    
    // Object pool
    private ObjectPool<Enemy> _pool;
    
    // State
    public enum State {
        Build,
        Fight
    }
    
    [HideInInspector] 
    public State state { get; private set; }
    
    private float _buildCountDown;
    private SpawnPoint _currentSpawnPoint;
    private int _currentEnemyAmount;
    private List<SwarmManager> _swarmManagers;
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
        
        // Initiate ObjectPool
        _pool = new ObjectPool<Enemy>(
            CreatePooledItem, 
            OnTakeFromPool, 
            OnReturnedToPool, 
            OnDestroyPoolObject
            );
        
        // Initiate state machine
        _swarmManagers = new List<SwarmManager>();
        state = State.Build;
        _buildCountDown = 30f;
        _currentSpawnPoint = spawnPoints[0];
        _currentEnemyAmount = 10;
        SetActive(false);
        timer.ActivateTimer(_currentSpawnPoint.transform, _currentEnemyAmount);
    }
    
    private void Update() {
        
        if (state == State.Build) {

            if (!_isActive) {
                timer.RefreshTimer(_buildCountDown);
                return;
            }

            if (_buildCountDown > 0f) {
                
                // Set build timer
                _buildCountDown -= Time.deltaTime;
                _buildCountDown = Mathf.Clamp(_buildCountDown, 0f, Mathf.Infinity);
                timer.RefreshTimer(_buildCountDown);
                return;
            }
            
            // Spawn enemies
            StartCoroutine(SpawnWave(_currentEnemyAmount));
            
            // Start fighting phase
            timer.DeactivateTimer();
            state = State.Fight;
        }

        if (state == State.Fight) {
            
            if (_swarmManagers.Count > 0) {
                _swarmManagers.RemoveAll(spawn => spawn == null);
                return;
            }
            // Choose randomly spawn point
            _currentSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Get randomly amount of enemies
            _currentEnemyAmount = Random.Range(1, maxEnemyAmount);
            
            // Start building phase
            _buildCountDown = maxCountDown;
            timer.ActivateTimer(_currentSpawnPoint.transform, _currentEnemyAmount);
            SetActive(false);
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
    
    private Enemy CreatePooledItem() {
        return Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
    }
    
    private void OnTakeFromPool(Enemy enemy) {
        
        Vector3 GetRandomPosition() {
            float RandomCoordinate(float value) => value + Random.Range(-_currentSpawnPoint.spawnRange, _currentSpawnPoint.spawnRange);
            return new Vector3(
                RandomCoordinate(_currentSpawnPoint.transform.position.x),
                _currentSpawnPoint.transform.position.y,
                RandomCoordinate(_currentSpawnPoint.transform.position.z)
            );
        }
        enemy.transform.position = GetRandomPosition();
        enemy.transform.rotation = _currentSpawnPoint.transform.rotation;
        enemy.ResetValues();
        enemy.SetPool(_pool);
        enemy.gameObject.SetActive(true);
    }
    
    private void OnReturnedToPool(Enemy enemy) {
        enemy.gameObject.SetActive(false);
    }
    
    private void OnDestroyPoolObject(Enemy enemy) {
        Destroy(enemy.gameObject);
    }
    
    #endregion
    
    
    #region Spawn Loop
    
    private IEnumerator SpawnWave(int enemyAmount) {
        SwarmManager swarmManager = Instantiate(swarmManagerPrefab, transform.position, transform.rotation);
        _swarmManagers.Add(swarmManager);
        
        for (var i = 0; i < enemyAmount; i++) {
            Enemy zombie = _pool.Get();
            zombie.swarmManager = swarmManager;
            swarmManager.Join(zombie);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    }
    
    #endregion
}