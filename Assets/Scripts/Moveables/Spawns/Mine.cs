using UnityEngine;

public class Mine : Seeker {

    [Header("Mine")]
    public float timer;
    public int damage;
    
    [Header("Explosion")]
    public GameObject impactEffect;
    public Explosive explosive;
    
    private GameObject _parentSpawner;
    
    
    #region Unity methods
    
    private void Update() {
        
        if (_parentSpawner == null) {
            explosive.Explode(damage, impactEffect);
            Destroy(gameObject);
        }
        
        if (Target == null) {
            return;
        }

        if (timer >= 0) {
            timer -= Time.deltaTime;
        } else {
            explosive.Explode(damage, impactEffect);
            Destroy(gameObject);
        }
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetParent(GameObject parent) {
        _parentSpawner = parent;
    }
    
    #endregion
}