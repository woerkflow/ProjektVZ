using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour {
    
    [Header("Spawning")]
    public Enemy[] enemyPrefabs = new Enemy[4];
    public Transform[] spawnPoints = new Transform[8];
    public SwarmManager swarmManagerPrefab;
    public int maxEnemyAmount;
    public float maxCountDown;
    
    [Header("UI")]
    public Camera userCamera;
    public Canvas timer;
    public TMP_Text timerText;
    public TMP_Text amountText;
    
    // Object pool
    private ObjectPool<Enemy> _pool;
    
    // State
    private float _buildCountDown;
    private string _state;
    private Transform _currentSpawnPoint;
    private int _currentEnemyAmount;
    private List<SwarmManager> _swarmManagers;
    
    
    #region Unity Methods

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
        _state = "Build";
        _buildCountDown = 10f;
        _currentSpawnPoint = spawnPoints[0];
        _currentEnemyAmount = 10;
        ActivateTimer();
    }
    
    private void Update() {
        
        if (_state.Equals("Build")) {

            if (_buildCountDown > 0f) {
                
                // Set build timer
                _buildCountDown -= Time.deltaTime;
                _buildCountDown = Mathf.Clamp(_buildCountDown, 0f, Mathf.Infinity);
                RefreshTimer();
                RotateTimerToCamera();
                return;
            }
            // Spawn enemies
            StartCoroutine(SpawnWave(_currentEnemyAmount));
            
            // Start fighting phase
            DeactivateTimer();
            _state = "Fight";
        }

        if (_state.Equals("Fight")) {
            
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
            ActivateTimer();
            _state = "Build";
        }
    }
    
    #endregion
    
    
    #region Object Pooling
    
    private Enemy CreatePooledItem() {
        return Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
    }
    
    private void OnTakeFromPool(Enemy enemy) {
        
        Vector3 GetRandomPosition() {
            float RandomCoordinate(float value) => value + Random.Range(0f, 0.04f);
            return new Vector3(
                RandomCoordinate(_currentSpawnPoint.position.x),
                _currentSpawnPoint.position.y,
                RandomCoordinate(_currentSpawnPoint.position.z)
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
    
    
    #region Timer

    private void ActivateTimer() {
        
        // Translate timer to spawn point & rotate it to the user camera
        Vector3 direction = _currentSpawnPoint.position - timer.transform.position;
        timer.transform.Translate(new Vector3(direction.x, 0f, direction.z));
        RotateTimerToCamera();
        
        amountText.SetText(_currentEnemyAmount.ToString());
        timer.gameObject.SetActive(true);
    }

    private void RefreshTimer() {
        timerText.SetText(System.TimeSpan.FromSeconds(_buildCountDown).ToString("mm':'ss"));
    }

    private void RotateTimerToCamera() {
        Vector3 direction = timer.transform.position - userCamera.transform.position;
        timer.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
    }

    private void DeactivateTimer() {
        timer.gameObject.SetActive(false);
    }
    
    #endregion
}
