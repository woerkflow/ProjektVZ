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
    
    private void Start() {
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.5f);
    }
    
    private void UpdateTarget() {

        if (_target != null && _target.CompareTag(enemyTag)) {
            return;
        }
        
        Collider[] enemies = Physics.OverlapSphere(transform.position, perceptionRange);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider enemy in enemies) {
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
    
    void Update() {
        
        if (_target == null) {
            return;
        }
        LockOnTarget();
        Shoot();
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
            _fireCountDown = 1f / fireRate;
        } else {
            _fireCountDown -= Time.deltaTime;
        }
    }
}
