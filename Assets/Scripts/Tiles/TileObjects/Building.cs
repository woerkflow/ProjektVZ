using UnityEngine;

public class Building : MonoBehaviour {
    
    public int maxHealth { get; set; }
    public TileObject upgrade { get; set; }
    public int currentHealth { get; private set; }

    
    #region Unity Methods

    private void Start() {
        currentHealth = maxHealth;
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetHealth(int value) {
        currentHealth = value;

        if (currentHealth <= 0) {
            DestroyBuilding();
        }
    }

    public int GetHealth() {
        return currentHealth;
    }
    
    public void TakeDamage(int value) {
        SetHealth(GetHealth() - value);
    }
    
    private void DestroyBuilding() {
        TileObject tileObject = GetComponent<TileObject>();
        
        if (tileObject) {
            tileObject.DestroyObject();
        }
    }
    
    #endregion
}