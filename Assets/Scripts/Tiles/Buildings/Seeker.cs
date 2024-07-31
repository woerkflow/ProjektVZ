using UnityEngine;

public class Seeker : MonoBehaviour {
    
    [Header("Target")]
    public string enemyTag;
    public float perceptionRange;
    
    protected GameObject Target;
    protected Collider[] HitColliders;
    
    
    #region Private class methods
    
    protected void UpdateTarget() {

        if (Target != null 
            && Target.gameObject.activeSelf
            && Target.CompareTag(enemyTag)) {
            return;
        }
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, perceptionRange, HitColliders);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        for (int i = 0; i < enemyCount; i++) {
            Collider enemy = HitColliders[i];

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

    protected static int GetMaxColliders(float range) {
        float diameterInTiles = range * 0.02f;
        float quadInTiles = Mathf.Pow(diameterInTiles, 2f);
        return (int)(quadInTiles + 5);
    }
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, perceptionRange);
    }
    
    #endregion
}