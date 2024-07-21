using UnityEngine;

public class Mine : Seeker {

    [Header("Explode")]
    public GameObject impactEffect;
    public int damage;
    public float explosionRadius;
    public float timer;
    
    private GameObject _parentSpawner;
    
    
    #region Unity methods
    
    private void Update() {
        
        if (_parentSpawner == null) {
            Explode();
            StartImpactEffect();
        }
        
        if (Target == null) {
            return;
        }

        if (timer >= 0) {
            timer -= Time.deltaTime;
        } else {
            Explode();
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
    
    private void Damage(Enemy enemy) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }
    
    private void Explode() {
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, HitColliders);
        
        for (int i = 0; i < enemyCount; i++) {
            Collider enemy = HitColliders[i];
            
            if (enemy.CompareTag(enemyTag)) {
                Damage(enemy.GetComponent<Enemy>());
            }
        }
    }
    
    private void StartImpactEffect() {
        GameObject effectInst = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInst, 1f);
        Destroy(gameObject);
    }
    
    #endregion
}
