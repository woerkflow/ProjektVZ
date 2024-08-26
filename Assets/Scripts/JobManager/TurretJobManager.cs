using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class TurretJobManager : MonoBehaviour, IJobSystem {
    
    private readonly List<Turret> _turrets = new();
    private NativeArray<float3> _positions;
    private NativeArray<float3> _targets;
    private NativeArray<quaternion> _rotations;
    private NativeArray<float> _speeds;
    private NativeArray<quaternion> _rotationResults;
    private int _jobCount;
    private JobHandle _rotationJob;
    
    
    # region Unity methods
    
    private void OnDestroy() {
        DisposeArrays();
    }
    
    #endregion
    
    
    #region Public Class Methods

    public void CalculateJobCount() {
        _jobCount = _turrets.Count;
    }

    public int GetJobCount() {
        return _jobCount;
    }

    public JobHandle ScheduleJobs() {
        EnsureArrayCapacity();
        
        for (int i = 0; i < _jobCount; i++) {
            Turret turret = _turrets[i];
            Transform partToRotate = turret.GetPartToRotate();
            _positions[i] = partToRotate.position;
            _targets[i] = turret.rotateTarget;
            _rotations[i] = partToRotate.rotation;
            _speeds[i] = turret.GetRotationSpeed();
        }
        return Moveable.InterpolatedRotationFor(
            _positions, 
            _targets, 
            _rotations, 
            _speeds, 
            _rotationResults
        );
    }

    public void ApplyJobResults() {
        
        for (int i = 0; i < _jobCount; i++) {
            Turret turret = _turrets[i];
            turret.GetPartToRotate().rotation = _rotationResults[i];
        }
    }
    
    public void Register(Turret turret) {
        _turrets.Add(turret);
    }

    public void Unregister(Turret turret) {
        _turrets.Remove(turret);
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
        _rotations = new NativeArray<quaternion>(_jobCount, Allocator.Persistent);
        _speeds = new NativeArray<float>(_jobCount, Allocator.Persistent);
        _rotationResults = new NativeArray<quaternion>(_jobCount, Allocator.Persistent);
    }
    
    private void DisposeArrays() {
        if (_positions.IsCreated) _positions.Dispose();
        if (_targets.IsCreated) _targets.Dispose();
        if (_speeds.IsCreated) _speeds.Dispose();
        if (_rotations.IsCreated) _rotations.Dispose();
        if (_rotationResults.IsCreated) _rotationResults.Dispose();
    }
    
    #endregion
}
