using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class SpawnJobManager : MonoBehaviour {

    private List<Spawn> _spawns = new();
    
    #region Unity methods

    private void Update() {
        int spawnCount = _spawns.Count;
    
        if (spawnCount == 0) {
            return;
        }
        NativeArray<float3> positions = new NativeArray<float3>(spawnCount, Allocator.TempJob);
        NativeArray<float3> targets = new NativeArray<float3>(spawnCount, Allocator.TempJob);
        NativeArray<float> speeds = new NativeArray<float>(spawnCount, Allocator.TempJob);
        NativeArray<float3> positionResults = new NativeArray<float3>(spawnCount, Allocator.TempJob);
        NativeArray<quaternion> rotationResults = new NativeArray<quaternion>(spawnCount, Allocator.TempJob);
    
        for (int i = 0; i < spawnCount; i++) {
            Spawn spawn = _spawns[i];
            positions[i] = spawn.transform.position;
            targets[i] = spawn.moveTarget;
            speeds[i] = spawn.speed;
        }
        JobHandle moveJob = Moveable.LinearMoveFor(positions, targets, speeds, Time.deltaTime, positionResults);
        JobHandle rotationJob = Moveable.InstantRotationFor(positions, targets, rotationResults, moveJob);
        rotationJob.Complete();
    
        for (int i = 0; i < spawnCount; i++) {
            Spawn spawn = _spawns[i];
            spawn.transform.position = positionResults[i];
            spawn.transform.rotation = rotationResults[i];
        }
        positions.Dispose();
        targets.Dispose();
        speeds.Dispose();
        positionResults.Dispose();
        rotationResults.Dispose();
    }

    #endregion
    
    #region Public class methods
    
    public void Register(Spawn spawn) {
        _spawns.Add(spawn);
    }

    public void Unregister(Spawn spawn) {
        _spawns.Remove(spawn);
    }
    
    #endregion
}
