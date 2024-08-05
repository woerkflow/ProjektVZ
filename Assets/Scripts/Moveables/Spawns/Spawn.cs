using System;
using UnityEngine;

public class Spawn : MonoBehaviour {
    
    [Header("Spawn")] 
    public Type type;
    public string enemyTag;
    public float perceptionRange;
    
    public enum Type {
        Chicken,
        Bull
    }

    private GameObject _parentSpawner;
    private GameObject _target;
    private CapsuleCollider _targetCollider;
    
    [Header("Movement")] 
    public float speed;
    
    [HideInInspector]
    public Vector3 direction;
    [HideInInspector]
    public float currentSpeed;

    private SpawnJobManager _spawnJobManager;
    
    [Header("Explosion")]
    public CapsuleCollider capsuleCollider;
    public int damage;
    public GameObject impactEffect;
    public Explosive explosive;
    
    
    #region Unity methods
    
    private void Start() {
        
        // Get bullet job manager
        _spawnJobManager = FindObjectOfType<SpawnJobManager>();
        
        // Register motion job
        if (_spawnJobManager != null) {
            _spawnJobManager.Register(this);
        } else {
            Debug.LogError("SpawnManager not found in the scene.");
        }
        
        // Initialize direction
        direction = Vector3.zero;
        
        // Start coroutine
        InvokeRepeating(nameof(UpdateDirection), 0f, 1f);
    }
    
    private void Update() {
        
        if (direction == Vector3.zero) {
            currentSpeed = 0f;
            return;
        }
        currentSpeed = speed;
    }

    private void OnDestroy() {
        
        // Unregister motion job
        _spawnJobManager.Unregister(this);
    }

    #endregion
    
    
    #region Public class methods

    public void SetParent(GameObject parent) {
        _parentSpawner = parent;
    }

    public void SetTarget(GameObject target) {
        _target = target;
        _targetCollider = target?.GetComponent<CapsuleCollider>();
    }
    
    #endregion
    
    
    #region Private class methods

    private void Explode() {
        
        switch (type) {
            case Type.Chicken:
                explosive.Explode(damage, impactEffect);
                break;
            case Type.Bull:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Destroy(gameObject);
    }
    
    private void UpdateDirection() {
        
        if (_parentSpawner == null) {
            Explode();
            return;
        }
        
        if (_target == null 
            || !_target.activeSelf
            || Vector3.Distance(_target.transform.position, transform.position) > perceptionRange
        ) {
            direction = 
                Moveable.Direction(
                    Vector3.Distance(_parentSpawner.transform.position, transform.position) < perceptionRange 
                            ? Moveable.GetRandomPosition(transform) 
                            : _parentSpawner.transform.position, 
                    transform.position
                );
            _target = null;
            return;
        }
        direction = Moveable.Direction(
            _target.transform.position, 
            transform.position
        );
    
        if (direction.magnitude > capsuleCollider.radius + _targetCollider.radius) {
            return;
        }
        Explode();
    }
    
    #endregion
}