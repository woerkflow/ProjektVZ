using UnityEngine;

public class Turret : Seeker {
    
    [Header("Turret")]
    public float fireRate;
    public Transform firePoint;
    
    [Header("Rotation")]
    public Transform partToRotate;
    public float turnSpeed;
    
    protected float FireCountDown;
    
    
    #region Private class methods
    
    protected void LockOnTarget() {
        Vector3 direction = Target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    #endregion
}
