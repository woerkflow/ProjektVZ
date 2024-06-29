using UnityEngine;

public class SingleTarget : Bullet {
    
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
            HitTarget(Target);
        }
    }
    
    #endregion
    
    #region Class methods
    
    private void HitTarget(GameObject target) {
        Damage(target.GetComponent<Enemy>());
        StartImpactEffect();
    }
    
    #endregion
}