using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour {
    
    [Header("Target")]
    public GameObject mainTarget;
    public float perceptionRange;
    
    private GameObject _target;
    private Building _targetBuildingComponent;
    private float _targetCapsuleRadius;
    
    [Header("Movement")]
    public float speed;

    [HideInInspector] 
    public Vector3 direction;
    [HideInInspector]
    public float currentSpeed;
    
    private EnemyJobManager _enemyJobManager;
    
    [Header("Attack")]
    public float attackSpeed;
    public int damage;
    public int maxHealth;
    public CapsuleCollider capsuleCollider;
    public float deadTime;
    
    private float _capsuleRadius;
    private float _elapsedAttackTime;
    private int _currentHealth;
    private float _elapsedDeadTime;

    [Header("Animation")] 
    public Animator animator;
    public string walkParameter;
    public string attackParameter;
    public string dieParameter;
    
    private ObjectPool<Enemy> _pool;
    private SwarmManager _swarmManager;
    private PlayerManager _playerManager;
    
    
    #region Unity Methods
    
    private void Start() {
        _enemyJobManager = FindObjectOfType<EnemyJobManager>();
        
        if (_enemyJobManager != null) {
            _enemyJobManager.Register(this);
        } else {
            Debug.LogError("EnemyJobManager not found in the scene.");
        }
        _playerManager = PlayerManager.Instance;
        _elapsedAttackTime = attackSpeed;
        _currentHealth = maxHealth;
        _elapsedDeadTime = 0f;
        _capsuleRadius = capsuleCollider.radius;
        SetTarget(mainTarget);
    }
    
    private void Update() {
        
        if (_currentHealth <= 0) {
            
            if (_elapsedDeadTime < deadTime) {
                _elapsedDeadTime += Time.deltaTime;
                return;
            }
            _playerManager.SetResourceWhiskey(
                _playerManager.GetResourceWhiskey() + 1
            );
            DestroyEnemy();
            return;
        }
        
        if (_target == null) {
            _target = mainTarget;
            return;
        }
        direction = Moveable.Direction(
            _target.transform.position, 
            transform.position
        );
        
        if (direction.magnitude > _targetCapsuleRadius + _capsuleRadius) {
            animator.SetFloat(walkParameter, 1f, 0.3f, Time.deltaTime);
            currentSpeed = speed;
            return;
        }
        
        if (_target != mainTarget) {
            animator.SetFloat(walkParameter, 0f);
            currentSpeed = 0f;
            AttackTarget();
            return;
        }
        animator.SetTrigger(dieParameter);
        DeactivateValues();
        DestroyEnemy();
    }
    
    private void OnDestroy() {
        _enemyJobManager.Unregister(this);
    }
    
    #endregion
    
    
    #region Object Pooling

    public void SetPool(ObjectPool<Enemy> pool) {
        _pool = pool;
    }
    
    private void DestroyEnemy() {
        
        if (_pool != null) {
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

        if (_currentHealth > 0) {
            return;
        }
        animator.SetTrigger(dieParameter);
        DeactivateValues();
    }
    
    public GameObject GetTarget() {
        return _target;
    }
    
    public void SetTarget(GameObject newTarget) {
        _target = newTarget;
        _targetBuildingComponent = _target.GetComponent<Building>();
        _targetCapsuleRadius = _target.GetComponent<CapsuleCollider>().radius;
    }

    public void SetSwarmManager(SwarmManager swarmManager) {
        _swarmManager = swarmManager;
    }

    public void ResetValues() {
        SetHealth(maxHealth);
        capsuleCollider.enabled = true;
        gameObject.tag = "Zombie";
        currentSpeed = speed;
    }
    
    #endregion
    
    
    #region Private Enemy Methods
    
    private void AttackTarget() {
        
        if (_elapsedAttackTime < attackSpeed) {
            _elapsedAttackTime += Time.deltaTime;
            return;
        }
        animator.SetTrigger(attackParameter);
            
        if (_target != null) {
            Damage(_targetBuildingComponent, damage);
        }
        _elapsedAttackTime = 0f;
    }
    
    private static void Damage(Building building, int damage) {
        building.SetHealth(building.GetHealth() - damage);
    }
    
    private void DeactivateValues() {
        capsuleCollider.enabled = false;
        gameObject.tag = "ZombieDead";
        currentSpeed = 0f;
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, perceptionRange);
    }
    
    #endregion
}
