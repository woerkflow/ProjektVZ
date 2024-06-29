using UnityEngine;

public class Spawnpoint : MonoBehaviour {
    
    #region Unity Mehtods
    
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.04f);
    }
    
    #endregion
}