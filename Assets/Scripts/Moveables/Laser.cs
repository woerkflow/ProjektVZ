using UnityEngine;

public class Laser : MonoBehaviour {
    
    [Header("Laser parts")] 
    public LineRenderer lineRenderer;
    public GameObject effectFirePoint;
    public GameObject effectTarget;
    public GameObject effectArea;
    public int damage;
    public float damageTick;
    
    private GameObject _effectArea;
    private Enemy _target;
    private float _damageTick;

    
    #region Unity methods

    private void Update() {
        
        if (_target == null) {
            Destroy(gameObject);
            return;
        }
        
        if (_target.GetHealth() > 0) {

            if (_damageTick > 0) {
                _damageTick -= Time.deltaTime;
                return;
            }
            _damageTick = damageTick;
            Damage(_target);
        } else if (_target.GetHealth() <= 0 && _effectArea == null) {
            CreateDamageArea();
        }
    }
    
    #endregion
    
    
    #region Public class methods

    public void Seek(Transform firePoint, GameObject target) {
        _target = target.GetComponent<Enemy>();
        
        // Set line renderer
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.transform.position);
        
        // Create fire point effect
        effectFirePoint.transform.position = firePoint.position;
        effectFirePoint.transform.rotation = firePoint.rotation;
        
        // Create target effect
        effectTarget.transform.position = target.transform.position;
    }
    
    #endregion
    
    
    #region Private class methods
    
    private void Damage(Enemy enemy) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }

    private void CreateDamageArea() {
        _effectArea = Instantiate(effectArea, _target.transform.position, _target.transform.rotation);
    }
    
    #endregion
}
