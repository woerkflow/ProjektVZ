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
    private Transform _firePoint;

    
    #region Unity methods

    private void Update() {
        
        if (_target.GetHealth() > 0) {
            SetPositions();

            if (_damageTick > 0) {
                _damageTick -= Time.deltaTime;
                return;
            }
            _damageTick = damageTick;
            Damage(_target);
        } else if (_target.GetHealth() <= 0 && _effectArea == null) {
            CreateDamageArea();
            Destroy(gameObject);
        }
    }
    
    #endregion
    
    
    #region Public class methods

    public void Seek(Transform firePoint, GameObject target) {
        _target = target.GetComponent<Enemy>();
        _firePoint = firePoint;
        SetPositions();
    }
    
    #endregion
    
    
    #region Private class methods

    private void SetPositions() {
        
        // Set line renderer
        lineRenderer.SetPosition(0, 
            _firePoint.position
        );
        lineRenderer.SetPosition(1, 
            _target.transform.position + _target.GetComponent<CapsuleCollider>().center
        );
        
        // Create fire point effect
        effectFirePoint.transform.position = _firePoint.position;
        effectFirePoint.transform.rotation = _firePoint.rotation;
        
        // Create target effect
        effectTarget.transform.position = _target.transform.position;
    }
    
    private void Damage(Enemy enemy) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }

    private void CreateDamageArea() {
        _effectArea = Instantiate(effectArea, _target.transform.position, _target.transform.rotation);
    }
    
    #endregion
}