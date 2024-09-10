using TMPro;
using UnityEngine;

public class UpgradeMenu : UIMenu {
    
    [Header("Menu")]
    [SerializeField] private TMP_Text buildingHealth;

    private Tile _selectedTile;
    
    
    #region Button Menu Methods
    
    public void Upgrade() {
        Interact(TileInteractionType.Upgrade);
    }
    
    public void Repair() {
        Interact(TileInteractionType.Repair);
    }
    
    public void Destroy() {
        Interact(TileInteractionType.Destroy);
    }
    
    public void Close() {
        Deactivate();
        _selectedTile = null;
    }
    
    #endregion
    
    
    #region Public Methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
        Building tileObjectBuilding = _selectedTile.tileObjectBuilding;
        SetStringValue(buildingHealth, tileObjectBuilding.currentHealth + "/" + tileObjectBuilding.GetMaxHealth());
    }
    
    #endregion
    
    
    #region Private Methods
    
    private void Interact(TileInteractionType type) {
        
        if (!_selectedTile.PerformInteraction(type)) {
            return;
        }
        Close();
    }
    
    #endregion
}
