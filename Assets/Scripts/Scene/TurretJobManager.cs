using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TurretJobManager : MonoBehaviour {
    
    private List<Turret> _turrets = new List<Turret>();
    
    
    #region Unity methods

    private void Update() {
        int turretCount = _turrets.Count;
        
        if (turretCount == 0) {
            return;
        }
        NativeArray<Vector3> directions = new NativeArray<Vector3>(turretCount, Allocator.TempJob);
        NativeArray<Quaternion> rotations = new NativeArray<Quaternion>(turretCount, Allocator.TempJob);
        NativeArray<float> speeds = new NativeArray<float>(turretCount, Allocator.TempJob);
        NativeArray<Quaternion> rotationResults = new NativeArray<Quaternion>(turretCount, Allocator.TempJob);
        
        for (int i = 0; i < turretCount; i++) {
            Turret turret = _turrets[i];
            directions[i] = turret.direction;
            rotations[i] = turret.partToRotate.transform.rotation;
            speeds[i] = turret.speed;
        }
        JobHandle rotationJob = Moveable.InterpolatedRotationFor(
            directions, 
            rotations, 
            speeds, 
            rotationResults
        );
        rotationJob.Complete();
        
        for (int i = 0; i < turretCount; i++) {
            Turret turret = _turrets[i];
            turret.partToRotate.transform.rotation = rotationResults[i];
        }
        directions.Dispose();
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
