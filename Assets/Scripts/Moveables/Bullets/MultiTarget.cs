using UnityEngine;

public class MultiTarget : Bullet {
    
    public float explosionRadius;
    public string enemyTag;
    public int maxTargets;
    
    #region Unity methods
    
    private void Update() {

        if (Target == null) {
            Destroy(gameObject);
            return;
        }
        Vector3 direction = GetTargetDirection();
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude > distanceThisFrame) {
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        } else {
            HitTargets();
        }
    }
    
    #endregion
    
    #region Class methods
    
    private void HitTargets() {
        Explode();
        StartImpactEffect();
    }
    
    private void Explode() {
        Collider[] hitColliders = new Collider[maxTargets];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, hitColliders);
        
        for (int i = 0; i < numColliders; i++) {
            Collider enemy = hitColliders[i];
            
            if (enemy.CompareTag(enemyTag)) {
                Damage(enemy.GetComponent<Enemy>());
            }
        }
    }
    
    #endregion
}