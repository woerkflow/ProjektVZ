using TMPro;
using UnityEngine;

public class BuildMenu : MonoBehaviour {
    
    [Header("Menu")]
    public UIMenu buildMenu;
    public TMP_Text resourceWoodText;
    public TMP_Text resourceWasteText;
    public TMP_Text resourceWhiskeyText;
    
    private Tile _selectedTile;
    
    
    #region Public Methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
        Resources playerResources = _selectedTile.GetPlayerResources();
        UIMenu.SetIntValue(resourceWoodText, playerResources.wood);
        UIMenu.SetIntValue(resourceWasteText, playerResources.waste);
        UIMenu.SetIntValue(resourceWhiskeyText, playerResources.whiskey);
    }
    
    #endregion
    
    
    #region Button Methods
    
    public void Select(TileObject buildingToBuild) {
        _selectedTile.OnSelect(buildingToBuild);
    }
    
    public void Build() {
        
        if (!_selectedTile.PerformInteraction(TileInteractionType.Build)) {
            return;
        }
        Select(null);
        Close();
    }

    public void Rotate(float buildingRotation) {
        _selectedTile.RotateObject(buildingRotation);
    }

    public void Close() {
        buildMenu.Deactivate();
        _selectedTile = null;
    }
    
    #endregion
}