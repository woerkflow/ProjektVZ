using UnityEngine;

public class DamageArea : MonoBehaviour {

    [Header("Explosion")]
    public float duration;
    public float tickTime;
    public int damage;
    public Explosive explosive;
    
    private float _elapsedDurationTime;
    
    
    #region Unity methods
    
    private void Start() {
        
        // Initialize timer
        _elapsedDurationTime = 0f;
        
        // Start coroutine
        InvokeRepeating(nameof(UpdateTick), 0, tickTime);
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void UpdateTick() {

        if (_elapsedDurationTime < duration) {
            _elapsedDurationTime += tickTime;
            explosive.Explode(damage);
            return;
        }
        Destroy(gameObject);
    }
    
    #endregion
}
