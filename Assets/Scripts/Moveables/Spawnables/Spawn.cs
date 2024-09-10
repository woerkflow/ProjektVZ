using System;
using System.Collections;
using UnityEngine;

public class Spawn : MonoBehaviour, ISpawnable {
    
    [SerializeField] private SpawnBlueprint blueprint;
    
    public Vector3 moveTarget { get; private set; }
    
    private SphereCollider _triggerCollider;
    private GameObject _target;
    private CapsuleCollider _targetCollider;
    private Spawner _parentSpawner;
    private bool _isExploded;
    private JobSystemManager _jobSystemManager;
    private FXManager _fxManager;
    private Coroutine _behaviourCoroutine;
    
    
    #region Unity Methods
    
    private void Start() {
        InitializeManagers();
        _isExploded = false;
        moveTarget = transform.position;
        _behaviourCoroutine = StartCoroutine(BehaviourRoutine());
    }
    
    private void Update() {
        
        if (_parentSpawner) {
            return;
        }
        Explode();

        if (!_isExploded) {
            return;
        }
        Destroy(gameObject, 0.3f);
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
        
        if (_isExploded) {
            return;
        }
        
        switch (blueprint.type) {
            case SpawnType.Chicken:
                PlaySound();
                PlayEffect();
                blueprint.explosive.Explode(blueprint.minDamage, blueprint.maxDamage);
                break;
            case SpawnType.Mine:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _isExploded = true;
    }
    
    private void UpdateDirection() {
        
        if (!HasValidTarget()) {
            moveTarget = Moveable.GetDistance(_parentSpawner.transform.position, transform.position) <= blueprint.perceptionRange
                ? Moveable.GetRandomPosition(transform)
                : _parentSpawner.transform.position;
            return;
        }
        moveTarget = _target.transform.position;
        Vector3 direction = Moveable.Direction(moveTarget, transform.position);

        if (direction.magnitude > blueprint.capsuleCollider.radius + _targetCollider.radius) {
            return;
        }
        Explode();
        
        if (!_isExploded) {
            return;
        }
        Destroy(gameObject, 0.3f);
    }
    
    private IEnumerator BehaviourRoutine() {
        
        while (!_isExploded) {
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

    public float GetSpeed() 
        => blueprint.speed;

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
           && Moveable.GetDistance(_target.transform.position, transform.position) <= blueprint.perceptionRange;
    
    private void PlaySound() {
        _fxManager.PlaySound(
            blueprint.impactEffectClip, 
            transform.position, 
            0.5f
        );
    }
    
    private void PlayEffect() {
        _fxManager.PlayEffect(
            blueprint.impactEffectPrefab, 
            transform.position, 
            blueprint.impactEffectPrefab.transform.rotation,
            _parentSpawner.transform
        );
    }
    
    #endregion
}