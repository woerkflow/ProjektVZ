using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {
    
    [Header("Turret")]
    public Transform firePoint;
    public float perceptionRange;
    
    private readonly List<Enemy> _targets = new();
    private Enemy _target;
    private float _elapsedTime;
    private SphereCollider _triggerCollider;
    private Coroutine _updateTargetCoroutine;
    private bool _isDestroyed;

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
        _triggerCollider = gameObject.AddComponent<SphereCollider>();
        _triggerCollider.isTrigger = true;
        _triggerCollider.radius = perceptionRange;
        
        _isDestroyed = false;
        _updateTargetCoroutine = StartCoroutine(UpdateTargetRoutine());
        
        InitializeManagers();
    }

    private void Update() {
        
        if (!HasValidTarget()) {
            ResetTimer();
            ResetTurret();
            return;
        }
        IncreaseTimer();
        RotateTurretTowardsTarget();

        if (!CanFireProjectile()) {
            return;
        }
        ResetTimer();
        FireProjectile();
    }
    
    private void OnTriggerEnter(Collider trigger) {
        Enemy enemy = trigger?.GetComponent<Enemy>();
        _targets.Add(enemy);
    }
    
    private void OnTriggerExit(Collider trigger) {
        Enemy enemy = trigger?.GetComponent<Enemy>();
        _targets.Remove(enemy);
    }

    private void OnDestroy() {
        _turretJobManager?.Unregister(this);
        _isDestroyed = true;

        if (_updateTargetCoroutine != null) {
            StopCoroutine(_updateTargetCoroutine);
        }
    }

    #endregion
    
    
    #region Behaviour Methods
    
    private void UpdateTarget() {
        
        _targets.RemoveAll(target 
            => !target
               || !target.gameObject.activeSelf 
               || !target.CompareTag("Zombie") 
               || !(Vector3.Distance(target.transform.position, transform.position) <= perceptionRange)
        );

        if (_targets.Count <= 0) {
            _target = null;
            return;
        }
        List<Enemy> injured = _targets.FindAll(target => target.currentHealth < target.maxHealth);
        _target = injured.Count <= 0 ? _targets[0] : injured[0];
    }
    
    private IEnumerator UpdateTargetRoutine() {
        
        while (!_isDestroyed) {
            yield return new WaitForSeconds(1f);
            UpdateTarget();
        }
    }
    
    #endregion

    
    #region Private Methods

    private void InitializeManagers() {
        _turretJobManager = FindObjectOfType<TurretJobManager>();

        if (_turretJobManager) {
            _turretJobManager.Register(this);
        } else {
            Debug.LogError("TurretJobManager not found in the scene.");
        }
    }

    private bool HasValidTarget() 
        => _target 
           && _target.gameObject.activeSelf
           && _target.CompareTag("Zombie") 
           && Vector3.Distance(_target.transform.position, transform.position) <= perceptionRange;

    private void ResetTurret() {
        rotateTarget = partToRotate.position;
        _target = null;
    }

    private void IncreaseTimer() {
        _elapsedTime += Time.deltaTime;
    }

    private void ResetTimer() {
        _elapsedTime = 0f;
    }

    private void RotateTurretTowardsTarget() {
        rotateTarget = _target.transform.position;
    }

    private bool CanFireProjectile() {
        return _elapsedTime >= fireRate;
    }

    private void FireProjectile() {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.GetComponent<ILaunchable>().Launch(firePoint, _target.gameObject);
    }

    #endregion
}