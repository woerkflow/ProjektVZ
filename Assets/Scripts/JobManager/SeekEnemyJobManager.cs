using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SeekEnemyJobManager : MonoBehaviour, IJobSystem {
    
    private JobSystemManager _jobSystemManager;
    
    private NativeArray<float3> _seekerPositions;
    private NativeArray<float3> _targetPositions;
    private NativeArray<float3> _nearestTargetPositions;
    
    private int _spawnsCount;
    private int _turretsCount;
    private int _enemiesCount;
    private int _jobCount;
    
    
    # region Unity methods
    
    private void OnDestroy() {
        DisposeArrays();
    }
    
    #endregion
    
    
    #region Public Class methods
    
    public void Register(JobSystemManager jobSystemManager) {
        _jobSystemManager = jobSystemManager;
    }

    public void CalculateJobCount() {
        _enemiesCount = _jobSystemManager.Enemies.Count;
        _spawnsCount = _jobSystemManager.Spawns.Count;
        _turretsCount = _jobSystemManager.Turrets.Count;
        _jobCount = _enemiesCount > 0 
            ? _spawnsCount + _turretsCount
            : 0;
    }

    public int GetJobCount() => _jobCount;
    
    public void EnsureArrayCapacity() {
        CreateArrays();
    }

    public JobHandle ScheduleJobs() {
        
        for (int i = 0; i < _spawnsCount; i++) {
            Spawn spawn = _jobSystemManager.Spawns[i];
            _seekerPositions[i] = spawn.transform.position;
        }
        
        for (int i = 0; i < _turretsCount; i++) {
            Turret turret = _jobSystemManager.Turrets[i];
            int index = i + _spawnsCount;
            _seekerPositions[index] = turret.transform.position;
        }
        
        for (int i = 0; i < _enemiesCount; i++) {
            Enemy enemy = _jobSystemManager.Enemies[i];
            _targetPositions[i] = enemy.transform.position;
        }
        return new FindNearestJobFor() {
            SeekerPositions = _seekerPositions,
            TargetPositions = _targetPositions,
            NearestTargetPositions = _nearestTargetPositions
        }.ScheduleParallel(_seekerPositions.Length, 128, default);
    }

    public void ApplyJobResults() {
        
        for (int i = 0; i < _spawnsCount; i++) {
            Spawn spawn = _jobSystemManager.Spawns[i];
            
            for (int j = 0; j < _enemiesCount; j++) {
                Enemy enemy = _jobSystemManager.Enemies[j];
                
                if (!Moveable.IsEqualPosition(enemy.transform.position, _nearestTargetPositions[i])) {
                    continue;
                }
                spawn.SetTarget(enemy.gameObject);
            }
        }
        
        for (int i = 0; i < _turretsCount; i++) {
            Turret turret = _jobSystemManager.Turrets[i];
            int index = i + _spawnsCount;
            
            for (int j = 0; j < _enemiesCount; j++) {
                Enemy enemy = _jobSystemManager.Enemies[j];

                if (!Moveable.IsEqualPosition(enemy.transform.position, _nearestTargetPositions[index])) {
                    continue;
                }
                turret.SetTarget(enemy);
            }
        }
        DisposeArrays();
    }
    
    #endregion
    
    
    #region Private Class Methods
    
    private void CreateArrays() {
        _seekerPositions = new NativeArray<float3>(_jobCount, Allocator.TempJob);
        _targetPositions = new NativeArray<float3>(_enemiesCount, Allocator.TempJob);
        _nearestTargetPositions = new NativeArray<float3>(_jobCount, Allocator.TempJob);
    }
    
    private void DisposeArrays() {
        if (_seekerPositions.IsCreated) _seekerPositions.Dispose();
        if (_targetPositions.IsCreated) _targetPositions.Dispose();
        if (_nearestTargetPositions.IsCreated) _nearestTargetPositions.Dispose();
    }
    
    #endregion
}