using System;
using System.Collections;
using UnityEngine;

public class Spawn : MonoBehaviour, ISpawnable {
    
    [Header("Spawn")] 
    [SerializeField] private SpawnType type;
    [SerializeField] private float perceptionRange;
    
    private SphereCollider _triggerCollider;
    private GameObject _target;
    private CapsuleCollider _targetCollider;
    private Spawner _parentSpawner;
    private bool _isDead;
    
    [Header("Movement")] 
    [SerializeField] private float speed;
    
    public Vector3 moveTarget { get; private set; }
    
    private JobSystemManager _jobSystemManager;
    
    [Header("Explosion")]
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private GameObject impactEffectPrefab;
    [SerializeField] private AudioClip impactEffectClip;
    [SerializeField] private Explosive explosive;

    private FXManager _fxManager;
    private Coroutine _behaviourCoroutine;
    
    
    #region Unity Methods
    
    private void Start() {
        InitializeManagers();
        
        _isDead = false;
        moveTarget = transform.position;
        _behaviourCoroutine = StartCoroutine(BehaviourRoutine());
    }
    
    private void Update() {
        
        if (_parentSpawner) {
            return;
        }
        Explode();
        Destroy(gameObject,0.1f);
    }
    
    private void OnDestroy() {
        _jobSystemManager?.UnregisterSpawn(this);
        _parentSpawner.Unregister(this);
        
        if (_behaviourCoroutine == null) {
            return;
        }
        StopCoroutine(_behaviourCoroutine);
    }
    
    #endregion
    
    
    #region Behavior methods
    
    private void Explode() {
        _isDead = true;
        
        switch (type) {
            case SpawnType.Chicken:
                explosive.Explode(minDamage, maxDamage);
                PlaySound();
                PlayEffect();
                break;
            case SpawnType.Bull:
                // Handle Bull specific explosion logic
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void UpdateDirection() {
        
        if (!HasValidTarget()) {
            moveTarget = Moveable.GetDistance(_parentSpawner.transform.position, transform.position) <= perceptionRange
                ? Moveable.GetRandomPosition(transform)
                : _parentSpawner.transform.position;
            return;
        }
        moveTarget = _target.transform.position;
        Vector3 direction = Moveable.Direction(moveTarget, transform.position);

        if (direction.magnitude > capsuleCollider.radius + _targetCollider.radius) {
            return;
        }
        Explode();
        Destroy(gameObject,0.1f);
    }
    
    private IEnumerator BehaviourRoutine() {
        
        while (!_isDead) {
            yield return new WaitForSeconds(1f);
            UpdateDirection();
        }
    }
    
    #endregion
    
    
    #region Public Methods
    
    public void SetParent(Spawner parent) {
        _parentSpawner = parent;
        _parentSpawner.Register(this);
    }

    public void SetTarget(GameObject target) {
        _target = target;
        _targetCollider = target.GetComponent<CapsuleCollider>();
    }

    public float GetSpeed() => speed;

    #endregion
    
    
    #region Private Methods
    
    private void InitializeManagers() {
        _fxManager = FindObjectOfType<FXManager>();
        _jobSystemManager = FindObjectOfType<JobSystemManager>();
        _jobSystemManager?.RegisterSpawn(this);
    }
    
    private bool HasValidTarget()
        => _target
           && _target.gameObject.activeSelf
           && Moveable.GetDistance(_target.transform.position, transform.position) <= perceptionRange;
    
    private void PlaySound() {
        _fxManager.PlaySound(
            impactEffectClip, 
            transform.position, 
            0.5f
        );
    }
    
    private void PlayEffect() {
        _fxManager.PlayEffect(
            impactEffectPrefab, 
            transform.position, 
            impactEffectPrefab.transform.rotation,
            _parentSpawner.transform
        );
    }
    
    #endregion
}