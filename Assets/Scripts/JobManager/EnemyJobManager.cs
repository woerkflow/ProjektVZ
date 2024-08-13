using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class EnemyJobManager : MonoBehaviour {
    
    private readonly List<Enemy> _enemies = new();
    
    #region Unity Methods

    private void Update() {
        int enemyCount = _enemies.Count;
        
        if (enemyCount == 0) {
            return;
        }
        NativeArray<float3> positions = new NativeArray<float3>(enemyCount, Allocator.TempJob);
        NativeArray<float3> targets = new NativeArray<float3>(enemyCount, Allocator.TempJob);
        NativeArray<float> speeds = new NativeArray<float>(enemyCount, Allocator.TempJob);
        NativeArray<float3> positionResults = new NativeArray<float3>(enemyCount, Allocator.TempJob);
        NativeArray<quaternion> rotationResults = new NativeArray<quaternion>(enemyCount, Allocator.TempJob);
        
        for (int i = 0; i < enemyCount; i++) {
            Enemy enemy = _enemies[i];
            positions[i] = enemy.transform.position;
            targets[i] = enemy.moveTarget;
            speeds[i] = enemy.currentSpeed;
        }
        JobHandle moveJob = Moveable.LinearMoveFor(positions, targets, speeds, Time.deltaTime, positionResults);
        JobHandle rotationJob = Moveable.InstantRotationFor(positions, targets, rotationResults, moveJob);
        rotationJob.Complete();
        
        for (int i = 0; i < enemyCount; i++) {
            Enemy enemy = _enemies[i];
            enemy.transform.position = positionResults[i];
            enemy.transform.rotation = rotationResults[i];
        }
        positions.Dispose();
        targets.Dispose();
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
