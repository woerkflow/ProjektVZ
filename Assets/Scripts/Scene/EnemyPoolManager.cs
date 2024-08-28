using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPoolManager : MonoBehaviour {
    
    [SerializeField] private Enemy[] enemyPrefabs = new Enemy[2];
    [SerializeField] private Enemy[] bossPrefabs = new Enemy[2];
    [SerializeField] private Enemy[] minionPrefabs = new Enemy[2];
    
    public bool bossSpawned { get; set; }
    public int currentEnemyAmount { get; set; }
    public int currentSpawnedEnemyAmount { get; set; }
    public int currentWaveIndex { get; set; }
    
    private ObjectPool<Enemy> _enemyPool;
    private IEnemyWaveFactory _factory;
    private List<Enemy> _currentEnemyWave = new();
    private int _maxWaveAmount;
    private int _maxCurrentEnemyAmount;
    private int _roundEnemyAmount;
    private int _currentRoundCount;
    
    
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
        currentSpawnedEnemyAmount = 0;
        currentEnemyAmount = 0;

        if (_currentRoundCount % 5 != 0) {
            _factory = new EnemyWaveFactory(
                enemyPrefabs, 
                _maxWaveAmount
            );
        } else {
            _factory = new BossWaveFactory(
                bossPrefabs[_currentRoundCount % 2],
                minionPrefabs,
                _maxWaveAmount
            );
        }
        _currentEnemyWave = _factory.CreateWave();
    }
    
    private Enemy CreatePooledItem() 
        => Instantiate(_currentEnemyWave[currentWaveIndex]);
    
    private static void OnTakeFromPool(Enemy enemy) {
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
    
    public void ClearPool() {
        _enemyPool.Clear();
    }
    
    #endregion
    
    
    #region Public Wave Methods

    public bool CanSpawnWave()
        => currentSpawnedEnemyAmount < _roundEnemyAmount
           && currentSpawnedEnemyAmount - _maxWaveAmount <= currentEnemyAmount;
    
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
    
    #endregion
}