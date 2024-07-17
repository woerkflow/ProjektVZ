using UnityEngine;

public class Launcher : MonoBehaviour {
    
    [Header("Shoot")]
    public float fireRate;
    public Transform firePoint;
    public GameObject prefabBullet;
    
    [Header("Enemy")]
    public string enemyTag;
    public float perceptionRange;
    
    [Header("Rotation")]
    public Transform partToRotate;
    public float turnSpeed;

    private GameObject _target;
    private float _fireCountDown;
    private Collider[] _hitColliders;
    
    
    #region Unity methods
    
    private void Start() {
        int maxColliders = 30;
        _hitColliders = new Collider[maxColliders];
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }
    
    void Update() {
        
        if (_target == null) {
            return;
        }
        LockOnTarget();
        Shoot();
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void UpdateTarget() {

        if (_target != null && _target.CompareTag(enemyTag)) {
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
            _target = nearestEnemy;
        } else {
            _target = null;
        }
    }
    
    private void LockOnTarget() {
        Vector3 direction = _target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }
    
    private void Shoot() {
        
        if (_fireCountDown <= 0f) {
            GameObject bulletGo = Instantiate(prefabBullet, firePoint.position, firePoint.rotation);
            Bullet bullet = bulletGo.GetComponent<Bullet>();

            if (bullet != null) {
                bullet.Seek(_target);
            }
            _fireCountDown = fireRate;
        } else {
            _fireCountDown -= Time.deltaTime;
        }
    }
    
    #endregion
}
