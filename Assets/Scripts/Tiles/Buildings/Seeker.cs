using UnityEngine;

public class Seeker : MonoBehaviour {
    
    [Header("Target")]
    public string enemyTag;
    public float perceptionRange;
    
    protected GameObject Target;
    protected Collider[] HitColliders;
    
    
    #region Unity methods
    
    private void Start() {
        int maxColliders = 30;
        HitColliders = new Collider[maxColliders];
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }
    
    #endregion
    
    
    #region Protected class methods
    
    private void UpdateTarget() {

        if (Target != null && Target.CompareTag(enemyTag)) {
            return;
        }
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, perceptionRange, HitColliders);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        for (int i = 0; i < enemyCount; i++) {
            Collider enemy = HitColliders[i];
            
            if (enemy.CompareTag(enemyTag)) {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            
                if (distanceToEnemy < shortestDistance) {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.gameObject;
                }
            }
        }

        if (nearestEnemy != null && shortestDistance <= perceptionRange) {
            Target = nearestEnemy;
        } else {
            Target = null;
        }
    }
    
    #endregion
}
