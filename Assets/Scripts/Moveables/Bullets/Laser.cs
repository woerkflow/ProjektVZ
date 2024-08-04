using UnityEngine;

public class Laser : MonoBehaviour {
    
    [Header("Line Renderer")] 
    public LineRenderer lineRenderer;
    public GameObject effectFirePoint;
    public GameObject effectTarget;
    public GameObject effectArea;
    
    [Header("Laser")]
    public int damage;
    public float damageTick;
    
    private GameObject _effectArea;
    private Enemy _target;
    private float _elapsedTickTime;
    private Transform _firePoint;

    
    #region Unity methods

    private void Update() {
        
        if (_target.GetHealth() > 0) {
            SetPositions();

            if (_elapsedTickTime < damageTick) {
                _elapsedTickTime += Time.deltaTime;
                return;
            }
            _elapsedTickTime = damageTick;
            Damage(_target, damage);
            return;
        }

        if (_effectArea != null) {
            return;
        }
        _effectArea = CreateDamageArea(effectArea, _target);
        Destroy(gameObject);
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
    
    private static void Damage(Enemy enemy, int damage) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }

    private static GameObject CreateDamageArea(GameObject effectArea, Enemy target) {
        return Instantiate(effectArea, target.transform.position, target.transform.rotation);
    }
    
    #endregion
}