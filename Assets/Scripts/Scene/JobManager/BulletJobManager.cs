using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class BulletJobManager : MonoBehaviour {
    
    private readonly List<Bullet> _bullets = new();
    
    
    # region Unity methods
    
    private void Update() {
        int bulletCount = _bullets.Count;
        
        if (bulletCount == 0) {
            return;
        }
        NativeArray<float3> starts = new NativeArray<float3>(bulletCount, Allocator.TempJob);
        NativeArray<float3> controls = new NativeArray<float3>(bulletCount, Allocator.TempJob);
        NativeArray<float3> ends = new NativeArray<float3>(bulletCount, Allocator.TempJob);
        NativeArray<float> ts = new NativeArray<float>(bulletCount, Allocator.TempJob);
        NativeArray<float> nextts = new NativeArray<float>(bulletCount, Allocator.TempJob);
        NativeArray<float3> positionResults = new NativeArray<float3>(bulletCount, Allocator.TempJob);
        NativeArray<float3> nextPositionResults = new NativeArray<float3>(bulletCount, Allocator.TempJob);
        NativeArray<quaternion> rotationResults = new NativeArray<quaternion>(bulletCount, Allocator.TempJob);
        
        for (int i = 0; i < bulletCount; i++) {
            Bullet bullet = _bullets[i];
            starts[i] = bullet.parabolicCurve.start;
            controls[i] = bullet.parabolicCurve.control;
            ends[i] = bullet.parabolicCurve.end;
            ts[i] = bullet.parabolicCurve.t;
            nextts[i] = bullet.parabolicCurve.t + 0.1f;
        }
        JobHandle moveJob = Moveable.ParabolicMoveFor(starts, controls, ends, ts, positionResults);
        JobHandle nextMoveJob = Moveable.ParabolicMoveFor(starts, controls, ends, nextts, nextPositionResults, moveJob);
        JobHandle rotationJob = Moveable.InstantRotationFor(positionResults, nextPositionResults, rotationResults, nextMoveJob);
        rotationJob.Complete();

        for (int i = 0; i < bulletCount; i++) {
            Bullet bullet = _bullets[i];
            bullet.transform.position = positionResults[i];
            bullet.transform.rotation = rotationResults[i];
        }
        starts.Dispose();
        controls.Dispose();
        ends.Dispose();
        ts.Dispose();
        nextts.Dispose();
        positionResults.Dispose();
        nextPositionResults.Dispose();
        rotationResults.Dispose();
    }
    
    #endregion
    
    
    #region Public class methods
    
    public void Register(Bullet bullet) {
        _bullets.Add(bullet);
    }

    public void Unregister(Bullet bullet) {
        _bullets.Remove(bullet);
    }
    
    #endregion
}