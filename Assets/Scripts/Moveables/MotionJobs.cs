using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct LinearMoveJobFor : IJobFor {
    [ReadOnly] public NativeArray<float3> Positions;
    [ReadOnly] public NativeArray<float3> Targets;
    [ReadOnly] public NativeArray<float> Speeds;
    [ReadOnly] public float DeltaTime;
    public NativeArray<float3> Results;

    public void Execute(int index) {
        float3 target = Targets[index];
        float3 position = Positions[index];
        float speed = Speeds[index];
        
        float3 direction = new float3(target.x - position.x, 0, target.z - position.z);
        float lenSq = math.lengthsq(direction);
        
        if (lenSq > 1e-5f) {
            direction = math.normalize(direction);
            Results[index] = position + direction * speed * DeltaTime;
        } else {
            Results[index] = position;
        }
    }
}

[BurstCompile]
public struct ParabolicMoveJobFor : IJobFor {
    [ReadOnly] public NativeArray<float3> Starts;
    [ReadOnly] public NativeArray<float3> Controls;
    [ReadOnly] public NativeArray<float3> Ends;
    [ReadOnly] public NativeArray<float> Ts;
    public NativeArray<float3> Results;

    public void Execute(int index) {
        float3 start = Starts[index];
        float3 control = Controls[index];
        float3 end = Ends[index];
        float t = Ts[index];

        float3 startPoint = math.lerp(start, control, t);
        float3 endPoint = math.lerp(control, end, t);
        Results[index] = math.lerp(startPoint, endPoint, t);
    }
}

[BurstCompile]
public struct ParabolicMoveAndRotationJobFor : IJobFor {
    [ReadOnly] public NativeArray<float3> Starts;
    [ReadOnly] public NativeArray<float3> Ends;
    [ReadOnly] public NativeArray<float> TravelTime;
    [ReadOnly] public NativeArray<float> TimesElapsed;
    public NativeArray<float3> Positions;
    public NativeArray<quaternion> Rotations;

    public void Execute(int index) {
        float3 start = Starts[index];
        float3 end = Ends[index];
        float travelTime = TravelTime[index];
        float timeElapsed = TimesElapsed[index];
        float gravity = 0.04f;

        // Berechnung der initialen Geschwindigkeit
        float3 displacement = end - start;
        float3 initialVelocity = new float3(
            displacement.x / travelTime, 
            (displacement.y / travelTime) + 0.5f * gravity * travelTime, 
            displacement.z / travelTime
        );

        // Berechnung der aktuellen Position
        float t = timeElapsed / travelTime;
        Positions[index] = start + initialVelocity * t + 0.5f * new float3(0, -gravity, 0) * (t * t);
        
        // Berechnung der Geschwindigkeit für die Rotation
        float3 velocity = initialVelocity + new float3(0, -gravity * t, 0);

        // Setzen der Rotation des Projektils basierend auf der Geschwindigkeit
        Rotations[index] = quaternion.LookRotation(math.normalize(velocity), math.up());
    }
}

[BurstCompile]
public struct InstantRotationJobFor : IJobFor {
    [ReadOnly] public NativeArray<float3> Positions;
    [ReadOnly] public NativeArray<float3> Targets;
    public NativeArray<quaternion> Results;

    public void Execute(int index) {
        float3 target = Targets[index];
        float3 position = Positions[index];
        
        float3 direction = new float3(target.x - position.x, 0, target.z - position.z);
        float lenSq = math.lengthsq(direction);
        
        if (lenSq > 1e-5f) {
            Results[index] = quaternion.LookRotationSafe(direction, math.up()) ;
        } else {
            Results[index] = quaternion.identity;
        }
    }
}

[BurstCompile]
public struct InterpolatedRotationJobFor : IJobFor {
    [ReadOnly] public NativeArray<float3> Positions;
    [ReadOnly] public NativeArray<float3> Targets;
    [ReadOnly] public NativeArray<quaternion> Rotations;
    [ReadOnly] public NativeArray<float> Speeds;
    [ReadOnly] public float DeltaTime;
    public NativeArray<quaternion> Results;

    public void Execute(int index) {
        float3 target = Targets[index];
        float3 position = Positions[index];
        float speed = Speeds[index];
        quaternion rotation = Rotations[index];
        
        float3 direction = new float3(target.x - position.x, 0, target.z - position.z);
        float lenSq = math.lengthsq(direction);
        
        if (lenSq > 1e-5f) {
            quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());
            Results[index] = math.slerp(rotation, targetRotation, speed * DeltaTime);
        } else {
            Results[index] = rotation;
        }
    }
}