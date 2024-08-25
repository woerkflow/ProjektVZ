using UnityEngine;

public class Loot : MonoBehaviour {
    
    public Resources resources;
    
    public PlayerManager playerManager { get; set; }
    
    
    #region Unity Methods
    
    private void Start() {
        playerManager = FindObjectOfType<PlayerManager>();
    }
    
    public void OnRayEnter() {
        playerManager.AddResources(resources);
        Destroy(gameObject);
    }
    
    #endregion
}