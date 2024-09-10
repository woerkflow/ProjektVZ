using UnityEngine;

public class SawBlade : Bullet {
    
    private bool _isPlayed;
    
    #region Unity Methods
    
    private void Start() {
        InitializeManagers();
        timeElapsed = 0f;
        _isPlayed = false;
    }
    
    private void Update() {
        IncreaseTimer();
        
        if (transform.position.y > blueprint.impactAnchor.position.y) {
            return;
        }
        Destroy(gameObject);
    }
    
    #endregion
    
    
    #region Private Class Methods
    
    private void OnTriggerEnter(Collider coll) {
        HitCollider(coll);
    }
    
    private void HitCollider(Collider coll) {
        Enemy enemy = coll.GetComponent<Enemy>();

        if (!enemy) {
            return;
        }

        if (!_isPlayed) {
            PlaySound();
            PlayEffect(transform.rotation);
            _isPlayed = true;
        }
        enemy.TakeDamage(Random.Range(blueprint.minDamage, blueprint.maxDamage));
    }
    
    #endregion
}
