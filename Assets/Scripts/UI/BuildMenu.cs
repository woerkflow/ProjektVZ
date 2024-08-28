using TMPro;
using UnityEngine;

public class BuildMenu : UIMenu {
    
    [Header("Menu")]
    [SerializeField] private TMP_Text resourceWoodText;
    [SerializeField] private TMP_Text resourceWasteText;
    [SerializeField] private TMP_Text resourceWhiskeyText;
    
    private Tile _selectedTile;
    
    
    #region Button Menu Methods
    
    public void Build(TileObject buildingToBuild) {
        _selectedTile.OnSelect(buildingToBuild);
        
        if (!_selectedTile.PerformInteraction(TileInteractionType.Build)) {
            return;
        }
        Close();
    }

    public void Rotate(float buildingRotation) {
        _selectedTile.RotateObject(buildingRotation);
    }

    public void Close() {
        Deactivate();
        _selectedTile = null;
    }
    
    #endregion
    
    
    #region Public Methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
        Resources playerResources = _selectedTile.playerManager.resources;
        SetIntValue(resourceWoodText, playerResources.wood);
        SetIntValue(resourceWasteText, playerResources.waste);
        SetIntValue(resourceWhiskeyText, playerResources.whiskey);
    }
    
    #endregion
}