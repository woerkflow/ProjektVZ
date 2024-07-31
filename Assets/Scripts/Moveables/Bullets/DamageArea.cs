using UnityEngine;

public class DamageArea : MonoBehaviour {

    [Header("Explosion")]
    public float duration;
    public float tickTime;
    public int damage;
    public Explosive explosive;
    
    private float _timeElapsedDuration;
    private float _timeElapsedTick;
    
    
    #region Unity methods
    
    void Start() {
        _timeElapsedDuration = 0f;
        _timeElapsedTick = 0f;
    }
    
    void Update() {

        if (_timeElapsedDuration >= duration) {
            Destroy(gameObject);
            return;
        }
        _timeElapsedDuration += Time.deltaTime;

        if (_timeElapsedTick < tickTime) {
            _timeElapsedTick += Time.deltaTime;
            return;
        }
        explosive.Explode(damage);
        _timeElapsedTick = 0f;
    }
    
    #endregion
}
