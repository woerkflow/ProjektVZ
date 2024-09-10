using UnityEngine;

public class Bullet : MonoBehaviour, ILaunchable {

    [SerializeField] protected BulletBlueprint blueprint;
    
    public float timeElapsed { get; protected set; }
    public Vector3 start { get; private set; }
    public Vector3 end { get; private set; }
    public float travelTime { get; private set; }
    
    private GameObject _target;
    private JobSystemManager _jobSystemManager;
    private FXManager _fxManager;
    
    
    #region Unity Methods

    private void OnDestroy() {
        _jobSystemManager?.UnregisterBullet(this);
    }

    #endregion
    

    #region Public Methods

    public void Launch(Transform firePoint, GameObject target) {
        start = firePoint.position;
        end = new Vector3(
            target.transform.position.x, 
            target.transform.position.y + blueprint.impactHeight,
            target.transform.position.z
        );
        travelTime = Moveable.GetTravelTime(start, end);
    }

    #endregion
    
    
    #region Private Methods

    protected void IncreaseTimer() {
        timeElapsed += Time.deltaTime;
    }

    protected void InitializeManagers() {
        _fxManager = FindObjectOfType<FXManager>();
        _jobSystemManager = FindObjectOfType<JobSystemManager>();
        _jobSystemManager?.RegisterBullet(this);
    }

    protected void PlaySound() {
        _fxManager.PlaySound(
            blueprint.impactEffectClip,
            transform.position,
            0.25f
        );
    }
    
    protected void PlayEffect(Quaternion rotation) {
        _fxManager.PlayEffect(
            blueprint.impactEffectPrefab,
            transform.position,
            rotation
        );
    }

    #endregion
}
