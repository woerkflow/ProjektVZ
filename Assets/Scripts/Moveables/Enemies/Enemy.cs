using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour {
    
    [Header("Target")]
    public GameObject mainTarget;
    public string enemyTag;
    public float maxSpeed;
    
    [Header("Perception")]
    public float perceptionRange;
    public float turnSpeed;
    
    [Header("Attack")]
    public float attackSpeed;
    public int damage;
    public int maxHealth;

    [Header("Animation")] 
    public string walkParameter;
    public string attackParameter;
    public string dieParameter;
    
    // GameObject
    private float _speed;
    private int _currentHealth;
    private Animator _animator;
    private float _attackCountDown;
    private float _capsuleRadius;
    private ObjectPool<Enemy> _pool;
    private float _deadCounter;
    
    // Target
    public GameObject target { get; set; }
    public SwarmManager swarmManager { private get; set; }
    private Building _targetBuildingComponent;
    private float _targetCapsuleRadius;
    private Vector3 _targetPosition;
    
    #region Unity Methods
    
    void Start() {
        
        // Initialize enemy values
        _attackCountDown = 0f;
        _currentHealth = maxHealth;
        _speed = Random.Range(0.004f, maxSpeed);
        _animator = GetComponentInChildren<Animator>();
        _capsuleRadius = GetComponent<CapsuleCollider>().radius;
        
        // Initialize main target
        SetTarget(mainTarget);
    }
    
    void Update() {
        
        if (_currentHealth <= 0) {
            
            if (_deadCounter > 0) {
                _deadCounter -= Time.deltaTime;
                return;
            }
            DestroyEnemy();
            return;
        }

        if (target == null) {
            _animator.SetFloat(walkParameter, 0f);
            return;
        }
        Vector3 direction = _targetPosition - new Vector3(transform.position.x, 0f, transform.position.z);
        RotateToTarget(direction);
        
        if (direction.magnitude > _targetCapsuleRadius + _capsuleRadius) {
            
            // Start walking animation
            _animator.SetFloat(walkParameter, 1f, 0.3f, Time.deltaTime);
            MoveToTarget(direction);
        } else if (target != mainTarget) {
            
            // Stop walking animation
            _animator.SetFloat(walkParameter, 0f);
            AttackTarget();
        } else {
            _animator.SetFloat(walkParameter, 0f);
        }
    }
    
    #endregion
    
    #region Object Pooling

    public void SetPool(ObjectPool<Enemy> pool) {
        _pool = pool;
    }
    
    private void DestroyEnemy() {
        if (_pool != null) {
            _animator.SetFloat(walkParameter, 0f);
            swarmManager.Unsubscribe(this);
            _pool.Release(this);
        } else {
            Destroy(gameObject);
        }
    }
    
    #endregion
    
    #region Public Enemy Methods
    
    public int GetHealth() {
        return _currentHealth;
    }

    public void SetHealth(int value) {
        _currentHealth = value;
        
        if (_currentHealth <= 0) {
            gameObject.tag = "ZombieDead";
            _animator.SetTrigger(dieParameter);
            _deadCounter = 2f;
        }
    }
    
    public void SetTarget(GameObject newTarget) {
        target = newTarget
            ? newTarget
            : mainTarget;
        
        _targetBuildingComponent = target?.GetComponent<Building>();
        
        CapsuleCollider targetCapsuleCollider = target?.GetComponentInChildren<CapsuleCollider>();
        _targetCapsuleRadius = targetCapsuleCollider
            ? targetCapsuleCollider.radius 
            : 0.01f;
        
        _targetPosition = target
            ? new Vector3(target.transform.position.x, 0, target.transform.position.z) 
            : new Vector3(mainTarget.transform.position.x, 0, mainTarget.transform.position.z);
    }

    public void ResetValues() {
        SetHealth(maxHealth);
        gameObject.tag = "Zombie";
    }
    
    #endregion
    
    #region Enemy Private Methods
    
    private float DeltaSpeed(float value) => value * Time.deltaTime;
    
    private void MoveToTarget(Vector3 direction) {
        transform.Translate(direction.normalized * DeltaSpeed(_speed), Space.World);
    }

    private void RotateToTarget(Vector3 direction) {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, DeltaSpeed(turnSpeed)).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
    
    private void AttackTarget() {
        if (_attackCountDown <= 0) {
            _animator.SetTrigger(attackParameter);
            
            if (target != null) {
                Damage(_targetBuildingComponent);
            }
            _attackCountDown = 1f / attackSpeed;
        } else {
            _attackCountDown -= Time.deltaTime;
        }
    }
    
    private void Damage(Building building) {
        building.SetHealth(building.GetHealth() - damage);
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, perceptionRange);
    }
    
    #endregion
}
