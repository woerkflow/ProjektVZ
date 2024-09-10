using UnityEngine;

[System.Serializable]
public struct LaserBlueprint {
    public LineRenderer lineRenderer;
    public  GameObject effectFirePoint;
    public  GameObject effectTarget;
    public  GameObject effectArea;
    public int damage;
    public int duration;
    public int tickTime;
}
