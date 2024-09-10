using UnityEngine;

public class Turret : MonoBehaviour {
    
    [SerializeField] private TurretBlueprint blueprint;
    
    public Vector3 rotateTarget { get; private set; }
    
    private JobSystemManager _jobSystemManager;
    private Enemy _target;
    private float _elapsedTime;
    private SphereCollider _triggerCollider;
    private Coroutine _updateTargetCoroutine;
    
    
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
    
    public Transform GetPartToRotate() => blueprint.partToRotate;
    
    public float GetRotationSpeed() => blueprint.rotationSpeed;
    
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
           || !(Moveable.GetDistance(_target.transform.position, blueprint.firePoint.transform.position) <= blueprint.perceptionRange);

    private void ResetTarget() {
        _target = null;
    }
    
    private void ResetTurret() {
        rotateTarget = blueprint.partToRotate.position;
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
    
    private bool CanFireProjectile() => _elapsedTime >= blueprint.fireRate;
    
    private void FireProjectile() {
        GameObject projectile = Instantiate(blueprint.projectilePrefab);
        projectile.GetComponent<ILaunchable>().Launch(blueprint.firePoint, _target.gameObject);
    }
    
    #endregion
}