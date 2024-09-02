using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class BulletJobManager : MonoBehaviour, IJobSystem {
    
    private JobSystemManager _jobSystemManager;
    
    private NativeArray<float3> _starts;
    private NativeArray<float3> _ends;
    private NativeArray<float> _travelTime;
    private NativeArray<float> _timesElapsed;
    private NativeArray<float3> _positions;
    private NativeArray<quaternion> _rotations;
    
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
        _jobCount = _jobSystemManager.Bullets.Count;
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
            Bullet bullet = _jobSystemManager.Bullets[i];
            _starts[i] = bullet.start;
            _ends[i] = bullet.end;
            _travelTime[i] = bullet.travelTime;
            _timesElapsed[i] = bullet.timeElapsed;
        }
        return new ParabolicMoveAndRotationJobFor {
            Starts = _starts,
            Ends = _ends,
            TravelTime = _travelTime,
            TimesElapsed = _timesElapsed,
            Gravity = Moveable.Gravity,
            Positions = _positions,
            Rotations = _rotations
        }.ScheduleParallel(_starts.Length, 128, default);
    }
    
    public void ApplyJobResults() {
        
        for (int i = 0; i < _jobCount; i++) {
            Bullet bullet = _jobSystemManager.Bullets[i];
            bullet.transform.position = _positions[i];
            bullet.transform.rotation = _rotations[i];
        }
    }
    
    #endregion
    
    
    #region Private Class Methods
    
    private void CreateArrays() {
        _starts = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _ends = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _travelTime = new NativeArray<float>(_jobCount, Allocator.Persistent);
        _timesElapsed = new NativeArray<float>(_jobCount, Allocator.Persistent);
        _positions = new NativeArray<float3>(_jobCount, Allocator.Persistent);
        _rotations = new NativeArray<quaternion>(_jobCount, Allocator.Persistent);
    }
    
    private void DisposeArrays() {
        if (_starts.IsCreated) _starts.Dispose();
        if (_ends.IsCreated) _ends.Dispose();
        if (_travelTime.IsCreated) _travelTime.Dispose();
        if (_timesElapsed.IsCreated) _timesElapsed.Dispose();
        if (_positions.IsCreated) _positions.Dispose();
        if (_rotations.IsCreated) _rotations.Dispose();
    }
    
    #endregion
}