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
    

    #region Private class methods

    private void Damage(Enemy enemy, int damage) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }
    
    #endregion
    
    
    #region Public class methods
    
    public void Explode(int damage) {
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, radius, _hitColliders);
        
        for (int i = 0; i < enemyCount; i++) {
            Collider enemy = _hitColliders[i];
            
            if (enemy.CompareTag(enemyTag)) {
                Damage(enemy.GetComponent<Enemy>(), damage);
            }
        }
    }

    #endregion
}
