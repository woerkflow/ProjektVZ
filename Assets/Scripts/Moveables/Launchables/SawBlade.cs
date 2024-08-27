using UnityEngine;

public class SawBlade : Bullet {
    
    
    #region Unity Methods
    
    private void Update() {
        IncreaseTimer();
        
        if (transform.position.y > 0.912f) { //ToDo: Need a better solution here
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
        PlaySound();
        PlayEffect(transform.rotation);
        enemy.TakeDamage(Random.Range(minDamage, maxDamage));
    }
    
    #endregion
}
