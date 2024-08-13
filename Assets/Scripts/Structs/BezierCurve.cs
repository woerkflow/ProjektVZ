using UnityEngine;

[System.Serializable]
public struct BezierCurve {
    public Vector3 start;
    public Vector3 control;
    public Vector3 end;
    public float t;
}
