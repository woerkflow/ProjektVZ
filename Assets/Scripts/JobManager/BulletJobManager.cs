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
        NativeArray<float3> ends = new NativeArray<float3>(bulletCount, Allocator.TempJob);
        NativeArray<float> travelTime = new NativeArray<float>(bulletCount, Allocator.TempJob);
        NativeArray<float> timesElapsed = new NativeArray<float>(bulletCount, Allocator.TempJob);
        NativeArray<float3> positions = new NativeArray<float3>(bulletCount, Allocator.TempJob);
        NativeArray<quaternion> rotations = new NativeArray<quaternion>(bulletCount, Allocator.TempJob);
        
        for (int i = 0; i < bulletCount; i++) {
            Bullet bullet = _bullets[i];
            starts[i] = bullet.start;
            ends[i] = bullet.end;
            travelTime[i] = bullet.travelTime;
            timesElapsed[i] = bullet.timeElapsed;
        }

        JobHandle moveRotationJob =
            Moveable.ParabolicMoveAndRotationFor(
                starts, 
                ends,
                travelTime,
                timesElapsed, 
                positions, 
                rotations
            );
        moveRotationJob.Complete();

        for (int i = 0; i < bulletCount; i++) {
            Bullet bullet = _bullets[i];
            bullet.transform.position = positions[i];
            bullet.transform.rotation = rotations[i];
        }
        starts.Dispose();
        ends.Dispose();
        timesElapsed.Dispose();
        positions.Dispose();
        rotations.Dispose();
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