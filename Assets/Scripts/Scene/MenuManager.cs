using System;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    [Header("Static Menus")] 
    [SerializeField] private BuildMenu buildMenu;
    [SerializeField] private UpgradeMenu upgradeMenu;
    
    [Header("Hover Menus")]
    [SerializeField] private HoverBuildingMenu buildingMenu;
    [SerializeField] private HoverResourceMenu resourceMenu;
    [SerializeField] private float menuHeight;
    
    
    #region Unity Methods

    private void Start() {
        CloseMenus();
        CloseHoverMenus();
    }
    
    #endregion
    
    
    #region Public Methods

    public void OpenHoverMenu(Tile tile) {
        
        switch (tile.GetTileObjectType()) {
            case TileObjectType.Building:
                buildingMenu.SetPosition(tile, menuHeight);
                buildingMenu.SetValue(tile);
                buildingMenu.Activate();
                break;
            case TileObjectType.Resource:
                resourceMenu.SetPosition(tile, menuHeight);
                resourceMenu.SetValues(tile);
                resourceMenu.Activate();
                break;
            case TileObjectType.Empty:
            case TileObjectType.Ruin:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OpenMenu(Tile tile) {
        
        switch (tile.GetTileObjectType()) {
            case TileObjectType.Ruin:
            case TileObjectType.Empty:
                buildMenu.SelectTile(tile);
                buildMenu.Activate();
                break;
            case TileObjectType.Building:
                upgradeMenu.SelectTile(tile);
                upgradeMenu.Activate();
                break;
            case TileObjectType.Resource:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void CloseMenus() {

        if (buildMenu.IsActive()) {
            buildMenu.Deactivate();
        }
        
        if (upgradeMenu.IsActive()) {
            upgradeMenu.Deactivate();
        }
    }

    public void CloseHoverMenus() {
        
        if (buildingMenu.IsActive()) {
            buildingMenu.Deactivate();
        }

        if (resourceMenu.IsActive()) {
            resourceMenu.Deactivate();
        }
    }
    
    #endregion
}
