using System.Collections;
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
    public Canvas timerPrefab;
    public Camera userCamera;
    
    private Transform _currentSpawnPoint;
    private float _countDown;
    private ObjectPool<Enemy> _pool;

    #region Unity Methods

    private void Start()
    {
        _pool = new ObjectPool<Enemy>(
            CreatePooledItem, 
            OnTakeFromPool, 
            OnReturnedToPool, 
            OnDestroyPoolObject
            );
    }
    
    private void Update() {

        if (_countDown > 0) {
            // Choose randomly spawn point
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);
            _currentSpawnPoint = spawnPoints[spawnPointIndex];
        }
        if (_countDown <= 0f) {
            
            // Spawn randomly amount of enemies
            int enemyAmount = Random.Range(1, maxEnemyAmount);
            StartCoroutine(SpawnWave(enemyAmount));
            
            //Set new countdown
            _countDown = maxCountDown;
        }
        _countDown -= Time.deltaTime;
        _countDown = Mathf.Clamp(_countDown, 0f, Mathf.Infinity);
    }
    
    #endregion
    
    #region Object Pooling
    
    private Enemy CreatePooledItem() {
        return Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
    }

    private Vector3 GetRandomPosition () {
        
        float RandomCoordinate(float value) => value + Random.Range(0f, 0.04f);
        
        return new Vector3(
            RandomCoordinate(_currentSpawnPoint.position.x),
            _currentSpawnPoint.position.y,
            RandomCoordinate(_currentSpawnPoint.position.z)
        );
    }
    
    private void OnTakeFromPool(Enemy enemy) {
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

        Canvas timerUI = Instantiate(
            timerPrefab, 
            _currentSpawnPoint.transform.position,
            _currentSpawnPoint.transform.rotation
            );

        TextMesh timer = timerUI.GetComponent<TextMesh>();
        timer.text = string.Format("{0:00.00}", _countDown);
            
        for (var i = 0; i < enemyAmount; i++) {
            Enemy zombie = _pool.Get();
            zombie.swarmManager = swarmManager;
            swarmManager.Subscribe(zombie);
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }
    }
    
    #endregion
}
