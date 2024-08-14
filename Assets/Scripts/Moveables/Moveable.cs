using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Moveable {
    
    private static readonly Vector3[] Points = new [] {
        new Vector3(-1,0,-1), 
        Vector3.left,
        new Vector3(-1,0,1), 
        Vector3.back,
        Vector3.zero, 
        Vector3.forward,
        new Vector3(1,0,-1), 
        Vector3.right,
        new Vector3(1,0,1),
    };
    
    #region Helper
    
    public static Vector3 Direction(Vector3 target, Vector3 transform) 
        => new (target.x - transform.x, 0f, target.z - transform.z);
    
    public static Vector3 GetRandomPosition(Transform transform) 
        => transform.position + Points[Random.Range(0, Points.Length)];
    
    #endregion
    
    #region Rectlinear Motion
    
    public static JobHandle LinearMoveFor(
        NativeArray<float3> positions,
        NativeArray<float3> targets,
        NativeArray<float> speeds,
        float time,
        NativeArray<float3> results,
        JobHandle dependency = default
    ) {
        LinearMoveJobFor moveJob = new LinearMoveJobFor {
            Positions = positions,
            Targets = targets,
            Speeds = speeds,
            DeltaTime = time,
            Results = results
        };
        return moveJob.ScheduleParallel(targets.Length, 128, dependency);
    }
    
    #endregion
    
    #region Curvilinear Motion
    
    public static JobHandle ParabolicMoveAndRotationFor(
        NativeArray<float3> starts, 
        NativeArray<float3> ends,
        NativeArray<float> travelTime,
        NativeArray<float> timesElapsed,
        NativeArray<float3> positions,
        NativeArray<quaternion> rotations,
        JobHandle dependency = default
    ) {
        ParabolicMoveAndRotationJobFor moveJob = new ParabolicMoveAndRotationJobFor {
            Starts = starts,
            Ends = ends,
            TravelTime = travelTime,
            TimesElapsed = timesElapsed,
            Positions = positions,
            Rotations = rotations
        };
        return moveJob.ScheduleParallel(starts.Length, 128, dependency);
    }
    
    #endregion

    #region Rotational Motion
    
    public static JobHandle InstantRotationFor(
        NativeArray<float3> positions,
        NativeArray<float3> targets, 
        NativeArray<quaternion> results,
        JobHandle dependency = default
    ) {
        InstantRotationJobFor rotateJob = new InstantRotationJobFor {
            Positions = positions,
            Targets = targets,
            Results = results
        };
        return rotateJob.ScheduleParallel(targets.Length, 128, dependency);
    }
    
    public static JobHandle InterpolatedRotationFor(
        NativeArray<float3> positions,
        NativeArray<float3> targets,
        NativeArray<quaternion> rotations,
        NativeArray<float> speeds,
        NativeArray<quaternion> results,
        JobHandle dependency = default
    ) {
        InterpolatedRotationJobFor rotateJob = new InterpolatedRotationJobFor() {
            Positions = positions,
            Targets = targets,
            Rotations = rotations,
            Speeds = speeds,
            DeltaTime = Time.deltaTime,
            Results = results
        };
        return rotateJob.ScheduleParallel(targets.Length, 128, dependency);
    }

    #endregion
}