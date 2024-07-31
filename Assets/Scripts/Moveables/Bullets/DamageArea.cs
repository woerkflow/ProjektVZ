using UnityEngine;

public class DamageArea : MonoBehaviour {

    [Header("Explosion")]
    public float duration;
    public float tickTime;
    public int damage;
    public Explosive explosive;
    
    private float _timeElapsedDuration;
    
    
    #region Unity methods
    
    void Start() {
        _timeElapsedDuration = 0f;
        InvokeRepeating(nameof(UpdateTick), 0, tickTime);
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void UpdateTick() {

        if (_timeElapsedDuration >= duration) {
            Destroy(gameObject);
            return;
        }
        _timeElapsedDuration += tickTime;
        explosive.Explode(damage);
    }
    
    #endregion
}
