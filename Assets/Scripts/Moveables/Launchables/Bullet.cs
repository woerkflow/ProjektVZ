using UnityEngine;

public class Bullet : MonoBehaviour, ILaunchable {
    
    [Header("Common")]
    [SerializeField] protected int minDamage;
    [SerializeField] protected int maxDamage;
    
    private GameObject _target;
    
    [Header("Motion")]
    [SerializeField] private float impactHeight;
    
    public float timeElapsed { get; protected set; }
    public Vector3 start { get; private set; }
    public Vector3 end { get; private set; }
    public float travelTime { get; private set; }
    
    private BulletJobManager _bulletJobManager;
    
    [Header("Impact")]
    [SerializeField] protected GameObject impactEffectPrefab;
    [SerializeField] private AudioClip impactEffectClip;

    private FXManager _fxManager;

    private bool _isPlayed;
    
    
    #region Unity Methods

    private void Start() {
        InitializeManagers();
        _isPlayed = false;
        timeElapsed = 0f;
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
        travelTime = Moveable.GetTravelTime(start, end);
    }

    #endregion
    
    
    #region Private Methods

    protected void IncreaseTimer() {
        timeElapsed += Time.deltaTime;
    }

    private void InitializeManagers() {
        _fxManager = FindObjectOfType<FXManager>();
        _bulletJobManager = FindObjectOfType<BulletJobManager>();
        
        if (!_bulletJobManager) {
            Debug.LogError("BulletJobManager not found in the scene.");
            return;
        }
        _bulletJobManager.Register(this);
    }

    protected void PlaySound() {

        if (_isPlayed) {
            return;
        }
        _fxManager.PlaySound(
            impactEffectClip, 
            transform.position, 
            0.25f
        );
        _isPlayed = true;
    }
    
    protected void PlayEffect(Quaternion rotation) {
        _fxManager.PlayEffect(
            impactEffectPrefab, 
            transform.position, 
            rotation
        );
    }

    #endregion
}
