using System;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    [Header("Bullet")]
    public Type type;
    public int minDamage;
    public int maxDamage;
    public GameObject impactEffect;
    
    public enum Type {
        SingleTarget,
        MultiTarget
    }
    
    [Header("Motion")]
    public float speed;
    public float impactHeight;
    
    public Vector3 startPoint { get; private set; }
    public Vector3 controlPoint { get; private set; }
    public Vector3 endPoint { get; private set; }
    public float t { get; private set; }
    
    private GameObject _target;
    private float _timeElapsed;
    private float _travelTime;
    private BulletJobManager _bulletJobManager;
    
    [Header("Explosion")]
    public Explosive explosive;
    
    
    #region Unity methods

    private void Start() {
        _bulletJobManager = FindObjectOfType<BulletJobManager>();
        
        if (_bulletJobManager != null) {
            _bulletJobManager.Register(this);
        } else {
            Debug.LogError("BulletJobManager not found in the scene.");
        }
    }
    
    private void Update() {
        _timeElapsed += Time.deltaTime;
        t = _timeElapsed / _travelTime;
        t = Mathf.Clamp01(t);
        
        if (t < 1f) {
            return;
        }
        HitTarget(_target);
    }
    
    private void OnDestroy() {
        _bulletJobManager.Unregister(this);
    }
    
    #endregion
    
    
    #region Public class methods

    public void Seek(Transform firePoint, GameObject target) {
        _target = target;
        startPoint = firePoint.position;
        endPoint = new Vector3 (_target.transform.position.x, _target.transform.position.y + impactHeight, _target.transform.position.z);
        controlPoint = Moveable.CalculateControlPoint(startPoint, endPoint);
        _travelTime = (Vector3.Distance(startPoint, controlPoint) + Vector3.Distance(controlPoint, endPoint)) / speed;
        _timeElapsed = 0f;
    }
    
    #endregion
    
    
    #region Impact effect methods
    
    private void StartImpactEffect() {
        GameObject effectInst = Instantiate(impactEffect, endPoint, transform.rotation);
        Destroy(effectInst, 1f);
    }
    
    private void HitTarget(GameObject target) {

        switch (type) {
            case Type.SingleTarget:
                target.GetComponent<Enemy>().TakeDamage(minDamage);
                break;
            case Type.MultiTarget:
                explosive.Explode(minDamage, maxDamage);
                break;
            default:
                throw new ArgumentOutOfRangeException();  
        }
        StartImpactEffect();
        Destroy(gameObject);
    }
    
    #endregion
}