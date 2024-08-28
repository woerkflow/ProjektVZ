using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    
    [SerializeField] private float spawnRange;
    
    #region Unity Methods
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
    
    #endregion
    
    
    #region Public Methods

    public float GetSpawnRange() => spawnRange;

    #endregion
}