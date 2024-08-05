using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour {
    
    [Header("Line Renderer")] 
    public LineRenderer lineRenderer;
    public GameObject effectFirePoint;
    public GameObject effectTarget;
    public GameObject effectArea;
    
    [Header("Laser")]
    public int damage;
    public int duration;
    public int tickTime;
    
    private GameObject _effectArea;
    private Enemy _target;
    private float _elapsedTickTime;
    private Transform _firePoint;

    
    #region Unity methods

    private void Update() {
        
        if (_target.GetHealth() > 0) {
            SetPositions();
            return;
        }
        
        if (_effectArea == null) {
            _effectArea = CreateDamageArea(effectArea, _target);
        }
        Destroy(gameObject);
    }
    
    #endregion
    
    
    #region Public class methods

    public void Seek(Transform firePoint, GameObject target) {
        _target = target.GetComponent<Enemy>();
        _firePoint = firePoint;
        
        StartCoroutine(
            StartTickDamage(
                duration, 
                tickTime,
                _target, 
                damage, 
                gameObject
            )
        );
    }
    
    #endregion
    
    
    #region Private class methods

    private void SetPositions() {
        lineRenderer.SetPosition(
            0, 
            _firePoint.position
        );
        lineRenderer.SetPosition(
            1, 
            _target.transform.position + _target.GetComponent<CapsuleCollider>().center
        );
        effectFirePoint.transform.position = _firePoint.position;
        effectFirePoint.transform.rotation = _firePoint.rotation;
        effectTarget.transform.position = _target.transform.position;
    }

    private static GameObject CreateDamageArea(GameObject effectArea, Enemy target) {
        return Instantiate(effectArea, target.transform.position, target.transform.rotation);
    }
    
    private static void Damage(Enemy enemy, int damage) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }

    private static IEnumerator StartTickDamage(
        int duration,
        int tickTime,
        Enemy enemy,
        int damage,
        GameObject gameObject
    ) {
        for (var i = 0; i < duration; i++) {
            Damage(enemy, damage);
            yield return new WaitForSeconds(tickTime);
        }
        Destroy(gameObject);
    }
    
    #endregion
}