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

    private void Update() {
        
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
    
    private void Damage(Enemy enemy) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }
    
    private void Explode() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        
        foreach (Collider collider in colliders) {
            if (collider.CompareTag(enemyTag)) {
                Damage(collider.GetComponent<Enemy>());
            }
        }
    }
    
    private void StartImpactEffect() {
        GameObject effectInst = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInst, 1f);
        Destroy(gameObject);
    }
}
