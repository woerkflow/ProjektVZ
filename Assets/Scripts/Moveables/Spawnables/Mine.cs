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
        
        if (_parentSpawner) {
            return;
        }
        Explode();
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider coll) {
        
        if (!coll.CompareTag("Zombie")) {
            return;
        }
        _targets.Add(coll.gameObject);
    }
    
    private void OnTriggerExit(Collider coll) {
        
        if (!_targets.Contains(coll.gameObject)) {
            return;
        }
        _targets.Remove(coll.gameObject);
    }

    private void OnDestroy() {
        _parentSpawner.Unregister(this);
        
        if (_behaviourCoroutine == null) {
            return;
        }
        StopCoroutine(_behaviourCoroutine);
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
        explosive.Explode(minDamage, maxDamage);
        StartImpactEffect(transform.position, transform.rotation);
    }
    
    private void StartImpactEffect(Vector3 position, Quaternion rotation) {
        GameObject effectInstance = Instantiate(impactEffect, position, rotation);
        Destroy(effectInstance, 1f);
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
            return;
        }
            
        if (_elapsedExplosionTime >= explosionTimer) {
            Explode();
            return;
        }
        _elapsedExplosionTime++;
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