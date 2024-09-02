using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SeekBuildingJobManager : MonoBehaviour, IJobSystem {
    
    private JobSystemManager _jobSystemManager;
    
    private NativeArray<float3> _seekerPositions;
    private NativeArray<float3> _targetPositions;
    private NativeArray<float3> _nearestTargetPositions;

    private int _buildingsCount;
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
        _buildingsCount = _jobSystemManager.Buildings.Count;
        _jobCount = _buildingsCount > 0
            ? _jobSystemManager.Enemies.Count
            : 0;
    }
    
    public int GetJobCount() => _jobCount;
    
    public void EnsureArrayCapacity() {
        CreateArrays();
    }
    
    public JobHandle ScheduleJobs() {
        
        for (int i = 0; i < _jobCount; i++) {
            Enemy enemy = _jobSystemManager.Enemies[i];
            _seekerPositions[i] = enemy.transform.position;
        }
        
        for (int i = 0; i < _buildingsCount; i++) {
            Building building = _jobSystemManager.Buildings[i];
            _targetPositions[i] = building.transform.position;
        }
        return new FindNearestJobFor() {
            SeekerPositions = _seekerPositions,
            TargetPositions = _targetPositions,
            NearestTargetPositions = _nearestTargetPositions
        }.ScheduleParallel(_seekerPositions.Length, 128, default);
    }
    
    public void ApplyJobResults() {
        
        for (int i = 0; i < _jobCount; i++) {
            Enemy enemy = _jobSystemManager.Enemies[i];
            
            for (int j = 0; j < _buildingsCount; j++) {
                Building building = _jobSystemManager.Buildings[j];

                if (Moveable.IsEqualPosition(building.transform.position, _nearestTargetPositions[i])) {
                    enemy.SetTarget(building);
                }
            }
        }
        DisposeArrays();
    }
    
    #endregion
    
    
    #region Private Class Methods
    
    private void CreateArrays() {
        _seekerPositions = new NativeArray<float3>(_jobCount, Allocator.TempJob);
        _targetPositions = new NativeArray<float3>(_buildingsCount, Allocator.TempJob);
        _nearestTargetPositions = new NativeArray<float3>(_jobCount, Allocator.TempJob);
    }
    
    private void DisposeArrays() {
        if (_seekerPositions.IsCreated) _seekerPositions.Dispose();
        if (_targetPositions.IsCreated) _targetPositions.Dispose();
        if (_nearestTargetPositions.IsCreated) _nearestTargetPositions.Dispose();
    }
    
    #endregion
}