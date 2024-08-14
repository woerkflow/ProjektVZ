using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour, ILaunchable {
    
    [Header("Common")]
    public BulletType bulletType;
    public int minDamage;
    public int maxDamage;
    public GameObject impactEffectPrefab;

    private GameObject _target;
    
    [Header("Motion")]
    public float impactHeight;
    
    public Vector3 start { get; set; }
    public Vector3 end { get; set; }
    public float timeElapsed { get; set; }
    public float travelTime { get; set; }
    
    private BulletJobManager _bulletJobManager;
    
    [Header("Explosion")]
    public Explosive explosive;

    private bool _isExploded;
    
    
    #region Unity Methods

    private void Start() {
        _bulletJobManager = FindObjectOfType<BulletJobManager>();
        
        if (_bulletJobManager) {
            _bulletJobManager.Register(this);
        } else {
            Debug.LogError("BulletJobManager not found in the scene.");
        }
        _isExploded = false;
    }
    
    private void Update() {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= travelTime) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider) {
        
        switch (bulletType) {
            case BulletType.SingleTarget:
                DamageSingleTarget(collider);
                break;
            case BulletType.MultiTarget:
                
                if (_isExploded) {
                    explosive?.Explode(minDamage, maxDamage);
                    _isExploded = true;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();  
        }
        StartImpactEffect();
    }

    private void OnDestroy() {
        _bulletJobManager?.Unregister(this);
    }

    #endregion
    

    #region Public Methods

    public void Launch(Transform firePoint, GameObject target) {
        start = firePoint.position;
        end = new Vector3(
            target.transform.position.x, 
            target.transform.position.y + impactHeight,
            target.transform.position.z
        );
        travelTime = Mathf.Sqrt((2 * (start.y - end.y)) / 0.04f);
        timeElapsed = 0f;
    }

    #endregion
    
    
    #region Private Methods

    private void DamageSingleTarget(Collider coll) {
        Enemy enemy = coll.GetComponent<Enemy>();

        if (enemy) {
            enemy.TakeDamage(Random.Range(minDamage, maxDamage));
        }
    }

    private void StartImpactEffect() {
        GameObject effectInstance = Instantiate(impactEffectPrefab, transform.position, transform.rotation);
        Destroy(effectInstance, 1f);
    }

    #endregion
}
