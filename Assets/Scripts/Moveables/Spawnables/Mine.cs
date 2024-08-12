using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour, ISpawnable {

    [Header("Common")]
    public float explosionTimer;
    public int minDamage;
    public int maxDamage;

    private Spawner _parentSpawner;
    private bool _isDead;
    
    [Header("Target")]
    public float perceptionRange;
    
    private readonly List<GameObject> _targets = new();
    private GameObject _target;
    private SphereCollider _triggerCollider;

    [Header("Explosion")]
    public GameObject impactEffect;
    public Explosive explosive;

    private float _elapsedExplosionTime;
    private Coroutine _behaviourCoroutine;

    
    #region Unity Methods

    private void Start() {
        _triggerCollider = gameObject.AddComponent<SphereCollider>();
        _triggerCollider.isTrigger = true;
        _triggerCollider.radius = perceptionRange;
        
        _isDead = false;
        _elapsedExplosionTime = 0f;
        _behaviourCoroutine = StartCoroutine(BehaviourCoroutine());
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
        _parentSpawner.Unregister(this);
        
        if (_behaviourCoroutine != null) {
            StopCoroutine(_behaviourCoroutine);
        }
    }

    #endregion

    
    #region Public Methods

    public void SetParent(Spawner spawner) {
        _parentSpawner = spawner;
        _parentSpawner.Register(this);
    }

    #endregion

    
    #region Behaviour Methods

    private void Explode() {
        _isDead = true;
        explosive.Explode(minDamage, maxDamage, impactEffect);
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
    }

    private void UpdateTimer() {
        
        if (!_target) {
            _elapsedExplosionTime = 0f;
        } else {
            
            if (_elapsedExplosionTime >= explosionTimer) {
                Explode();
                return;
            }
            _elapsedExplosionTime++;
        }
    }

    private IEnumerator BehaviourCoroutine() {
        
        while (!_isDead) {
            yield return new WaitForSeconds(1f);
            UpdateTarget();
            UpdateTimer();
        }
    }

    #endregion
}