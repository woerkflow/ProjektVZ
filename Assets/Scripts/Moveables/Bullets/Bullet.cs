using System;
using UnityEngine;

public class Bullet : MonoBehaviour, ILaunchable {
    
    [Header("Common")]
    public Type bulletType;
    public int minDamage;
    public int maxDamage;
    public GameObject impactEffectPrefab;
    
    public enum Type {
        SingleTarget,
        MultiTarget
    }
    
    private GameObject _target;
    
    [Header("Motion")]
    public float speed;
    public float impactHeight;
    
    [HideInInspector]
    public BezierCurve parabolicCurve;
    
    private float _timeElapsed;
    private float _travelTime;
    private BulletJobManager _bulletJobManager;
    
    [Header("Explosion")]
    public Explosive explosive;
    
    
    #region Unity Methods

    private void Start() {
        _bulletJobManager = FindObjectOfType<BulletJobManager>();
        
        if (_bulletJobManager) {
            _bulletJobManager.Register(this);
        } else {
            Debug.LogError("BulletJobManager not found in the scene.");
        }
    }
    
    private void Update() {
        _timeElapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_timeElapsed / _travelTime);
        UpdatePosition(t);

        if (t >= 1f) {
            HitTarget();
        }
    }
    
    private void OnDestroy() {
        _bulletJobManager?.Unregister(this);
    }

    #endregion
    

    #region Public Methods

    public void Launch(Transform firePoint, GameObject target) {
        _target = target;
        InitializeParabolicCurve(firePoint.position, target.transform.position);
    }

    #endregion
    
    
    #region Private Methods

    private void InitializeParabolicCurve(Vector3 start, Vector3 targetPosition) {
        Vector3 end = new Vector3(
            targetPosition.x, 
            targetPosition.y + impactHeight,
            targetPosition.z
        );
        Vector3 control = Moveable.CalculateControlPoint(start, end);

        _timeElapsed = 0f;
        _travelTime = (Vector3.Distance(start, control) + Vector3.Distance(control, end)) / speed;
        parabolicCurve = new BezierCurve { start = start, control = control, end = end, t = 0f };
    }

    private void UpdatePosition(float t) {
        parabolicCurve.t = t;
    }

    private void HitTarget() {
        
        switch (bulletType) {
            case Type.SingleTarget:
                _target?.GetComponent<Enemy>()?.TakeDamage(minDamage);
                break;
            case Type.MultiTarget:
                explosive?.Explode(minDamage, maxDamage);
                break;
            default:
                throw new ArgumentOutOfRangeException();  
        }
        StartImpactEffect();
        Destroy(gameObject);
    }

    private void StartImpactEffect() {
        
        if (!impactEffectPrefab) {
            return;
        }
        GameObject effectInstance = Instantiate(impactEffectPrefab, parabolicCurve.end, Quaternion.identity);
        Destroy(effectInstance, 1f);
    }

    #endregion
}
