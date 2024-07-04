using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    
    public float spawnRange;
    
    #region Unity Mehtods
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
    
    #endregion
}