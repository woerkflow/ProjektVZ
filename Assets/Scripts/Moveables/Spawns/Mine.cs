using UnityEngine;

public class Mine : Seeker {

    [Header("Mine")]
    public float timer;
    public int damage;
    
    private GameObject _parentSpawner;
    
    [Header("Explosion")]
    public GameObject impactEffect;
    public Explosive explosive;
    
    
    #region Unity methods
    
    private void Start() {
        
        // Calculate maxColliders by perception range
        HitColliders = new Collider[GetMaxColliders(perceptionRange)];
        
        // Start coroutines
        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
        InvokeRepeating(nameof(UpdateTimer), 0f, 0.5f);
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetParent(GameObject parent) {
        _parentSpawner = parent;
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void UpdateTimer() {
        
        if (_parentSpawner == null) {
            explosive.Explode(damage, impactEffect);
            Destroy(gameObject);
            return;
        }
        
        if (Target == null) {
            return;
        }

        if (timer > 0) {
            timer -= 0.5f;
            return;
        }
        _parentSpawner = null;
    }
    
    #endregion
}