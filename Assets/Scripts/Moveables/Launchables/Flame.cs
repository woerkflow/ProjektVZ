using UnityEngine;

public class Flame : Bullet {
    
    [Header("Explosion")]
    public Explosive explosive;
    
    
    #region Unity Methods

    private void Update() {
        timeElapsed += Time.deltaTime;

        if (transform.position.y > 0.7989f) {
            return;
        }
        Explode();
        Destroy(gameObject, 0.5f);
    }
    
    #endregion
    
    
    #region Private Class Methods

    private void Explode() {
        PlaySound();
        PlayEffect(impactEffectPrefab.transform.rotation);
        explosive?.Explode(minDamage, maxDamage);
    }
    
    #endregion
}