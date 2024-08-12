using TMPro;
using UnityEngine;

public class FarmMenu : MonoBehaviour {
    
    [Header("Menu")]
    public UIMenu farmMenu;
    public TMP_Text resourceWoodAmount;
    public TMP_Text resourceWasteAmount;
    public TMP_Text resourceWhiskeyAmount;
    public TMP_Text timeCosts;
    
    private Tile _selectedTile;
    
    
    #region Menu Management Methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
        Resources tileResources = _selectedTile.resources;
        UIMenu.SetIntValue(resourceWoodAmount, tileResources.wood);
        UIMenu.SetIntValue(resourceWasteAmount, tileResources.waste);
        UIMenu.SetIntValue(resourceWhiskeyAmount, tileResources.whiskey);
        UIMenu.SetIntValue(timeCosts, _selectedTile.tileObject.blueprint.timeCosts);
    }
    
    #endregion
    
    
    #region Menu Button Methods

    public void Farm() {
        
        if (!_selectedTile.PerformInteraction(TileStrategyType.Farm)) {
            return;
        }
        Close();
    }
    
    public void Close() {
        farmMenu.Deactivate();
        _selectedTile = null;
    }
    
    #endregion
}