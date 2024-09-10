using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour, ILaunchable {

    [SerializeField] private LaserBlueprint blueprint;
    
    private GameObject _effectArea;
    private Enemy _target;
    private float _elapsedTickTime;
    private Transform _firePoint;

    
    #region Unity methods

    private void Update() {
        
        if (_target.currentHealth > 0) {
            SetPositions();
            return;
        }
        
        if (_target && !_effectArea) {
            _effectArea = CreateDamageArea(blueprint.effectArea, _target);
        }
        Destroy(gameObject);
    }
    
    #endregion
    
    
    #region Public class methods

    public void Launch(Transform firePoint, GameObject target) {
        _target = target.GetComponent<Enemy>();
        _firePoint = firePoint;
        SetPositions();
        
        StartCoroutine(
            StartTickDamage(
                blueprint.duration, 
                blueprint.tickTime,
                _target, 
                blueprint.damage, 
                gameObject
            )
        );
    }
    
    #endregion
    
    
    #region Private class methods

    private void SetPositions() {
        Vector3 targetCenter = _target.GetComponent<CapsuleCollider>().center;
        
        blueprint.lineRenderer.SetPosition(
            0, 
            _firePoint.position
        );
        blueprint.lineRenderer.SetPosition(
            1, 
            _target.transform.position + targetCenter
        );
        blueprint.effectFirePoint.transform.position = _firePoint.position;
        blueprint.effectFirePoint.transform.rotation = _firePoint.rotation;
        blueprint.effectTarget.transform.position = _target.transform.position + targetCenter;
        blueprint.effectTarget.transform.rotation = Quaternion.Inverse(_firePoint.rotation);
    }

    private static GameObject CreateDamageArea(GameObject effectArea, Enemy target)
        => Instantiate(effectArea, target.transform.position, target.transform.rotation);

    private static IEnumerator StartTickDamage(
        int duration,
        int tickTime,
        Enemy enemy,
        int damage,
        GameObject gameObject
    ) {
        for (int i = 0; i < duration; i++) {
            enemy.TakeDamage(damage);
            yield return new WaitForSeconds(tickTime);
        }
        Destroy(gameObject);
    }
    
    #endregion
}