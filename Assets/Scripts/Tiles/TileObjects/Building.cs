using UnityEngine;

public class Building : MonoBehaviour {

    public int maxHealth;
    public int currentHealth { get; private set; }

    private bool _isBroken;

    
    #region Unity Methods

    private void Start() {
        currentHealth = maxHealth;
        _isBroken = false;
    }
    
    #endregion
    
    
    #region Public Class Methods

    public void SetHealth(int value) {
        currentHealth = value;

        if (currentHealth > 0 || _isBroken) {
            return;
        }
        _isBroken = true;
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