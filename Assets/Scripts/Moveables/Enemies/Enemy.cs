using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour {
    
    [SerializeField] private EnemyBlueprint blueprint;
    
    public float currentSpeed { get; private set; }
    public Vector3 moveTarget { get; private set; }
    public int currentHealth { get; private set; }
    
    private JobSystemManager _jobSystemManager;
    private EnemyPoolManager _enemyPoolManager;
    private GameObject _target;
    private Building _targetBuildingComponent;
    private float _targetCapsuleRadius;
    private float _capsuleRadius;
    private float _elapsedAttackTime;
    private float _elapsedDeadTime;
    
    
    #region Unity Methods
    
    private void Start() {
        InitializeManagers();
        _capsuleRadius = blueprint.capsuleCollider.radius;
        _target = blueprint.mainTarget;
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
        blueprint.animator.SetTrigger(blueprint.dieParameter);
        DeactivateValues();
    }

    public void SetTarget(Building newTarget) {
        _target = newTarget.gameObject;
        _targetBuildingComponent = newTarget;
        _targetCapsuleRadius = _target.GetComponent<CapsuleCollider>().radius;
    }

    public void ResetValues() {
        currentHealth = blueprint.maxHealth;
        _elapsedAttackTime = blueprint.attackSpeed;
        _elapsedDeadTime = 0f;
        blueprint.capsuleCollider.enabled = true;
        gameObject.tag = "Zombie";
        currentSpeed = blueprint.speed;
        _jobSystemManager?.RegisterEnemy(this);
    }
    
    #endregion
    
    
    #region Private Methods
    
    private void HandleDeath() {
        
        if (_elapsedDeadTime < blueprint.deadTime) {
            _elapsedDeadTime += Time.deltaTime;
            return;
        }
        DropLoot();
        DestroyEnemy();
    }

    private void DropLoot() {

        for (int i = 0; i < blueprint.lootAmount; i++) {
            Instantiate(blueprint.loots[Random.Range(0, blueprint.loots.Length)], transform.position, transform.rotation);
        }
    }

    private void UpdateTarget() {
        
        if (!_target) {
            _target = blueprint.mainTarget;
        }
        moveTarget = _target.transform.position;
    }

    private void MoveTowardsTarget() {
        Vector3 direction = Moveable.Direction(moveTarget, transform.position);

        if (direction.magnitude > _targetCapsuleRadius + _capsuleRadius) {
            blueprint.animator.SetFloat(blueprint.walkParameter, 1f, 0.1f, Time.deltaTime);
            currentSpeed = blueprint.speed;
            return;
        }
        currentSpeed = 0f;
    }

    private bool IsWithinAttackRange() 
        => currentSpeed == 0f 
           && _target != blueprint.mainTarget;

    private void HandleAttack() {
        blueprint.animator.SetFloat(blueprint.walkParameter, 0f);
        
        if (_elapsedAttackTime >= blueprint.attackSpeed) {
            blueprint.animator.SetTrigger(blueprint.attackParameter);
            
            if (_target) {
                _targetBuildingComponent?.TakeDamage(Random.Range(blueprint.minDamage, blueprint.maxDamage));
            }
            _elapsedAttackTime = 0f;
            return;
        }
        _elapsedAttackTime += Time.deltaTime;
    }
    
    private void DeactivateValues() {
        blueprint.capsuleCollider.enabled = false;
        gameObject.tag = "ZombieDead";
        currentSpeed = 0f;
    }
    
    #endregion
}