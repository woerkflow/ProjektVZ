using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour, ISpawnable {
    
    [Header("Spawn")] 
    public SpawnType type;
    public float perceptionRange;
    
    public enum SpawnType {
        Chicken,
        Bull
    }
    
    private readonly List<GameObject> _targets = new();
    private SphereCollider _triggerCollider;
    private GameObject _target;
    private CapsuleCollider _targetCollider;
    private Spawner _parentSpawner;
    private bool _isDead;
    
    [Header("Movement")] 
    public float speed;
    
    public Vector3 moveTarget { get; private set; }
    
    private SpawnJobManager _spawnJobManager;
    
    [Header("Explosion")]
    public CapsuleCollider capsuleCollider;
    public int minDamage;
    public int maxDamage;
    public GameObject impactEffect;
    public Explosive explosive;

    private Coroutine _behaviourCoroutine;
    
    #region Unity Methods

    private void Start() {
        _triggerCollider = gameObject.AddComponent<SphereCollider>();
        _triggerCollider.isTrigger = true;
        _triggerCollider.radius = perceptionRange;
        
        InitializeManagers();
        
        _isDead = false;
        moveTarget = transform.position;
        _behaviourCoroutine = StartCoroutine(BehaviourRoutine());
    }

    private void Update() {
        
        if (!_parentSpawner) {
            Explode();
        }
    }
    
    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Zombie")) {
            _targets.Add(other.gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other) {
        
        if (_targets.Contains(other.gameObject)) {
            _targets.Remove(other.gameObject);
        }
    }

    private void OnDestroy() {
        _spawnJobManager?.Unregister(this);
        _parentSpawner.Unregister(this);
        
        if (_behaviourCoroutine != null) {
            StopCoroutine(_behaviourCoroutine);
        }
    }

    #endregion
    
    
    #region Behavior methods
    
    private void Explode() {
        _isDead = true;
        
        switch (type) {
            case SpawnType.Chicken:
                explosive.Explode(minDamage, maxDamage, impactEffect);
                break;
            case SpawnType.Bull:
                // Handle Bull specific explosion logic
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Destroy(gameObject);
    }
    
    private void UpdateTarget() {
        
        _targets.RemoveAll(enemy 
            => !enemy
               || !enemy.gameObject.activeSelf 
               || !enemy.CompareTag("Zombie")
        );
        _target = _targets.Count > 0 
            ? _targets[0] 
            : null;
        _targetCollider = _target?.GetComponent<CapsuleCollider>();
    }

    private void UpdateDirection() {
        
        if (!_target) {
            moveTarget = Vector3.Distance(_parentSpawner.transform.position, transform.position) < perceptionRange
                ? Moveable.GetRandomPosition(transform)
                : _parentSpawner.transform.position;
            return;
        }
        moveTarget = _target.transform.position;
        Vector3 direction = Moveable.Direction(_target.transform.position, transform.position);

        if (_targetCollider 
            && direction.magnitude <= capsuleCollider.radius + _targetCollider.radius
        ) {
            Explode();
        }
    }
    
    private IEnumerator BehaviourRoutine() {
        
        while (!_isDead) {
            yield return new WaitForSeconds(1f);
            UpdateTarget();
            UpdateDirection();
        }
    }
    
    #endregion

    
    #region Public Methods

    public void SetParent(Spawner parent) {
        _parentSpawner = parent;
        _parentSpawner.Register(this);
    }

    #endregion

    
    #region Private Methods

    private void InitializeManagers() {
        _spawnJobManager = FindObjectOfType<SpawnJobManager>();
        
        if (!_spawnJobManager) {
            Debug.LogError("SpawnManager not found in the scene.");
            enabled = false;
            return;
        }
        _spawnJobManager.Register(this);
    }
    
    #endregion
}