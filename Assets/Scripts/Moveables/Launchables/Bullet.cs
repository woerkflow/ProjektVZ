using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour, ILaunchable {
    
    [Header("Common")]
    public BulletType bulletType;
    public int minDamage;
    public int maxDamage;
    
    private GameObject _target;
    
    [Header("Motion")]
    public float impactHeight;
    
    public Vector3 start { get; set; }
    public Vector3 end { get; set; }
    public float timeElapsed { get; set; }
    public float travelTime { get; set; }
    
    private BulletJobManager _bulletJobManager;
    
    [Header("Impact")]
    public GameObject impactEffectPrefab;
    public AudioClip bladeImpactClip;
    public AudioClip flameImpactClip;
    
    public SoundFXManager soundFXManager { get; set; }

    private bool _isPlayed;
    
    [Header("Explosion")]
    public Explosive explosive;
    
    
    #region Unity Methods

    private void Start() {
        InitializeManagers();
        _isPlayed = false;
        timeElapsed = 0f;
    }
    
    private void Update() {
        timeElapsed += Time.deltaTime;

        if (transform.position.y > 0.9925f) {
            return;
        }
        
        if (bulletType == BulletType.Flame) {
            PlaySound(flameImpactClip);
            StartImpactEffect(transform.position, Quaternion.identity);
            explosive?.Explode(minDamage, maxDamage);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider coll) {

        if (bulletType == BulletType.SawBlade) {
            DamageSingleTarget(coll);
        }
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
        travelTime = Mathf.Sqrt((2 * (start.y - end.y)) / 0.03f);
    }

    #endregion
    
    
    #region Private Methods

    private void InitializeManagers() {
        _bulletJobManager = FindObjectOfType<BulletJobManager>();
        
        if (_bulletJobManager) {
            _bulletJobManager.Register(this);
        } else {
            Debug.LogError("BulletJobManager not found in the scene.");
        }
        soundFXManager = FindObjectOfType<SoundFXManager>();
    }

    private void DamageSingleTarget(Collider coll) {
        Enemy enemy = coll.GetComponent<Enemy>();

        if (!enemy) {
            return;
        }
        PlaySound(bladeImpactClip);
        StartImpactEffect(transform.position, transform.rotation);
        enemy.TakeDamage(Random.Range(minDamage, maxDamage));
    }

    private void PlaySound(AudioClip audioClip) {

        if (_isPlayed) {
            return;
        }
        soundFXManager.PlaySoundFXClip(audioClip, transform.position, 0.5f);
        _isPlayed = true;
    }

    private void StartImpactEffect(Vector3 position, Quaternion rotation) {
        GameObject effectInstance = Instantiate(impactEffectPrefab, position, rotation);
        Destroy(effectInstance, 1f);
    }

    #endregion
}
