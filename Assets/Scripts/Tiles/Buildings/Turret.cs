using UnityEngine;

public class Turret : Seeker {
    
    [Header("Turret")]
    public Type type;
    public Transform firePoint;
    
    [Header("Rotation")]
    public Transform partToRotate;
    public float turnSpeed;

    [Header("Projectile")] 
    public GameObject projectilePrefab;
    public float fireRate;

    private float _fireCountDown;
    private GameObject _projectile;

    public enum Type {
        Launcher,
        Beamer
    }
    
    
    #region Unity methods
    
    private void Update() {
        
        if (Target == null || !Target.CompareTag(enemyTag)) {
            _fireCountDown = fireRate;
            return;
        }
        LockOnTarget();
        
        if (_projectile != null) {
            _fireCountDown = fireRate;
            return;
        }

        if (_fireCountDown > 0f) {
            _fireCountDown -= Time.deltaTime;
            return;
        }
        Shoot();
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void Shoot() {
        _projectile = Instantiate(projectilePrefab);
        
        switch(type) {
            case Type.Launcher:
                _projectile.GetComponent<Bullet>().Seek(firePoint, Target);
                break;
            case Type.Beamer:
                _projectile.GetComponent<Laser>().Seek(firePoint, Target);
                break;
        }
    }
    
    private void LockOnTarget() {
        Vector3 direction = Target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    #endregion
}
