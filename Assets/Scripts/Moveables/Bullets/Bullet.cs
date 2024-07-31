using System;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    [Header("Bullet")]
    public Type type;
    public float speed;
    public int damage;
    public GameObject impactEffect;
    
    public enum Type {
        SingleTarget,
        MultiTarget
    }
    
    // Target
    private GameObject _target;
    
    // Curve
    private float _timeElapsed;
    private Vector3 _endPoint;
    private Vector3 _startPoint;
    private Vector3 _controlPoint;
    private float _travelTime;
    
    [Header("Explosion")]
    public Explosive explosive;
    
    
    #region Unity methods
    
    private void Update() {

        if (_target == null) {
            Destroy(gameObject);
            return;
        }
        
        // Get new t value
        _timeElapsed += Time.deltaTime;
        float t = _timeElapsed / _travelTime;
        t = Mathf.Clamp01(t);

        if (t < 1f) {
            Vector3 position = CalculateParabolicPosition(_startPoint, _controlPoint, _endPoint, t);
            Vector3 nextPosition = CalculateParabolicPosition(_startPoint, _controlPoint, _endPoint, t + 0.001f);
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(nextPosition - position);
            return;
        }
        HitTarget(_target);
    }
    
    #endregion
    
    
    #region Public class methods

    public void Seek(Transform firePoint, GameObject turretTarget) {
        _target = turretTarget;
        _startPoint = firePoint.position;
        SetPositions();
    }
    
    #endregion
    
    
    #region Private class methods

    private void SetPositions() {
        _endPoint = _target.transform.position;
        _controlPoint = CalculateControlPoint(_startPoint, _endPoint);
        
        // Set travel time
        _travelTime = Vector3.Distance(_startPoint, _endPoint) / speed;

        // Set timer
        _timeElapsed = 0f;
    }
    
    #endregion

    
    #region Trajectory

    private static Vector3 CalculateControlPoint(Vector3 start, Vector3 end) {
        Vector3 mid = (start + end) * 0.5f;
        return new Vector3(mid.x, start.y, mid.z);
    }
    
    private static Vector3 CalculateParabolicPosition(Vector3 start, Vector3 control, Vector3 end, float t) {
        Vector3 startToControl = Vector3.Lerp(start, control, t);
        Vector3 controlToEnd = Vector3.Lerp(control, end, t);
        return Vector3.Lerp(startToControl, controlToEnd, t);
    }

    #endregion
    
    
    #region Impact effect methods
    
    private void Damage(Enemy enemy) {
        enemy.SetHealth(enemy.GetHealth() - damage);
    }
    
    private void StartImpactEffect() {
        GameObject effectInst = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(effectInst, 1f);
    }
    
    private void HitTarget(GameObject target) {

        switch (type) {
            case Type.SingleTarget:
                Damage(target.GetComponent<Enemy>());
                break;
            case Type.MultiTarget:
                explosive.Explode(damage);
                break;
            default:
                throw new ArgumentOutOfRangeException();  
        }
        StartImpactEffect();
        Destroy(gameObject);
    }
    
    #endregion
}