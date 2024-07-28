using UnityEngine;

public class DamageArea : MonoBehaviour {

    [Header("Explosion")]
    public float duration;
    public float tickTime;
    public int damage;
    public Explosive explosive;
    
    private float _timeElapsed;
    private float _timeElapsedDamage;
    
    
    #region Unity methods
    
    void Start() {
        _timeElapsed = 0f;
        _timeElapsedDamage = 1f;
    }
    
    void Update() {

        if (_timeElapsed >= duration) {
            Destroy(gameObject);
            return;
        }
        _timeElapsed += Time.deltaTime;

        if (_timeElapsedDamage > 0f) {
            _timeElapsedDamage -= Time.deltaTime;
            return;
        }
        explosive.Explode(damage);
        _timeElapsedDamage = tickTime;
    }
    
    #endregion
}
