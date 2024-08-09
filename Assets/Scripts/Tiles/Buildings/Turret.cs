using System;
using UnityEngine;

public class Turret : MonoBehaviour, ITargetable {
    
    [Header("Turret")]
    public TurretType type;
    public Transform firePoint;
    public float perceptionRange;

    public enum TurretType {
        Launcher,
        Beamer
    }

    private GameObject _target;
    private float _elapsedTime;
    private Seeker _seeker;

    [Header("Rotation")]
    public Transform partToRotate;
    public float rotationSpeed;
    
    public Vector3 rotateTarget { get; private set; }

    private TurretJobManager _turretJobManager;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float fireRate;

    
    #region Unity Methods

    private void Start() {
        InitializeManagers();
    }

    private void Update() {
        
        if (!HasValidTarget()) {
            ResetTurret();
            return;
        }
        IncreaseTimer();
        RotateTurretTowardsTarget();

        if (CanFireProjectile()) {
            FireProjectile();
        }
    }

    private void OnDestroy() {
        _turretJobManager?.Unregister(this);
        _seeker?.Unregister(this);
    }

    #endregion

    
    #region Public Methods

    public GameObject GetTarget() => _target;

    public void SetTarget(GameObject target) {
        _target = target;
    }

    #endregion

    
    #region Private Methods

    private void InitializeManagers() {
        _turretJobManager = FindObjectOfType<TurretJobManager>();
        _seeker = FindObjectOfType<Seeker>();

        if (_turretJobManager != null) {
            _turretJobManager.Register(this);
        } else {
            Debug.LogError("TurretJobManager not found in the scene.");
        }

        if (_seeker != null) {
            _seeker.Register(this);
        } else {
            Debug.LogError("Seeker not found in the scene.");
        }
    }

    private bool HasValidTarget() {
        return _target != null 
               && _target.activeSelf 
               && Vector3.Distance(_target.transform.position, transform.position) <= perceptionRange;
    }

    private void ResetTurret() {
        _elapsedTime = 0f;
        rotateTarget = partToRotate.position;
        _target = null;
    }

    private void IncreaseTimer() {
        _elapsedTime += Time.deltaTime;
    }

    private void RotateTurretTowardsTarget() {
        rotateTarget = _target.transform.position;
    }

    private bool CanFireProjectile() {
        return _elapsedTime >= fireRate;
    }

    private void FireProjectile() {
        _elapsedTime = 0f;
        GameObject projectile = Instantiate(projectilePrefab);

        switch (type) {
            case TurretType.Launcher:
                projectile.GetComponent<Bullet>().Seek(firePoint, _target);
                break;
            case TurretType.Beamer:
                projectile.GetComponent<Laser>().Seek(firePoint, _target);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}