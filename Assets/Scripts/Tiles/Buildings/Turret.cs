using System;
using UnityEngine;

public class Turret : Seeker {
    
    [Header("Turret")]
    public Type type;
    public Transform firePoint;
    
    public enum Type {
        Launcher,
        Beamer
    }
    
    private float _fireCountDown;
    
    [Header("Rotation")]
    public Transform partToRotate;
    public float turnSpeed;

    [Header("Projectile")] 
    public GameObject projectilePrefab;
    public float fireRate;
    
    private GameObject _projectile;
    
    
    #region Unity methods
    
    private void Start() {
        
        // Calculate maxColliders by perception range
        HitColliders = new Collider[GetMaxColliders(perceptionRange)];
        
        // Start coroutines
        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
        InvokeRepeating(nameof(Shoot), 0f, 0.5f);
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void Shoot() {
        
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
            _fireCountDown -= 0.5f;
            return;
        }
        CreateProjectile();
    }
    
    private void LockOnTarget() {
        Vector3 direction = Target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
    
    private void CreateProjectile() {
        _projectile = Instantiate(projectilePrefab);
        
        switch(type) {
            case Type.Launcher:
                _projectile.GetComponent<Bullet>().Seek(firePoint, Target);
                break;
            case Type.Beamer:
                _projectile.GetComponent<Laser>().Seek(firePoint, Target);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}
