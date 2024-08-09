using System.Collections;
using UnityEngine;

public class Mine : MonoBehaviour, ISpawnable, ITargetable {

    [Header("Mine")]
    public float perceptionRange;
    public float explosionTimer;
    public int minDamage;
    public int maxDamage;

    private GameObject _parentSpawner;
    private GameObject _target;
    private Seeker _seeker;
    private bool _isDead;

    [Header("Explosion")]
    public GameObject impactEffect;
    public Explosive explosive;

    private float _elapsedExplosionTime;
    private Coroutine _timerCoroutine;

    
    #region Unity Methods

    private void Start() {
        InitializeManagers();
        _isDead = false;
        _elapsedExplosionTime = 0f;
        _timerCoroutine = StartCoroutine(UpdateTimerCoroutine());
    }

    private void OnDestroy() {
        _seeker?.Unregister(this);
        
        if (_timerCoroutine != null) {
            StopCoroutine(_timerCoroutine);
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
    }

    #endregion

    
    #region Private Methods

    private void InitializeManagers() {
        _seeker = FindObjectOfType<Seeker>();
        
        if (_seeker != null) {
            _seeker.Register(this);
        } else {
            Debug.LogError("Seeker not found in the scene.");
        }
    }

    private void Explode() {
        _isDead = true;
        explosive.Explode(minDamage, maxDamage, impactEffect);
        Destroy(gameObject);
    }

    private IEnumerator UpdateTimerCoroutine() {
        
        while (!_isDead) {
            
            if (_parentSpawner == null) {
                Explode();
                yield break;
            }

            if (_target == null 
                || !_target.activeSelf
                || Vector3.Distance(_target.transform.position, transform.position) > perceptionRange) {
                _elapsedExplosionTime = 0f;
                _target = null;
            } else {
                
                if (_elapsedExplosionTime >= explosionTimer) {
                    Explode();
                    yield break;
                }
                _elapsedExplosionTime++;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion
}