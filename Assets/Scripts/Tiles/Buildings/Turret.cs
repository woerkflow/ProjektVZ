using System;
using UnityEngine;

public class Turret : MonoBehaviour {
    
    [Header("Turret")]
    public Type type;
    public Transform firePoint;
    public float perceptionRange;
    
    public enum Type {
        Launcher,
        Beamer
    }
    
    private GameObject _target;
    private float _elapsedTime;
    private Seeker _seeker;
    
    [Header("Rotation")]
    public Transform partToRotate;
    public float speed;
    
    [HideInInspector] 
    public Vector3 rotateTarget;

    private TurretJobManager _turretJobManager;

    [Header("Projectile")] 
    public GameObject projectilePrefab;
    public float fireRate;
    
    private GameObject _projectile;
    
    
    #region Unity methods
    
    private void Start() {
        _turretJobManager = FindObjectOfType<TurretJobManager>();
        _seeker = FindObjectOfType<Seeker>();

        if (_turretJobManager != null) {
            _turretJobManager.Register(this);
        } else {
            Debug.LogError("TurretJobManager not found in the scene.");
        }
        
        if (_seeker != null) {
            _seeker.RegisterTurret(this);
        } else {
            Debug.LogError("Seeker not found in the scene.");
        }
    }

    private void Update() {
        
        if (_target == null 
            || !_target.activeSelf
            || Vector3.Distance(_target.transform.position, transform.position) > perceptionRange
        ) {
            _elapsedTime = 0f;
            rotateTarget = partToRotate.position;
            _target = null;
            return;
        }
        rotateTarget = _target.transform.position;
        
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
        _seeker.UnregisterTurret(this);
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
