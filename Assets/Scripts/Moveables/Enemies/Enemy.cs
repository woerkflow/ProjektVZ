using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour {
    
    [Header("Target")]
    [SerializeField] private GameObject mainTarget;
    
    private GameObject _target;
    private Building _targetBuildingComponent;
    private float _targetCapsuleRadius;
    private JobSystemManager _jobSystemManager;
    
    [Header("Movement")]
    [SerializeField] private float speed;
    
    public float currentSpeed { get; private set; }
    public Vector3 moveTarget { get; private set; }
    
    [Header("Attack")]
    [SerializeField] private float attackSpeed;
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private CapsuleCollider capsuleCollider;
    
    private float _capsuleRadius;
    private float _elapsedAttackTime;
    
    [Header("Animation")] 
    [SerializeField] private Animator animator;
    [SerializeField] private string walkParameter;
    [SerializeField] private string attackParameter;
    [SerializeField] private string dieParameter;
    
    private EnemyPoolManager _enemyPoolManager;
    
    [Header("Death")]
    [SerializeField] private int maxHealth;
    [SerializeField] private float deadTime;
    [SerializeField] private int lootAmount;
    [SerializeField] private Loot[] loots = new Loot[3];
    
    public int currentHealth { get; private set; }
    
    private float _elapsedDeadTime;
    
    
    #region Unity Methods
    
    private void Start() {
        InitializeManagers();
        _capsuleRadius = capsuleCollider.radius;
        _target = mainTarget;
    }
    
    private void Update() {
        
        if (currentHealth <= 0) {
            HandleDeath();
            return;
        }
        UpdateTarget();
        MoveTowardsTarget();

        if (!IsWithinAttackRange()) {
            return;
        }
        HandleAttack();
    }
    
    #endregion
    
    
    #region Initialization

    private void InitializeManagers() {
        _enemyPoolManager = FindObjectOfType<EnemyPoolManager>();
        _jobSystemManager = FindObjectOfType<JobSystemManager>();
        ResetValues();
    }
    
    #endregion
    
    
    #region Object Pooling
    
    private void DestroyEnemy() {
        _jobSystemManager?.UnregisterEnemy(this);
        
        if (_enemyPoolManager) {
            _enemyPoolManager.ReturnEnemyToPool(this);
            return;
        }
        Destroy(gameObject);
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
        _target = newTarget.gameObject;
        _targetBuildingComponent = newTarget;
        _targetCapsuleRadius = _target.GetComponent<CapsuleCollider>().radius;
    }

    public void ResetValues() {
        currentHealth = maxHealth;
        _elapsedAttackTime = attackSpeed;
        _elapsedDeadTime = 0f;
        capsuleCollider.enabled = true;
        gameObject.tag = "Zombie";
        currentSpeed = speed;
        _jobSystemManager?.RegisterEnemy(this);
    }
    
    #endregion
    
    
    #region Private Methods
    
    private void HandleDeath() {
        
        if (_elapsedDeadTime < deadTime) {
            _elapsedDeadTime += Time.deltaTime;
            return;
        }
        DropLoot();
        DestroyEnemy();
    }

    private void DropLoot() {

        for (int i = 0; i < lootAmount; i++) {
            Instantiate(loots[Random.Range(0, loots.Length)], transform.position, transform.rotation);
        }
    }

    private void UpdateTarget() {
        
        if (!_target) {
            _target = mainTarget;
        }
        moveTarget = _target.transform.position;
    }

    private void MoveTowardsTarget() {
        Vector3 direction = Moveable.Direction(moveTarget, transform.position);

        if (direction.magnitude > _targetCapsuleRadius + _capsuleRadius) {
            animator.SetFloat(walkParameter, 1f, 0.1f, Time.deltaTime);
            currentSpeed = speed;
            return;
        }
        currentSpeed = 0f;
    }

    private bool IsWithinAttackRange() 
        => currentSpeed == 0f && _target != mainTarget;

    private void HandleAttack() {
        animator.SetFloat(walkParameter, 0f);
        
        if (_elapsedAttackTime >= attackSpeed) {
            animator.SetTrigger(attackParameter);
            
            if (_target) {
                _targetBuildingComponent?.TakeDamage(Random.Range(minDamage, maxDamage));
            }
            _elapsedAttackTime = 0f;
            return;
        }
        _elapsedAttackTime += Time.deltaTime;
    }
    
    private void DeactivateValues() {
        capsuleCollider.enabled = false;
        gameObject.tag = "ZombieDead";
        currentSpeed = 0f;
    }
    
    #endregion
}