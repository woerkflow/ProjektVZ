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
            explosive.Explode(damage);
            StartImpactEffect();
        }
        
        if (Target == null) {
            return;
        }

        if (timer >= 0) {
            timer -= Time.deltaTime;
        } else {
            explosive.Explode(damage);
            StartImpactEffect();
        }
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetParent(GameObject parent) {
        _parentSpawner = parent;
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void StartImpactEffect() {
        GameObject effectInst = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInst, 1f);
        Destroy(gameObject);
    }
    
    #endregion
}