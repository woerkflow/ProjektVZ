using UnityEngine;

public class Building : TileObject {
    
    [Header("Building")]
    public int maxHealth;
    public TileObject ruin;
    
    // Health
    private int _currentHealth;
    
    // Parent tile
    private Tile _parentTile;
    
    
    #region Unity Methods

    private void Start() {
        _currentHealth = maxHealth;
    }
    
    #endregion
    
    
    #region Public class methods

    public void SetHealth(int value) {
        _currentHealth = value;

        if (_currentHealth <= 0) {
            _parentTile.ReplaceObject(ruin);
        }
    }

    public int GetHealth() {
        return _currentHealth;
    }

    public void SetParentTile(Tile tile) {
        _parentTile = tile;
    }
    
    #endregion
}