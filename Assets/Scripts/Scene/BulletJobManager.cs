using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class BulletJobManager : MonoBehaviour {
    
    private List<Bullet> _bullets = new List<Bullet>();
    
    
    # region Unity methods
    
    private void Update() {
        int bulletCount = _bullets.Count;
        
        if (bulletCount == 0) {
            return;
        }
        NativeArray<Vector3> starts = new NativeArray<Vector3>(bulletCount, Allocator.TempJob);
        NativeArray<Vector3> controls = new NativeArray<Vector3>(bulletCount, Allocator.TempJob);
        NativeArray<Vector3> ends = new NativeArray<Vector3>(bulletCount, Allocator.TempJob);
        NativeArray<float> ts = new NativeArray<float>(bulletCount, Allocator.TempJob);
        NativeArray<float> nextts = new NativeArray<float>(bulletCount, Allocator.TempJob);
        NativeArray<Vector3> positionResults = new NativeArray<Vector3>(bulletCount, Allocator.TempJob);
        NativeArray<Vector3> nextPositionResults = new NativeArray<Vector3>(bulletCount, Allocator.TempJob);
        NativeArray<Vector3> directions = new NativeArray<Vector3>(bulletCount, Allocator.TempJob);
        NativeArray<Quaternion> rotationResults = new NativeArray<Quaternion>(bulletCount, Allocator.TempJob);
        
        for (int i = 0; i < bulletCount; i++) {
            Bullet bullet = _bullets[i];
            starts[i] = bullet.startPoint;
            controls[i] = bullet.controlPoint;
            ends[i] = bullet.endPoint;
            ts[i] = bullet.t;
            nextts[i] = bullet.t + 0.001f;
        }
        JobHandle moveJob = Moveable.ParabolicMoveFor(starts, controls, ends, ts, positionResults);
        JobHandle nextMoveJob = Moveable.ParabolicMoveFor(starts, controls, ends, nextts, nextPositionResults, moveJob);
        nextMoveJob.Complete();
        
        for (int i = 0; i < bulletCount; i++) {
            directions[i] = Moveable.Direction(nextPositionResults[i], positionResults[i]);
        }
        JobHandle rotationJob = Moveable.InstantRotationFor(directions, rotationResults);
        rotationJob.Complete();

        for (int i = 0; i < bulletCount; i++) {
            Bullet bullet = _bullets[i];
            bullet.transform.position = positionResults[i];
            if (directions[i] != Vector3.zero) {
                bullet.transform.rotation = rotationResults[i];
            }
        }
        starts.Dispose();
        controls.Dispose();
        ends.Dispose();
        ts.Dispose();
        nextts.Dispose();
        positionResults.Dispose();
        nextPositionResults.Dispose();
        directions.Dispose();
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