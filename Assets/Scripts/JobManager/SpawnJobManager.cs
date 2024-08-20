using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SpawnJobManager : MonoBehaviour, IJobSystem {

    private readonly List<Spawn> _spawns = new();
    private readonly List<Enemy> _enemies = new();

    private NativeArray<float3> _positions;
    private NativeArray<float3> _targets;
    private NativeArray<float> _speeds;
    private NativeArray<float3> _positionResults;
    private NativeArray<quaternion> _rotationResults;
    
    private int _spawnsCount;
    private int _enemiesCount;
    private int _jobCount;
    private JobHandle _moveJob;
    
    
    # region Unity methods
    
    private void OnDestroy() {
        DisposeArrays();
    }
    
    #endregion
    
    
    #region Public Class Methods

    public void CalculateJobCount() {
        _enemiesCount = _enemies.Count;
        _spawnsCount = _spawns.Count;
        _jobCount = _spawns.Count + _enemies.Count;
    }

    public int GetJobCount() {
        return _jobCount;
    }

    public JobHandle ScheduleJobs() {
        EnsureArrayCapacity();
        
        for (int i = 0; i < _spawns.Count; i++) {
            Spawn spawn = _spawns[i];
            _positions[i] = spawn.transform.position;
            _targets[i] = spawn.moveTarget;
            _speeds[i] = spawn.speed;
        }
        
        for (int i = 0; i < _enemies.Count; i++) {
            Enemy enemy = _enemies[i];
            int index = i + _spawnsCount;
            _positions[index] = enemy.transform.position;
            _targets[index] = enemy.moveTarget;
            _speeds[index] = enemy.currentSpeed;
        }
        return Moveable.CurvedMoveFor(
            _positions, 
            _targets, 
            _speeds, 
            _positionResults, 
            _rotationResults
        );
    }
    
    public void ApplyJobResults() {
        
        for (int i = 0; i < _spawnsCount; i++) {
            Spawn spawn = _spawns[i];
            spawn.transform.position = _positionResults[i];
            spawn.transform.rotation = _rotationResults[i];
        }
        
        for (int i = 0; i < _enemiesCount; i++) {
            Enemy enemy = _enemies[i];
            int index = i + _spawnsCount;
            enemy.transform.position = _positionResults[index];
            enemy.transform.rotation = _rotationResults[index];
        }
    }
    
    public void RegisterSpawn(Spawn spawn) {
        _spawns.Add(spawn);
    }

    public void UnregisterSpawn(Spawn spawn) {
        _spawns.Remove(spawn);
    }
    
    public void RegisterEnemy(Enemy enemy) {
        _enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy) {
        _enemies.Remove(enemy);
    }

    #endregion
    
    
    #region Private Class Methods
    
    private void EnsureArrayCapacity() {

        if (_positions.IsCreated) {
            
            if (_positions.Length > _jobCount) {
                return;
            }
            DisposeArrays();
        }
        CreateArrays();
    }
    
    private void CreateArrays() {
        _positions = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _targets = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _speeds = new NativeArray<float>(_jobCount, Allocator.Persistent);
        _positionResults = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _rotationResults = new NativeArray<quaternion>(_jobCount, Allocator.Persistent);
    }
    
    private void DisposeArrays() {
        if (_positions.IsCreated) _positions.Dispose();
        if (_targets.IsCreated) _targets.Dispose();
        if (_speeds.IsCreated) _speeds.Dispose();
        if (_positionResults.IsCreated) _positionResults.Dispose();
        if (_rotationResults.IsCreated) _rotationResults.Dispose();
    }
    
    #endregion
}
