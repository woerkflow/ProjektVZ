using UnityEngine;

public class Flame : Bullet {
    
    [SerializeField] private Explosive explosive;

    private bool _isExploded;
    
    
    #region Unity Methods

    private void Start() {
        InitializeManagers();
        timeElapsed = 0f;
        _isExploded = false;
    }

    private void Update() {
        IncreaseTimer();
        
        if (transform.position.y > blueprint.impactAnchor.position.y) {
            return;
        }
        Explode();

        if (!_isExploded) {
            return;
        }
        Destroy(gameObject);
    }
    
    #endregion
    
    
    #region Private Class Methods

    private void Explode() {
        
        if (_isExploded) {
            return;
        }
        PlaySound();
        PlayEffect(blueprint.impactEffectPrefab.transform.rotation);
        explosive?.Explode(blueprint.minDamage, blueprint.maxDamage);
        _isExploded = true;
    }
    
    #endregion
}