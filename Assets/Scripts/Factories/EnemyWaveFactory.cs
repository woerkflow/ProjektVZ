using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveFactory : IEnemyWaveFactory {
    private readonly Enemy[] _enemyPrefabs;
    private readonly int _enemyAmount;

    public EnemyWaveFactory(Enemy[] enemyPrefabs, int enemyAmount) {
        _enemyPrefabs = enemyPrefabs;
        _enemyAmount = enemyAmount;
    }

    public Enemy CreateBoss() {
        return null;
    }

    public List<Enemy> CreateWave() {
        List<Enemy> wave = new();
        
        for (int i = 0; i < _enemyAmount; i++) {
            wave.Add(_enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)]);
        }
        return wave;
    }
}