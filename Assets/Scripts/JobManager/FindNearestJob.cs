using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct FindNearestJobFor : IJobFor {
    [ReadOnly] public NativeArray<float3> SeekerPositions;
    [ReadOnly] public NativeArray<float3> TargetPositions;
    public NativeArray<float3> NearestTargetPositions;

    public void Execute(int index) {
        float3 seekerPos = SeekerPositions[index];
        float nearestDistSq = float.MaxValue;
        
        for (int i = 0; i < TargetPositions.Length; i++) {
            float3 targetPos = TargetPositions[i];
            float distSq = math.distancesq(seekerPos, targetPos);
            
            if (distSq < nearestDistSq) {
                nearestDistSq = distSq;
                NearestTargetPositions[index] = targetPos;
            }
        }
    }
}