using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class TurretJobManager : MonoBehaviour {
    
    private readonly List<Turret> _turrets = new();
    
    
    #region Unity methods

    private void Update() {
        int turretCount = _turrets.Count;
        
        if (turretCount == 0) {
            return;
        }
        NativeArray<float3> positions = new NativeArray<float3>(turretCount, Allocator.TempJob);
        NativeArray<float3> targets = new NativeArray<float3>(turretCount, Allocator.TempJob);
        NativeArray<quaternion> rotations = new NativeArray<quaternion>(turretCount, Allocator.TempJob);
        NativeArray<float> speeds = new NativeArray<float>(turretCount, Allocator.TempJob);
        NativeArray<quaternion> rotationResults = new NativeArray<quaternion>(turretCount, Allocator.TempJob);
        
        for (int i = 0; i < turretCount; i++) {
            Turret turret = _turrets[i];
            positions[i] = turret.partToRotate.position;
            targets[i] = turret.rotateTarget;
            rotations[i] = turret.partToRotate.transform.rotation;
            speeds[i] = turret.rotationSpeed;
        }
        JobHandle rotationJob = Moveable.InterpolatedRotationFor(positions, targets, rotations, speeds, rotationResults);
        rotationJob.Complete();
        
        for (int i = 0; i < turretCount; i++) {
            Turret turret = _turrets[i];
            turret.partToRotate.transform.rotation = rotationResults[i];
        }
        positions.Dispose();
        targets.Dispose();
        rotations.Dispose();
        speeds.Dispose();
        rotationResults.Dispose();
    }
    
    #endregion
    
    
    #region Public class methods
    
    public void Register(Turret turret) {
        _turrets.Add(turret);
    }

    public void Unregister(Turret turret) {
        _turrets.Remove(turret);
    }
    
    #endregion
}
