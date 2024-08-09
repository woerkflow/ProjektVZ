using UnityEngine;

public class Building : TileObject {
    
    [Header("Building")]
    public int maxHealth;
    public Building upgrade;
    
    private int _currentHealth;

    
    #region Unity Methods

    private void Start() {
        _currentHealth = maxHealth;
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetHealth(int value) {
        _currentHealth = value;

        if (_currentHealth <= 0) {
            DestroyObject();
        }
    }

    public int GetHealth() {
        return _currentHealth;
    }
    
    public void TakeDamage(int value) {
        SetHealth(GetHealth() - value);
    }
    
    #endregion
}