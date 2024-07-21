using UnityEngine;

public class Turret : MonoBehaviour {
    
    [Header("Turret")]
    public float fireRate;
    public Transform firePoint;
    
    [Header("Rotation")]
    public Transform partToRotate;
    public float turnSpeed;
    
    [Header("Enemy")]
    public string enemyTag;
    public float perceptionRange;

    protected GameObject Target;
    protected float FireCountDown;
    
    private Collider[] _hitColliders;
    
    #region Unity methods
    
    private void Start() {
        int maxColliders = 30;
        _hitColliders = new Collider[maxColliders];
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }
    
    #endregion
    
    #region Private class methods
    
    protected void UpdateTarget() {

        if (Target != null && Target.CompareTag(enemyTag)) {
            return;
        }
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, perceptionRange, _hitColliders);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        for (int i = 0; i < enemyCount; i++) {
            Collider enemy = _hitColliders[i];
            
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
    
    protected void LockOnTarget() {
        Vector3 direction = Target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    #endregion
}
