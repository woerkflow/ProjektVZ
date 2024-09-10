using UnityEngine;

[System.Serializable]
public struct TurretBlueprint {
    public Transform firePoint;
    public float perceptionRange;
    public float fireRate;
    public Transform partToRotate;
    public float rotationSpeed;
    public GameObject projectilePrefab;
}