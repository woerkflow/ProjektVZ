using System;
using UnityEngine;

public class Turret : MonoBehaviour {
    
    [Header("Turret")]
    public Type type;
    public Transform firePoint;
    public string enemyTag;
    public float perceptionRange;
    
    public enum Type {
        Launcher,
        Beamer
    }
    
    private float _elapsedTime;
    public GameObject _target;
    
    [Header("Rotation")]
    public Transform partToRotate;
    public float speed;

    [HideInInspector] 
    public Vector3 direction;

    private TurretJobManager _turretJobManager;

    [Header("Projectile")] 
    public GameObject projectilePrefab;
    public float fireRate;
    
    private GameObject _projectile;
    
    
    #region Unity methods
    
    private void Start() {
        _turretJobManager = FindObjectOfType<TurretJobManager>();

        if (_turretJobManager != null) {
            _turretJobManager.Register(this);
        } else {
            Debug.LogError("TurretJobManager not found in the scene.");
        }
    }

    private void Update() {
        
        if (_target == null 
            || !_target.activeSelf
            || Vector3.Distance(_target.transform.position, transform.position) > perceptionRange
        ) {
            _elapsedTime = 0f;
            _target = null;
            return;
        }
        direction = Moveable.Direction(
            _target.transform.position, 
            partToRotate.position
        );

        if (direction == Vector3.zero) {
            return;
        }
        
        if (_projectile != null) {
            _elapsedTime = 0f;
            return;
        }

        if (_elapsedTime < fireRate) {
            _elapsedTime += Time.deltaTime;
            return;
        }
        CreateProjectile();
    }
    
    private void OnDestroy() {
        _turretJobManager.Unregister(this);
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetTarget(GameObject target) {
        _target = target;
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void CreateProjectile() {
        _projectile = Instantiate(projectilePrefab);
        
        switch(type) {
            case Type.Launcher:
                _projectile.GetComponent<Bullet>().Seek(firePoint, _target);
                break;
            case Type.Beamer:
                _projectile.GetComponent<Laser>().Seek(firePoint, _target);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}
