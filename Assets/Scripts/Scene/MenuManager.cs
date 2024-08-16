using System;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    [Header("Menus")] 
    public UIMenu buildMenu;
    public UIMenu farmMenu;
    public UIMenu upgradeMenu;

    private UIMenu _currentMenu;
    
    
    #region Unity Methods

    private void Start() {
        CloseMenus();
    }
    
    #endregion
    
    
    #region Public Methods

    public void OpenMenu(TileObjectType type, Tile tile) {
        _currentMenu = GetCurrentMenu(type);
        
        switch (type) {
            case TileObjectType.Ruin:
            case TileObjectType.Empty:
                _currentMenu.GetComponent<BuildMenu>().SelectTile(tile);
                break;
            case TileObjectType.Resource:
                _currentMenu.GetComponent<FarmMenu>().SelectTile(tile);
                break;
            case TileObjectType.Building:
                _currentMenu.GetComponent<UpgradeMenu>().SelectTile(tile);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _currentMenu.Activate();
    }
    
    public void CloseMenus() {

        if (buildMenu.IsActive()) {
            buildMenu.Deactivate();
        }
        
        if (farmMenu.IsActive()) {
            farmMenu.Deactivate();
        }
        
        if (upgradeMenu.IsActive()) {
            upgradeMenu.Deactivate();
        }
    }
    
    #endregion
    
    
    #region Private Methods

    private UIMenu GetCurrentMenu(TileObjectType type) 
        => type switch {
            TileObjectType.Building => upgradeMenu,
            TileObjectType.Empty => buildMenu,
            TileObjectType.Resource => farmMenu,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    
    #endregion
}
