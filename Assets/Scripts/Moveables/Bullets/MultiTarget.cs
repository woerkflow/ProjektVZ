using UnityEngine;

public class MultiTarget : Bullet {
    
    public float explosionRadius;
    public string enemyTag;

    private Collider[] _hitColliders;
    
    #region Unity methods

    private void Start() {
        _hitColliders = new Collider[18];
    }
    
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
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, _hitColliders);
        
        for (int i = 0; i < numColliders; i++) {
            Collider obj = _hitColliders[i];
            
            if (obj.CompareTag(enemyTag)) {
                Damage(obj.GetComponent<Enemy>());
            }
        }
    }
    
    #endregion
}