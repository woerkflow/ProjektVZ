using System.Collections.Generic;
using UnityEngine;

public class BossWaveFactory : IEnemyWaveFactory {
    private readonly Enemy _bossPrefab;
    private readonly Enemy[] _minionPrefabs;
    private readonly int _minionAmount;

    public BossWaveFactory(Enemy bossPrefab, Enemy[] minionPrefabs, int minionAmount) {
        _bossPrefab = bossPrefab;
        _minionPrefabs = minionPrefabs;
        _minionAmount = minionAmount;
    }

    public Enemy CreateBoss() {
        return Object.Instantiate(_bossPrefab);
    }

    public List<Enemy> CreateWave() {
        List<Enemy> wave = new();
        
        for (int i = 0; i < _minionAmount; i++) {
            wave.Add(_minionPrefabs[Random.Range(0, _minionPrefabs.Length)]);
        }
        return wave;
    }
}