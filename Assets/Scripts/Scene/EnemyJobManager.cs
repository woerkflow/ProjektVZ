using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class EnemyJobManager : MonoBehaviour {
    
    private List<Enemy> _enemies = new List<Enemy>();
    
    #region Unity Methods

    private void Update() {
        int enemyCount = _enemies.Count;
        
        if (enemyCount == 0) {
            return;
        }
        NativeArray<Vector3> directions = new NativeArray<Vector3>(enemyCount, Allocator.TempJob);
        NativeArray<Vector3> currentPositions = new NativeArray<Vector3>(enemyCount, Allocator.TempJob);
        NativeArray<float> speeds = new NativeArray<float>(enemyCount, Allocator.TempJob);
        NativeArray<Vector3> positionResults = new NativeArray<Vector3>(enemyCount, Allocator.TempJob);
        NativeArray<Quaternion> rotationResults = new NativeArray<Quaternion>(enemyCount, Allocator.TempJob);
        
        for (int i = 0; i < enemyCount; i++) {
            Enemy enemy = _enemies[i];
            directions[i] = enemy.direction;
            currentPositions[i] = enemy.transform.position;
            speeds[i] = enemy.currentSpeed;
        }
        JobHandle moveJob = Moveable.LinearMoveFor(directions, speeds, currentPositions, positionResults);
        JobHandle rotationJob = Moveable.InstantRotationFor(directions, rotationResults, moveJob);
        rotationJob.Complete();
        
        for (int i = 0; i < enemyCount; i++) {
            Enemy enemy = _enemies[i];
            enemy.transform.position = positionResults[i];
            enemy.transform.rotation = rotationResults[i];
        }
        directions.Dispose();
        currentPositions.Dispose();
        speeds.Dispose();
        positionResults.Dispose();
        rotationResults.Dispose();
    }
    
    #endregion
    
    #region Public class methods
    
    public void Register(Enemy enemy) {
        _enemies.Add(enemy);
    }

    public void Unregister(Enemy enemy) {
        _enemies.Remove(enemy);
    }
    
    #endregion
}
