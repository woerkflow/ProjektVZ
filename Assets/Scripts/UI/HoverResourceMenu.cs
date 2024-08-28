using TMPro;
using UnityEngine;

public class HoverResourceMenu : UIMenu {
    
    [SerializeField] private TMP_Text[] labels;
    [SerializeField] private TMP_Text[] values;
    
    private TileObjectType _tileType;
    
    
    #region Public Class Methods
    
    public void SetValues(Tile tile) {
        Resources tileObjectResources = tile.tileObject.GetBluePrint().resources;
        SetStringValue(labels[0], GetLabelText(tileObjectResources));
        SetStringValue(labels[1], "Time:");
        SetStringValue(values[0], GetValueText(tileObjectResources));
        SetStringValue(values[1], "-" + tile.tileObject.GetBluePrint().timeCosts + "s");
    }
    
    #endregion
    
    
    #region Private Class Methods
    
    private static string GetLabelText(Resources resources) 
        => resources.wood > 0 
            ? "Wood:" 
            : resources.waste > 0 
                ? "Waste:" 
                : "Whiskey:";
    
    private static string GetValueText(Resources resources)
        => resources.wood > 0 
            ? "+" + resources.wood 
            : resources.waste > 0 
                ? "+" + resources.waste 
                : "+" + resources.whiskey;
    
    #endregion
}