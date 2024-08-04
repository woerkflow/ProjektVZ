using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

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
    
    public static Vector3 Direction(Vector3 target, Vector3 transform) => 
        new (target.x - transform.x, 0f, target.z - transform.z);
    
    public static Vector3 GetRandomPosition(Transform transform) =>
        transform.position + Points[Random.Range(0, Points.Length)] * 0.001f;
    
    #endregion
    
    #region Rectlinear Motion
    
    public static void LinearMoveFor(
        NativeArray<Vector3> directions,
        NativeArray<float> speeds,
        NativeArray<Vector3> currentPositions,
        NativeArray<Vector3> results
    ) {
        LinearMoveJobFor moveJob = new LinearMoveJobFor {
            Directions = directions,
            Speeds = speeds,
            CurrentPositions = currentPositions,
            DeltaTime = Time.deltaTime,
            Results = results
        };
        JobHandle moveHandle = moveJob.Schedule(directions.Length, 64);
        moveHandle.Complete();
    }
    
    #endregion
    
    #region Curvilinear Motion
    
    public static Vector3 CalculateControlPoint(Vector3 start, Vector3 end) {
        Vector3 mid = (start + end) * 0.5f;
        return new Vector3(mid.x, start.y, mid.z);
    }
    
    public static void ParabolicMoveFor(
        NativeArray<Vector3> starts, 
        NativeArray<Vector3> controls, 
        NativeArray<Vector3> ends,
        NativeArray<float> ts, 
        NativeArray<Vector3> results
    ) {
        ParabolicMoveJobFor moveJob = new ParabolicMoveJobFor {
            Starts = starts,
            Controls = controls,
            Ends = ends,
            Ts = ts,
            Results = results
        };
        JobHandle moveHandle = moveJob.Schedule(starts.Length, 64);
        moveHandle.Complete();
    }
    
    #endregion

    #region Rotational Motion
    
    public static void InstantRotationFor(
        NativeArray<Vector3> directions, 
        NativeArray<Quaternion> results
    ) {
        InstantRotationJobFor rotateJob = new InstantRotationJobFor {
            Directions = directions,
            Results = results
        };
        JobHandle rotateHandle = rotateJob.Schedule(directions.Length, 64);
        rotateHandle.Complete();
    }
    
    public static void InterpolatedRotationFor(
        NativeArray<Vector3> directions,
        NativeArray<Quaternion> rotations,
        NativeArray<float> speeds,
        NativeArray<Quaternion> results
    ) {
        InterpolatedRotationJobFor rotateJob = new InterpolatedRotationJobFor() {
            Directions = directions,
            Rotations = rotations,
            Speeds = speeds,
            Results = results
        };
        JobHandle rotateHandle = rotateJob.Schedule(directions.Length, 64);
        rotateHandle.Complete();
    }

    #endregion
}