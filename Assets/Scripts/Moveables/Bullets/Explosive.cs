using UnityEngine;

public class Explosive : MonoBehaviour {

    [Header("Explosion")] 
    public string enemyTag;
    public float radius;
    public int maxTargets;

    private Collider[] _hitColliders;
    
    
    #region Unity methods

    private void Start() {
        _hitColliders = new Collider[maxTargets];
    }

    #endregion
    
    
    #region Public class methods
    
    public void Explode(int minDamage, int maxDamage, GameObject impactEffect = null) {
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, radius, _hitColliders);
        
        if (impactEffect) {
            GameObject effectInst = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(effectInst, 1f);
        }
        
        for (int i = 0; i < enemyCount; i++) {
            Collider coll = _hitColliders[i];
            
            if (coll.CompareTag(enemyTag)) {
                coll.GetComponent<Enemy>().TakeDamage(Random.Range(minDamage, maxDamage));
            }
        }
    }

    #endregion
}
