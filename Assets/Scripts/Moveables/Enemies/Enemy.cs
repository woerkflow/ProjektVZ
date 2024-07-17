using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour {
    
    [Header("Target")]
    public GameObject mainTarget;
    public string enemyTag;
    public float speed;
    
    [Header("Perception")]
    public float perceptionRange;
    
    [Header("Attack")]
    public float attackSpeed;
    public int damage;
    public int maxHealth;

    [Header("Animation")] 
    public string walkParameter;
    public string attackParameter;
    public string dieParameter;
    
    // GameObject
    private int _currentHealth;
    private Animator _animator;
    private float _attackCountDown;
    private float _capsuleRadius;
    private ObjectPool<Enemy> _pool;
    private float _deadCounter;
    
    // Target
    private GameObject _target;
    private SwarmManager _swarmManager;
    private Building _targetBuildingComponent;
    private float _targetCapsuleRadius;
    private Vector3 _targetPosition;
    private PlayerManager _playerManager;
    
    #region Unity Methods
    
    void Start() {
        
        //Initialize player
        _playerManager = PlayerManager.Instance;
        
        // Initialize enemy values
        _attackCountDown = 0f;
        _currentHealth = maxHealth;
        _animator = GetComponentInChildren<Animator>();
        _capsuleRadius = GetComponent<CapsuleCollider>().radius;
        
        // Initialize main target
        SetTarget(mainTarget);
    }
    
    void Update() {
        
        // If zombie is really dead...
        if (_currentHealth <= 0) {
            
            if (_deadCounter > 0) {
                _deadCounter -= Time.deltaTime;
                return;
            }
            
            // Reward player
            _playerManager.SetResourceWhiskey(
                _playerManager.GetResourceWhiskey() + 1
            );
            DestroyEnemy();
            return;
        }

        // If zombie has no target stop...
        if (_target == null) {
            _animator.SetFloat(walkParameter, 0f);
            _target = mainTarget;
            return;
        }
        Vector3 direction = _targetPosition - new Vector3(transform.position.x, 0f, transform.position.z);
        RotateToTarget(direction);
        
        // If distance is greater than the sum of both hit box radii...
        if (direction.magnitude > _targetCapsuleRadius + _capsuleRadius) {
            
            // Start walking animation
            _animator.SetFloat(walkParameter, 1f, 0.3f, Time.deltaTime);
            
            // Translate zombie
            MoveToTarget(direction);
        
        // If distance is not greater than the sum of both hit box radii...
        } else if (_target != mainTarget) {
            
            // Stop walking animation
            _animator.SetFloat(walkParameter, 0f);
            
            // Start attack target
            AttackTarget();
        } else {
            
            // Stop walking animation
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
            _swarmManager.Leave(this);
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
            
            // Kill zombie
            gameObject.tag = "ZombieDead";
            _animator.SetTrigger(dieParameter);
            _deadCounter = 2f;
        }
    }
    
    public GameObject GetTarget() {
        return _target;
    }
    
    public void SetTarget(GameObject newTarget) {
        _target = newTarget;
        _targetBuildingComponent = _target.GetComponent<Building>();
        CapsuleCollider targetCapsuleCollider = _target.GetComponent<CapsuleCollider>();
        _targetCapsuleRadius = targetCapsuleCollider.radius;
        _targetPosition = new Vector3(_target.transform.position.x, 0, _target.transform.position.z);
    }

    public void SetSwarmManager(SwarmManager swarmManager) {
        _swarmManager = swarmManager;
    }

    public void ResetValues() {
        SetHealth(maxHealth);
        gameObject.tag = "Zombie";
    }
    
    #endregion
    
    #region Enemy Private Methods
    
    private float DeltaSpeed(float value) => value * Time.deltaTime;
    
    private void MoveToTarget(Vector3 direction) {
        transform.Translate(direction.normalized * DeltaSpeed(speed), Space.World);
    }

    private void RotateToTarget(Vector3 direction) {
        transform.rotation = Quaternion.LookRotation(direction);
    }
    
    private void AttackTarget() {
        if (_attackCountDown > 0) {
            _attackCountDown -= Time.deltaTime;
        } else {
            _animator.SetTrigger(attackParameter);
            
            if (_target != null) {
                Damage(_targetBuildingComponent);
            }
            _attackCountDown = attackSpeed;
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
