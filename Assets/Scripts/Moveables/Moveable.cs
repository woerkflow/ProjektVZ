using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Moveable {
    
    public const float Gravity = 0.1f;

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
    
    
    #region Helper Functions
    
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

    public static bool IsEqualPosition(Vector3 building, float3 position) 
        => Mathf.Approximately(position.x, building.x) 
           && Mathf.Approximately(position.z, building.z);
    
    #endregion
}