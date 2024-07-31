using UnityEngine;

public class Seeker : MonoBehaviour {
    
    [Header("Target")]
    public string enemyTag;
    public float perceptionRange;
    
    protected GameObject Target;
    protected Collider[] _hitColliders;
    
    
    #region Unity methods
    
    private void Start() {
        
        // Calculate maxColliders by perception range
        float radiusInTiles = perceptionRange / 0.04f;
        float diameterInTiles = radiusInTiles * 2;
        float quadInTiles = Mathf.Pow(diameterInTiles, 2);
        int maxColliders = (int)(quadInTiles + 5);
        _hitColliders = new Collider[maxColliders];
        
        // Start coroutine
        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
    }
    
    #endregion
    
    
    #region Private class methods
    
    protected void UpdateTarget() {

        if (Target != null 
            && Target.gameObject.activeSelf
            && Target.CompareTag(enemyTag)) {
            return;
        }
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, perceptionRange, _hitColliders);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        for (int i = 0; i < enemyCount; i++) {
            Collider enemy = _hitColliders[i];
            
            if (enemy.gameObject.activeSelf && enemy.CompareTag(enemyTag)) {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            
                if (distanceToEnemy < shortestDistance) {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.gameObject;
                }
            }
        }
        Target = nearestEnemy != null ? nearestEnemy : null;
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, perceptionRange);
    }
    
    #endregion
}