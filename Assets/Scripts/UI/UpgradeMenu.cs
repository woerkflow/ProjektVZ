using TMPro;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour {
    
    [Header("Menu")]
    public UIMenu upgradeMenu;
    public TMP_Text buildingHealth;

    private Tile _selectedTile;
    
    #region Public Methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
        Building tileObjectBuilding = _selectedTile.tileObjectBuilding;
        UIMenu.SetStringValue(buildingHealth, tileObjectBuilding.GetHealth() + "/" + tileObjectBuilding.maxHealth);
    }
    
    #endregion

    
    #region Button Menu Methods
    
    public void Upgrade() {
        Interact(TileStrategyType.Upgrade);
    }
    
    public void Repair() {
        Interact(TileStrategyType.Repair);
    }
    
    public void Destroy() {
        Interact(TileStrategyType.Destroy);
    }
    
    public void Close() {
        upgradeMenu.Deactivate();
        _selectedTile = null;
    }
    
    #endregion
    
    
    #region Private Methods
    
    private void Interact(TileStrategyType type) {
        
        if (!_selectedTile.PerformInteraction(type)) {
            return;
        }
        Close();
    }
    
    #endregion
}
