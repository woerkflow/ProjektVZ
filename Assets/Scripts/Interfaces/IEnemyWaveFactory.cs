using System.Collections.Generic;

public interface IEnemyWaveFactory {
    Enemy CreateBoss();
    List<Enemy> CreateWave();
}