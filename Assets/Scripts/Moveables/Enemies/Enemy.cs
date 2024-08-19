using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour {
    
    [Header("Target")]
    public GameObject mainTarget;
    public float perceptionRange;
    
    public GameObject target { get; private set; }
    
    private Building _targetBuildingComponent;
    private float _targetCapsuleRadius;
    
    [Header("Movement")]
    public float speed;
    
    public float currentSpeed { get; private set; }
    public Vector3 moveTarget { get; private set; }
    
    private SpawnJobManager _spawnJobManager;
    
    [Header("Attack")]
    public float attackSpeed;
    public int minDamage;
    public int maxDamage;
    public int maxHealth;
    public CapsuleCollider capsuleCollider;
    public float deadTime;
    
    public int currentHealth { get; private set; }
    
    private float _capsuleRadius;
    private float _elapsedAttackTime;
    private float _elapsedDeadTime;

    [Header("Animation")] 
    public Animator animator;
    public string walkParameter;
    public string attackParameter;
    public string dieParameter;
    
    private SwarmManager _swarmManager;
    private PlayerManager _playerManager;
    private EnemyPoolManager _enemyPoolManager;
    
    #region Unity Methoden
    
    private void Start() {
        InitializeManagers();
        _capsuleRadius = capsuleCollider.radius;
        target = mainTarget;
        ResetValues();
    }
    
    private void Update() {
        
        if (currentHealth <= 0) {
            HandleDeath();
            return;
        }
        UpdateTarget();
        MoveTowardsTarget();

        if (IsWithinAttackRange()) {
            HandleAttack();
        }
    }

    private void OnDestroy() {
        _spawnJobManager?.UnregisterEnemy(this);
    }
    
    #endregion
    
    
    #region Initialization

    private void InitializeManagers() {
        _playerManager = FindObjectOfType<PlayerManager>();
        _enemyPoolManager = FindObjectOfType<EnemyPoolManager>();
        _spawnJobManager = FindObjectOfType<SpawnJobManager>();
        
        if (!_spawnJobManager) {
            Debug.LogError("SpawnJobManager not found in the scene.");
            return;
        }
        _spawnJobManager.RegisterEnemy(this);
    }
    
    #endregion
    
    
    #region Object Pooling
    
    private void DestroyEnemy() {
        _swarmManager?.Unregister(this);
        
        if (_enemyPoolManager) {
            _enemyPoolManager.ReturnEnemyToPool(this);
        } else {
            Destroy(gameObject);
        }
    }
    
    #endregion
    
    
    #region Public Methods

    public void TakeDamage(int value) {
        currentHealth -= value;

        if (currentHealth > 0) { 
            return;
        }
        animator.SetTrigger(dieParameter);
        DeactivateValues();
    }

    public void SetTarget(Building newTarget) {
        target = newTarget.gameObject;
        _targetBuildingComponent = newTarget;
        _targetCapsuleRadius = target.GetComponent<CapsuleCollider>().radius;
    }

    public void SetSwarmManager(SwarmManager swarmManager) {
        _swarmManager = swarmManager;
    }

    public void ResetValues() {
        currentHealth = maxHealth;
        _elapsedAttackTime = attackSpeed;
        _elapsedDeadTime = 0f;
        capsuleCollider.enabled = true;
        gameObject.tag = "Zombie";
        currentSpeed = speed;
    }
    
    #endregion
    
    
    #region Private Methods
    
    private void HandleDeath() {
        
        if (_elapsedDeadTime < deadTime) {
            _elapsedDeadTime += Time.deltaTime;
        } else {
            _playerManager.AddResources( 
                new Resources {
                    whiskey = 1    
                }
            );
            DestroyEnemy();
        }
    }

    private void UpdateTarget() {
        
        if (!target) {
            target = mainTarget;
        }
        moveTarget = target.transform.position;
    }

    private void MoveTowardsTarget() {
        Vector3 direction = Moveable.Direction(moveTarget, transform.position);

        if (direction.magnitude > _targetCapsuleRadius + _capsuleRadius) {
            animator.SetFloat(walkParameter, 1f, 0.1f, Time.deltaTime);
            currentSpeed = speed;
        } else {
            currentSpeed = 0f;
        }
    }

    private bool IsWithinAttackRange() 
        => currentSpeed == 0f && target != mainTarget;

    private void HandleAttack() {
        animator.SetFloat(walkParameter, 0f);
        
        if (_elapsedAttackTime >= attackSpeed) {
            animator.SetTrigger(attackParameter);
            
            if (target) {
                _targetBuildingComponent?.TakeDamage(Random.Range(minDamage, maxDamage));
            }
            _elapsedAttackTime = 0f;
        } else {
            _elapsedAttackTime += Time.deltaTime;
        }
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