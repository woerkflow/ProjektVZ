using UnityEngine;

public class Flame : Bullet {
    
    [Header("Explosion")]
    public Explosive explosive;

    private void Update() {
        timeElapsed += Time.deltaTime;

        if (transform.position.y > 0.9925f) {
            return;
        }
        ExplodeAndDestroy();
    }

    private void ExplodeAndDestroy() {
        PlaySound();
        PlayEffect(impactEffectPrefab.transform.rotation);
        explosive?.Explode(minDamage, maxDamage);
        Destroy(gameObject);
    }
}