using UnityEngine;

public class Mine : MonoBehaviour {

    [Header("Mine")]
    public float explosionTimer;
    public int damage;
    
    private GameObject _parentSpawner;
    private GameObject _target;
    
    [Header("Explosion")]
    public GameObject impactEffect;
    public Explosive explosive;
    private float _elapsedExplosionTime;
    
    
    #region Unity methods
    
    private void Start() {
        
        // Set timer
        _elapsedExplosionTime = 0f;
        
        // Start coroutine
        InvokeRepeating(nameof(UpdateTimer), 0f, 1f);
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetParent(GameObject parent) {
        _parentSpawner = parent;
    }

    public void SetTarget(GameObject target) {
        _target = target;
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void Explode() {
        explosive.Explode(damage, impactEffect);
        Destroy(gameObject);
    }
    
    private void UpdateTimer() {
        
        if (_parentSpawner == null) {
            Explode();
            return;
        }
        
        if (_target == null || !_target.activeSelf) {
            _elapsedExplosionTime = 0f;
            _target = null;
            return;
        }

        if (_elapsedExplosionTime < explosionTimer) {
            _elapsedExplosionTime += 1f;
            return;
        }
        Explode();
    }
    
    #endregion
}