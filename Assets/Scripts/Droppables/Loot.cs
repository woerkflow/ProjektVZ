using UnityEngine;

public class Loot : MonoBehaviour {
    
    [SerializeField] private Resources resources;

    private PlayerManager _playerManager;
    
    
    #region Unity Methods
    
    private void Start() {
        _playerManager = FindObjectOfType<PlayerManager>();
    }
    
    #endregion
    
    
    #region Ray Interaction Methods
    
    public void OnRayEnter() {
        _playerManager.AddResources(resources);
        Destroy(gameObject);
    }
    
    #endregion
}