using System;
using System.Collections;
using UnityEngine;

public class Spawn : MonoBehaviour, ISpawnable, ITargetable {
    
    [Header("Spawn")] 
    public SpawnType type;
    public float perceptionRange;
    
    public enum SpawnType {
        Chicken,
        Bull
    }
    
    private GameObject _target;
    private GameObject _parentSpawner;
    private CapsuleCollider _targetCollider;
    private Seeker _seeker;
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
    
    private Coroutine _movementCoroutine;

    
    #region Unity Methods

    private void Start() {
        InitializeManagers();
        _isDead = false;
        moveTarget = transform.position;
        _movementCoroutine = StartCoroutine(UpdateDirection());
    }

    private void OnDestroy() {
        _spawnJobManager?.Unregister(this);
        _seeker?.Unregister(this);
        
        if (_movementCoroutine != null) {
            StopCoroutine(_movementCoroutine);
        }
    }

    #endregion

    
    #region Public Methods
    
    public GameObject GetTarget() => _target;

    public void SetParent(GameObject parent) {
        _parentSpawner = parent;
    }

    public void SetTarget(GameObject target) {
        _target = target;
        _targetCollider = target?.GetComponent<CapsuleCollider>();
    }

    #endregion

    
    #region Private Methods

    private void InitializeManagers() {
        _spawnJobManager = FindObjectOfType<SpawnJobManager>();
        _seeker = FindObjectOfType<Seeker>();
        
        if (_spawnJobManager == null) {
            Debug.LogError("SpawnManager not found in the scene.");
            enabled = false;
            return;
        }
        _spawnJobManager.Register(this);
        
        if (_seeker == null) {
            Debug.LogError("Seeker not found in the scene.");
            enabled = false;
            return;
        }
        _seeker.Register(this);
    }

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

    private IEnumerator UpdateDirection() {
        
        while (!_isDead) {
            
            if (_parentSpawner == null) {
                Explode();
                yield break;
            }

            if (_target == null 
                || !_target.activeSelf
                || Vector3.Distance(_target.transform.position, transform.position) > perceptionRange) {
                moveTarget = Vector3.Distance(_parentSpawner.transform.position, transform.position) < perceptionRange
                    ? Moveable.GetRandomPosition(transform)
                    : _parentSpawner.transform.position;
                _target = null;
            } else {
                moveTarget = _target.transform.position;
                Vector3 direction = Moveable.Direction(
                    _target.transform.position,
                    transform.position
                );

                if (_targetCollider != null 
                    && direction.magnitude <= capsuleCollider.radius + _targetCollider.radius) {
                    Explode();
                    yield break;
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion
}