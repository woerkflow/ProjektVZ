using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class TurretJobManager : MonoBehaviour, IJobSystem {
    
    private JobSystemManager _jobSystemManager;
    
    private NativeArray<float3> _positions;
    private NativeArray<float3> _targets;
    private NativeArray<quaternion> _rotations;
    private NativeArray<float> _speeds;
    private NativeArray<quaternion> _results;
    
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
        _jobCount = _jobSystemManager.Turrets.Count;
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
        
        for (int i = 0; i < _jobCount; i++) {
            Turret turret = _jobSystemManager.Turrets[i];
            Transform partToRotate = turret.GetPartToRotate();
            _positions[i] = partToRotate.position;
            _targets[i] = turret.rotateTarget;
            _rotations[i] = partToRotate.rotation;
            _speeds[i] = turret.GetRotationSpeed();
        }
        return new InterpolatedRotationJobFor() {
            Positions = _positions,
            Targets = _targets,
            Rotations = _rotations,
            Speeds = _speeds,
            DeltaTime = Time.deltaTime,
            Results = _results
        }.ScheduleParallel(_targets.Length, 128, default);
    }

    public void ApplyJobResults() {
        
        for (int i = 0; i < _jobCount; i++) {
            Turret turret = _jobSystemManager.Turrets[i];
            turret.GetPartToRotate().rotation = _results[i];
        }
    }
    
    #endregion

    
    #region Private Class Methods
    
    private void CreateArrays() {
        _positions = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _targets = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _rotations = new NativeArray<quaternion>(_jobCount, Allocator.Persistent);
        _speeds = new NativeArray<float>(_jobCount, Allocator.Persistent);
        _results = new NativeArray<quaternion>(_jobCount, Allocator.Persistent);
    }
    
    private void DisposeArrays() {
        if (_positions.IsCreated) _positions.Dispose();
        if (_targets.IsCreated) _targets.Dispose();
        if (_speeds.IsCreated) _speeds.Dispose();
        if (_rotations.IsCreated) _rotations.Dispose();
        if (_results.IsCreated) _results.Dispose();
    }
    
    #endregion
}
