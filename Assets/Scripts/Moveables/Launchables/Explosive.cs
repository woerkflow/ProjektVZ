using UnityEngine;

public class Explosive : MonoBehaviour {

    [Header("Explosion")]
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
        int collAmount = Physics.OverlapSphereNonAlloc(transform.position, radius, _hitColliders);
        
        for (int i = 0; i < collAmount; i++) {
            Enemy enemy = _hitColliders[i]?.GetComponent<Enemy>();
            enemy?.TakeDamage(Random.Range(minDamage, maxDamage));
        }
    }

    #endregion
}
