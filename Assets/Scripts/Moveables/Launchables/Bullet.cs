using UnityEngine;

public class Bullet : MonoBehaviour, ILaunchable {
    
    [Header("Common")]
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
    public AudioClip impactEffectClip;
    
    public FXManager fxManager { get; set; }

    private bool _isPlayed;
    
    
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
        Destroy(gameObject);
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
        fxManager = FindObjectOfType<FXManager>();
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
        fxManager.PlaySound(
            impactEffectClip, 
            transform.position, 
            0.25f
        );
        _isPlayed = true;
    }
    
    protected void PlayEffect(Quaternion rotation) {
        fxManager.PlayEffect(
            impactEffectPrefab, 
            transform.position, 
            rotation
        );
    }

    #endregion
}
