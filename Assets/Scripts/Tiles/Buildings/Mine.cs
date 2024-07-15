using UnityEngine;

public class Mine : MonoBehaviour {
    
    [Header("Enemy")]
    public string enemyTag;
    public float perceptionRange;

    [Header("Explode")]
    public GameObject impactEffect;
    public int damage;
    public float explosionRadius;
    public float timer;
    
    private GameObject _target;
    private Collider[] _hitColliders;
    private GameObject _parentSpawner;
    
    
    #region Unity methods
    
    private void Start() {
        int maxColliders = 30;
        _hitColliders = new Collider[maxColliders];
        InvokeRepeating(nameof(UpdateTarget), 0f, 1f);
    }
    
    
    private void Update() {
        
        if (_parentSpawner == null) {
            Explode();
            StartImpactEffect();
        }
        
        if (_target == null) {
            return;
        }

        if (timer >= 0) {
            timer -= Time.deltaTime;
        } else {
            Explode();
            StartImpactEffect();
        }
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetParent(GameObject parent) {
        _parentSpawner = parent;
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
    
    private void Damage(Enemy enemy) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }
    
    private void Explode() {
        int enemyCount = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, _hitColliders);
        
        for (int i = 0; i < enemyCount; i++) {
            Collider enemy = _hitColliders[i];
            
            if (enemy.CompareTag(enemyTag)) {
                Damage(enemy.GetComponent<Enemy>());
            }
        }
    }
    
    private void StartImpactEffect() {
        GameObject effectInst = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInst, 1f);
        Destroy(gameObject);
    }
    
    #endregion
}
