using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SpawnJobManager : MonoBehaviour {

    private List<Spawn> _spawns = new List<Spawn>();
    
    #region Unity methods

    private void Update() {
        int spawnCount = _spawns.Count;
        
        if (spawnCount == 0) {
            return;
        }
        NativeArray<Vector3> directions = new NativeArray<Vector3>(spawnCount, Allocator.TempJob);
        NativeArray<Vector3> currentPositions = new NativeArray<Vector3>(spawnCount, Allocator.TempJob);
        NativeArray<float> speeds = new NativeArray<float>(spawnCount, Allocator.TempJob);
        NativeArray<Vector3> positionResults = new NativeArray<Vector3>(spawnCount, Allocator.TempJob);
        NativeArray<Quaternion> rotationResults = new NativeArray<Quaternion>(spawnCount, Allocator.TempJob);
        
        // Add turret values into native arrays
        for (int i = 0; i < spawnCount; i++) {
            Spawn spawn = _spawns[i];
            directions[i] = spawn.direction;
            currentPositions[i] = spawn.transform.position;
            speeds[i] = spawn.currentSpeed;
        }

        // Position
        Moveable.LinearMoveFor(directions, speeds, currentPositions, positionResults);
        
        // Rotation
        Moveable.InstantRotationFor(directions, rotationResults);
        
        for (int i = 0; i < spawnCount; i++) {
            Spawn spawn = _spawns[i];
            spawn.transform.position = positionResults[i];
            spawn.transform.rotation = rotationResults[i];
        }
        
        directions.Dispose();
        currentPositions.Dispose();
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
