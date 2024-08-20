using UnityEngine;

public class SawBlade : Bullet {
    
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
}
