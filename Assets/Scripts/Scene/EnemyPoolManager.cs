using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPoolManager : MonoBehaviour {
    
    public Enemy[] enemyPrefabs = new Enemy[2];
    public Enemy[] bossPrefabs = new Enemy[2];
    public Enemy[] minionPrefabs = new Enemy[2];
    
    public bool bossSpawned { get; set; }
    public int currentEnemyAmount { get; set; }
    public int currentSpawnedEnemyAmount { get; set; }
    
    private ObjectPool<Enemy> _enemyPool;
    private IEnemyWaveFactory _factory;
    private List<Enemy> _currentEnemyWave = new();
    private int _maxWaveAmount; // 32
    private int _maxCurrentEnemyAmount; // 96
    private int _roundEnemyAmount; // 32 - 320
    private int _currentRoundCount; // 1 - ...
    private int _currentWaveIndex; // 0 - (_maxWaveAmount - 1)
    
    
    #region Unity Methods

    private void Start() {
        currentSpawnedEnemyAmount = 0;
        currentEnemyAmount = 0;
    }
    
    #endregion
    
    
    #region Object Pooling Methods

    public void Initialize(int maxCurrentEnemyAmount) {
        _maxCurrentEnemyAmount = maxCurrentEnemyAmount;
        _enemyPool = new ObjectPool<Enemy>(
            CreatePooledItem, 
            OnTakeFromPool, 
            OnReturnedToPool, 
            OnDestroyPoolObject,
            true,
            10,
            _maxCurrentEnemyAmount
        );
    }
    
    public void CreateEnemyWave(int currentRoundCount, int maxWaveAmount, int roundEnemyAmount) {
        _currentRoundCount = currentRoundCount;
        _maxWaveAmount = maxWaveAmount;
        _roundEnemyAmount = roundEnemyAmount;
        _currentWaveIndex = 0;

        if (_currentRoundCount % 5 != 0) {
            _factory = new EnemyWaveFactory(
                enemyPrefabs, 
                _maxWaveAmount
            );
        } else {
            _factory = new BossWaveFactory(
                bossPrefabs[(int)(_currentRoundCount * 0.2) - 1],
                minionPrefabs,
                _maxWaveAmount
            );
        }
        _currentEnemyWave = _factory.CreateWave();
    }
    
    private Enemy CreatePooledItem() {
        Enemy enemy = _currentEnemyWave[_currentWaveIndex];
        enemy.SetEnemyPoolManager(this);
        return Instantiate(enemy);
    }
    
    private void OnTakeFromPool(Enemy enemy) {
        enemy.gameObject.SetActive(true);
        enemy.ResetValues();
    }
    
    private void OnReturnedToPool(Enemy enemy) {
        currentEnemyAmount--;
        enemy.gameObject.SetActive(false);
    }
    
    private static void OnDestroyPoolObject(Enemy enemy) {
        Destroy(enemy.gameObject);
    }
    
    #endregion
    
    
    #region Public Wave Methods

    public bool CanSpawnWave()
        => currentSpawnedEnemyAmount < _roundEnemyAmount
           && currentSpawnedEnemyAmount - _maxWaveAmount <= currentEnemyAmount;

    public void SetWaveIndex(int index) {
        _currentWaveIndex = index;
    }
    
    #endregion
    
    
    #region Public Enemy Boss Methods

    public Enemy GetBoss()
        => _factory.CreateBoss();
    
    #endregion
    

    #region Public Enemy Pool Methods
    
    public Enemy GetEnemyFromPool()
        => _enemyPool.Get();
    
    public void ReturnEnemyToPool(Enemy enemy) {
        _enemyPool.Release(enemy);
    }

    public void ClearPool() {
        _enemyPool.Clear();
    }
    
    #endregion
}