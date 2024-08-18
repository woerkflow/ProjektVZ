using UnityEngine;

public class Mine : MonoBehaviour, ISpawnable {

    [Header("Common")]
    public int minDamage;
    public int maxDamage;

    private Spawner _parentSpawner;
    
    [Header("Target")]
    public float perceptionRange;
    private SphereCollider _triggerCollider;

    [Header("Explosion")]
    public GameObject impactEffectPrefab;
    public AudioClip impactEffectClip;
    public Explosive explosive;
    
    public FXManager fxManager { get; set; }
    
    #region Unity Methods

    private void Start() {
        _triggerCollider = gameObject.AddComponent<SphereCollider>();
        _triggerCollider.isTrigger = true;
        _triggerCollider.radius = perceptionRange;
        InitializeManagers();
    }

    private void Update() {
        
        if (_parentSpawner) {
            return;
        }
        Explode();
        Destroy(gameObject, 0.1f);
    }
    
    private void OnTriggerEnter(Collider coll) {
        
        if (!coll.CompareTag("Zombie")) {
            return;
        }
        Explode();
        Destroy(gameObject);
    }

    private void OnDestroy() {
        _parentSpawner.Unregister(this);
    }

    #endregion

    
    #region Public Methods

    public void SetParent(Spawner spawner) {
        _parentSpawner = spawner;
        _parentSpawner.Register(this);
    }

    #endregion

    
    #region Behaviour Methods

    private void InitializeManagers() {
        fxManager = FindObjectOfType<FXManager>();
    }

    private void Explode() {
        explosive.Explode(minDamage, maxDamage);
        PlaySound();
        PlayEffect();
    }
    
    private void PlaySound() {
        fxManager.PlaySound(impactEffectClip, transform.position, 0.5f);
    }
    
    private void PlayEffect() {
        fxManager.PlayEffect(
            impactEffectPrefab, 
            transform.position, 
            impactEffectPrefab.transform.rotation
        );
    }

    #endregion
}