using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct LinearMoveJobFor : IJobParallelFor {
    [ReadOnly] public NativeArray<Vector3> Directions;
    [ReadOnly] public NativeArray<float> Speeds;
    [ReadOnly] public NativeArray<Vector3> CurrentPositions;
    [ReadOnly] public float DeltaTime;
    public NativeArray<Vector3> Results;

    public void Execute(int index) {
        Vector3 step = Directions[index].normalized * Speeds[index] * DeltaTime;
        Results[index] = new Vector3(CurrentPositions[index].x + step.x, CurrentPositions[index].y, CurrentPositions[index].z + step.z);
    }
}

[BurstCompile]
public struct ParabolicMoveJobFor : IJobParallelFor {
    [ReadOnly] public NativeArray<Vector3> Starts;
    [ReadOnly] public NativeArray<Vector3> Controls;
    [ReadOnly] public NativeArray<Vector3> Ends;
    [ReadOnly] public NativeArray<float> Ts;
    public NativeArray<Vector3> Results;

    public void Execute(int index) {
        Vector3 startPoint = Vector3.Lerp(Starts[index], Controls[index], Ts[index]);
        Vector3 endPoint = Vector3.Lerp(Controls[index], Ends[index], Ts[index]);
        Results[index] = Vector3.Lerp(startPoint, endPoint, Ts[index]);
    }
}

[BurstCompile]
public struct InstantRotationJobFor : IJobParallelFor {
    [ReadOnly] public NativeArray<Vector3> Directions;
    public NativeArray<Quaternion> Results;

    public void Execute(int index) {
        
        if (Directions[index] == Vector3.zero) {
            Results[index] = Quaternion.identity;
        } else {
            Results[index] = Quaternion.LookRotation(Directions[index]);
        }
    }
}

[BurstCompile]
public struct InterpolatedRotationJobFor : IJobParallelFor {
    [ReadOnly] public NativeArray<Vector3> Directions;
    [ReadOnly] public NativeArray<Quaternion> Rotations;
    [ReadOnly] public NativeArray<float> Speeds;
    public NativeArray<Quaternion> Results;

    public void Execute(int index) {
        
        if (Directions[index] == Vector3.zero) {
            Results[index] = Quaternion.identity;
        } else {
            Quaternion lookRotation = Quaternion.LookRotation(Directions[index]);
            Vector3 rotation = Quaternion.Lerp(Rotations[index], lookRotation, Speeds[index]).eulerAngles;
            Results[index] = Quaternion.Euler(0f, rotation.y, 0f);
        }
    }
}