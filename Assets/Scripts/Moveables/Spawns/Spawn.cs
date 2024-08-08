using System;
using UnityEngine;

public class Spawn : MonoBehaviour {
    
    [Header("Spawn")] 
    public Type type;
    public float perceptionRange;
    
    public enum Type {
        Chicken,
        Bull
    }
    
    private GameObject _target;
    private GameObject _parentSpawner;
    private CapsuleCollider _targetCollider;
    private Seeker _seeker;
    
    [Header("Movement")] 
    public float speed;
    
    [HideInInspector] 
    public Vector3 moveTarget;
    
    private SpawnJobManager _spawnJobManager;
    
    [Header("Explosion")]
    public CapsuleCollider capsuleCollider;
    public int damage;
    public GameObject impactEffect;
    public Explosive explosive;
    
    
    #region Unity methods
    
    private void Start() {
        _spawnJobManager = FindObjectOfType<SpawnJobManager>();
        _seeker = FindObjectOfType<Seeker>();
        
        if (_spawnJobManager != null) {
            _spawnJobManager.Register(this);
        } else {
            Debug.LogError("SpawnManager not found in the scene.");
        }
        
        if (_seeker != null) {
            _seeker.RegisterSpawn(this);
        } else {
            Debug.LogError("Seeker not found in the scene.");
        }
        moveTarget = transform.position;
        InvokeRepeating(nameof(UpdateDirection), 0f, 1f);
    }

    private void OnDestroy() {
        _spawnJobManager.Unregister(this);
        _seeker.UnregisterSpawn(this);
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
            moveTarget = Vector3.Distance(_parentSpawner.transform.position, transform.position) < perceptionRange
                ? Moveable.GetRandomPosition(transform)
                : _parentSpawner.transform.position;
            _target = null;
            return;
        }
        moveTarget = _target.transform.position;
        
        Vector3 direction = Moveable.Direction(
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