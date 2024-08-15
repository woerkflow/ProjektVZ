using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class BulletJobManager : MonoBehaviour, IJobSystem {
    
    private readonly List<Bullet> _bullets = new();
    
    private NativeArray<float3> _starts;
    private NativeArray<float3> _ends;
    private NativeArray<float> _travelTime;
    private NativeArray<float> _timesElapsed;
    private NativeArray<float3> _positions;
    private NativeArray<quaternion> _rotations;

    private int _jobCount;
    private JobHandle _moveRotationJob;
    
    
    # region Unity methods
    
    private void OnDestroy() {
        DisposeArrays();
    }
    
    #endregion
    
    
    #region Public Class Methods

    public void CalculateJobCount() {
        _jobCount = _bullets.Count;
    }

    public int GetJobCount() {
        return _jobCount;
    }
    
    public JobHandle ScheduleJobs() {
        EnsureArrayCapacity();
        
        for (int i = 0; i < _jobCount; i++) {
            Bullet bullet = _bullets[i];
            _starts[i] = bullet.start;
            _ends[i] = bullet.end;
            _travelTime[i] = bullet.travelTime;
            _timesElapsed[i] = bullet.timeElapsed;
        }
        return Moveable.ParabolicMoveAndRotationFor(
            _starts, 
            _ends,
            _travelTime,
            _timesElapsed, 
            _positions, 
            _rotations
        );
    }
    
    public void ApplyJobResults() {
        
        for (int i = 0; i < _jobCount; i++) {
            Bullet bullet = _bullets[i];
            bullet.transform.position = _positions[i];
            bullet.transform.rotation = _rotations[i];
        }
    }
    
    public void Register(Bullet bullet) {
        _bullets.Add(bullet);
    }
    
    public void Unregister(Bullet bullet) {
        _bullets.Remove(bullet);
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
        _starts = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _ends = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _travelTime = new NativeArray<float>(_jobCount, Allocator.Persistent);
        _timesElapsed = new NativeArray<float>(_jobCount, Allocator.Persistent);
        _positions = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _rotations = new NativeArray<quaternion>(_jobCount, Allocator.Persistent);
    }
    
    private void DisposeArrays() {
        _positions.Dispose();
        _ends.Dispose();
        _travelTime.Dispose();
        _timesElapsed.Dispose();
        _positions.Dispose();
        _rotations.Dispose();
    }
    
    #endregion
}