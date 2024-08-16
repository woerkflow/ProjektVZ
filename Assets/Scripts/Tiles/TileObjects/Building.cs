using UnityEngine;

public class Building : MonoBehaviour {

    public int maxHealth;
    public int currentHealth { get; private set; }

    
    #region Unity Methods

    private void Start() {
        currentHealth = maxHealth;
    }
    
    #endregion
    
    
    #region Public Class Methods

    public void SetHealth(int value) {
        currentHealth = value;

        if (currentHealth > 0) {
            return;
        }
        Break();
    }

    public int GetHealth() {
        return currentHealth;
    }
    
    public void TakeDamage(int value) {
        SetHealth(GetHealth() - value);
    }
    
    #endregion
    
    
    #region Private Class Methods
    
    private void Break() {
        TileObject tileObject = GetComponent<TileObject>();

        if (!tileObject) {
            return;
        }
        tileObject.parentTile.DestroyObject();
    }
    
    #endregion
}