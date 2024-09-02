using UnityEngine;

public class Turret : MonoBehaviour {
    
    [Header("Turret")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float perceptionRange;
    
    private Enemy _target;
    private float _elapsedTime;
    private SphereCollider _triggerCollider;
    private Coroutine _updateTargetCoroutine;
    
    [Header("Rotation")]
    [SerializeField] private Transform partToRotate;
    [SerializeField] private float rotationSpeed;
    
    public Vector3 rotateTarget { get; private set; }
    
    private JobSystemManager _jobSystemManager;
    
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate;
    
    
    #region Unity Methods
    
    private void Start() {
        InitializeManagers();
    }
    
    private void Update() {
        
        if (HasNotValidTarget()) {
            ResetTimer();
            ResetTurret();
            ResetTarget();
            return;
        }
        IncreaseTimer();
        RotateTurretTowardsTarget();

        if (!CanFireProjectile()) {
            return;
        }
        ResetTimer();
        FireProjectile();
        ResetTarget();
    }
    
    private void OnDestroy() {
        _jobSystemManager?.UnregisterTurret(this);
    }
    
    #endregion
    
    
    #region Public Methods
    
    public Transform GetPartToRotate() => partToRotate;
    
    public float GetRotationSpeed() => rotationSpeed;
    
    public void SetTarget(Enemy target) {
        _target = target;
    }
    
    #endregion
    
    
    #region Private Methods
    
    private void InitializeManagers() {
        _jobSystemManager = FindObjectOfType<JobSystemManager>();
        _jobSystemManager?.RegisterTurret(this);
    }

    private bool HasNotValidTarget()
        => !_target
           || _target.gameObject.activeSelf == false
           || !(Moveable.GetDistance(_target.transform.position, firePoint.transform.position) <= perceptionRange);

    private void ResetTarget() {
        _target = null;
    }
    
    private void ResetTurret() {
        rotateTarget = partToRotate.position;
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
    
    private bool CanFireProjectile() => _elapsedTime >= fireRate;
    
    private void FireProjectile() {
        GameObject projectile = Instantiate(projectilePrefab);
        projectile.GetComponent<ILaunchable>().Launch(firePoint, _target.gameObject);
    }
    
    #endregion
}