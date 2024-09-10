using UnityEngine;

public class Mine : MonoBehaviour, ISpawnable {

    [SerializeField] private SpawnBlueprint blueprint;

    private Spawner _parentSpawner;
    private SphereCollider _triggerCollider;
    private FXManager _fxManager;
    private bool _isExploded;
    
    
    #region Unity Methods

    private void Start() {
        _triggerCollider = gameObject.AddComponent<SphereCollider>();
        _triggerCollider.isTrigger = true;
        _triggerCollider.radius = blueprint.perceptionRange;
        InitializeManagers();
        _isExploded = false;
    }

    private void Update() {
        
        if (_parentSpawner) {
            return;
        }
        Explode();

        if (!_isExploded) {
            return;
        }
        Destroy(gameObject);
    }
    
    private void OnTriggerEnter(Collider coll) {
        
        if (!coll.CompareTag("Zombie")) {
            return;
        }
        Explode();
        
        if (!_isExploded) {
            return;
        }
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
        _fxManager = FindObjectOfType<FXManager>();
    }

    private void Explode() {
        
        if (_isExploded) {
            return;
        }
        blueprint.explosive.Explode(blueprint.minDamage, blueprint.maxDamage);
        PlaySound();
        PlayEffect();
        _isExploded = true;
    }
    
    private void PlaySound() {
        _fxManager.PlaySound(
            blueprint.impactEffectClip, 
            transform.position, 
            0.5f
        );
    }
    
    private void PlayEffect() {
        _fxManager.PlayEffect(
            blueprint.impactEffectPrefab, 
            transform.position, 
            blueprint.impactEffectPrefab.transform.rotation,
            _parentSpawner.transform
        );
    }

    #endregion
}