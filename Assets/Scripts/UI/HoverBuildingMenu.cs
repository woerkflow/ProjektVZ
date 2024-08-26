using TMPro;
using UnityEngine;

public class HoverBuildingMenu : UIMenu {

    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text value;

    private TileObjectType _tileType;

    
    #region Public Class Methods
    
    public void SetValue(Tile tile) {
        Building building = tile.tileObjectBuilding;
        SetStringValue(label, "Health:");
        SetStringValue(value,building.currentHealth + "/" + building.GetMaxHealth());
    }
    
    #endregion
}