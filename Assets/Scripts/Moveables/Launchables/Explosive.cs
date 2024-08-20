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
    
    public void Explode(int minDamage, int maxDamage) {
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, radius, _hitColliders);
        
        for (int i = 0; i < enemyCount; i++) {
            Collider coll = _hitColliders[i];
            
            if (!coll.CompareTag(enemyTag)) {
                return;
            }
            coll.GetComponent<Enemy>().TakeDamage(Random.Range(minDamage, maxDamage));
        }
    }

    #endregion
}
