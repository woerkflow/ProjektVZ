using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Moveable {
    
    private const float Gravity = 0.1f;

    private static readonly Vector3[] Points = {
        new (-1,0,-1), 
        Vector3.left,
        new (-1,0,1), 
        Vector3.back,
        Vector3.zero, 
        Vector3.forward,
        new (1,0,-1), 
        Vector3.right,
        new (1,0,1)
    };
    
    
    #region Helper
    
    public static Vector3 Direction(Vector3 target, Vector3 transform) 
        => new (target.x - transform.x, 0f, target.z - transform.z);
    
    public static Vector3 GetRandomPosition(Transform transform) 
        => transform.position + Points[Random.Range(0, Points.Length)];
    
    public static float GetDistance(Vector3 target, Vector3 gameObject)
        => Vector3.Distance(
            new Vector3(target.x, 0f, target.z),
            new Vector3(gameObject.x, 0f, gameObject.z)
        );
    
    public static float GetTravelTime(Vector3 start, Vector3 end)
        => Mathf.Sqrt((2 * (start.y - end.y)) / Gravity);
    
    #endregion
    
    
    #region Curvilinear Motion

    public static JobHandle CurvedMoveFor(
        NativeArray<float3> positions,
        NativeArray<float3> targets,
        NativeArray<float> speeds,
        NativeArray<float3> positionResults,
        NativeArray<quaternion> rotationResults,
        JobHandle dependency = default
    ) {
        return new CurvedMoveJobFor {
            Positions = positions,
            Targets = targets,
            Speeds = speeds,
            DeltaTime = Time.deltaTime,
            PositionResults = positionResults,
            RotationResults = rotationResults
        }.ScheduleParallel(positions.Length, 128, dependency);
    }
    
    public static JobHandle ParabolicMoveAndRotationFor(
        NativeArray<float3> starts, 
        NativeArray<float3> ends,
        NativeArray<float> travelTime,
        NativeArray<float> timesElapsed,
        NativeArray<float3> positions,
        NativeArray<quaternion> rotations,
        JobHandle dependency = default
    ) {
        return new ParabolicMoveAndRotationJobFor {
            Starts = starts,
            Ends = ends,
            TravelTime = travelTime,
            TimesElapsed = timesElapsed,
            Gravity = Gravity,
            Positions = positions,
            Rotations = rotations
        }.ScheduleParallel(starts.Length, 128, dependency);
    }
    
    #endregion
    

    #region Rotational Motion
    
    public static JobHandle InterpolatedRotationFor(
        NativeArray<float3> positions,
        NativeArray<float3> targets,
        NativeArray<quaternion> rotations,
        NativeArray<float> speeds,
        NativeArray<quaternion> results,
        JobHandle dependency = default
    ) {
        return new InterpolatedRotationJobFor() {
            Positions = positions,
            Targets = targets,
            Rotations = rotations,
            Speeds = speeds,
            DeltaTime = Time.deltaTime,
            Results = results
        }.ScheduleParallel(targets.Length, 128, dependency);
    }

    #endregion
}