using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SpawnJobManager : MonoBehaviour, IJobSystem {

    private JobSystemManager _jobSystemManager;
    
    private NativeArray<float3> _positions;
    private NativeArray<float3> _targets;
    private NativeArray<float> _speeds;
    private NativeArray<float3> _positionResults;
    private NativeArray<quaternion> _rotationResults;
    
    private int _spawnsCount;
    private int _enemiesCount;
    private int _jobCount;
    
    
    # region Unity methods
    
    private void OnDestroy() {
        DisposeArrays();
    }
    
    #endregion
    
    
    #region Public Class Methods

    public void Register(JobSystemManager jobSystemManager) {
        _jobSystemManager = jobSystemManager;
    }

    public void CalculateJobCount() {
        _enemiesCount = _jobSystemManager.Enemies.Count;
        _spawnsCount = _jobSystemManager.Spawns.Count;
        _jobCount = _spawnsCount + _enemiesCount;
    }

    public int GetJobCount() => _jobCount;
    
        
    public void EnsureArrayCapacity() {

        if (_positions.IsCreated) {
            
            if (_positions.Length > _jobCount) {
                return;
            }
            DisposeArrays();
        }
        CreateArrays();
    }

    public JobHandle ScheduleJobs() {
        
        for (int i = 0; i < _spawnsCount; i++) {
            Spawn spawn = _jobSystemManager.Spawns[i];
            _positions[i] = spawn.transform.position;
            _targets[i] = spawn.moveTarget;
            _speeds[i] = spawn.GetSpeed();
        }
        
        for (int i = 0; i < _enemiesCount; i++) {
            Enemy enemy = _jobSystemManager.Enemies[i];
            int index = i + _spawnsCount;
            _positions[index] = enemy.transform.position;
            _targets[index] = enemy.moveTarget;
            _speeds[index] = enemy.currentSpeed;
        }
        return new CurvedMoveJobFor {
            Positions = _positions,
            Targets = _targets,
            Speeds = _speeds,
            DeltaTime = Time.deltaTime,
            PositionResults = _positionResults,
            RotationResults = _rotationResults
        }.ScheduleParallel(_positions.Length, 128, default);
    }
    
    public void ApplyJobResults() {
        
        for (int i = 0; i < _spawnsCount; i++) {
            Spawn spawn = _jobSystemManager.Spawns[i];
            spawn.transform.position = _positionResults[i];
            spawn.transform.rotation = _rotationResults[i];
        }
        
        for (int i = 0; i < _enemiesCount; i++) {
            Enemy enemy = _jobSystemManager.Enemies[i];
            int index = i + _spawnsCount;
            enemy.transform.position = _positionResults[index];
            enemy.transform.rotation = _rotationResults[index];
        }
    }

    #endregion
    
    
    #region Private Class Methods
    
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
