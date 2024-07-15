using UnityEngine;

public class BuildManager : MonoBehaviour {
    
    public static BuildManager Instance;
    
    [Header("Buildings")]
    public Building flameLauncher;
    public Building bladeLauncher;
    public Building pumpkinSpawner;
    public Building fenceObstacle;

    [Header("Menu")]
    public UIMenu buildMenu;
    
    private Building _selectedBuilding;
    private PlayerManager _playerManager;
    private Tile _selectedTile;
    
    
    #region Unity methods
    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("More than one BuildManager at once;");
        } else {
            Instance = this;
        }
    }

    private void Start() {
        _playerManager = PlayerManager.Instance;
        buildMenu.Deactivate();
    }

    #endregion
    
    
    #region Public class methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
        buildMenu.Activate();
    }

    public bool MenuIsActive() {
        return buildMenu.IsActive();
    }
    
    #endregion
    
    
    #region Private class methods

    private bool CanBuild() {
        
        return _selectedBuilding != null
               && _playerManager.GetResourceWood() >= _selectedBuilding.blueprint.resourceWood
               && _playerManager.GetResourceWaste() >= _selectedBuilding.blueprint.resourceWaste
               && _playerManager.GetResourceWhiskey() >= _selectedBuilding.blueprint.resourceWhiskey;
    }
    
    private void SelectBuildingToBuild(Building building) {
        _selectedBuilding = building;
    }
    
    #endregion
    
    
    #region Menu methods
    
    public void SelectFlameLauncher() {
        SelectBuildingToBuild(flameLauncher);
    }
    
    public void SelectBladeLauncher() {
        SelectBuildingToBuild(bladeLauncher);
    }
    
    public void SelectPumpkinSpawner() {
        SelectBuildingToBuild(pumpkinSpawner);
    }
    
    public void SelectFenceObstacle() {
        SelectBuildingToBuild(fenceObstacle);
    }

    public void Build() {

        if (CanBuild()) {

            // Take resources from the player
            _playerManager.SetResourceWood(
                _playerManager.GetResourceWood() - _selectedBuilding.blueprint.resourceWood
            );
            _playerManager.SetResourceWaste(
                _playerManager.GetResourceWaste() - _selectedBuilding.blueprint.resourceWaste
            );
            _playerManager.SetResourceWhiskey(
                _playerManager.GetResourceWhiskey() - _selectedBuilding.blueprint.resourceWhiskey
            );

            // Give resources to the tile
            _selectedTile.resourceWood += _selectedBuilding.blueprint.resourceWood;
            _selectedTile.resourceWaste += _selectedBuilding.blueprint.resourceWaste;
            _selectedTile.resourceWhiskey += _selectedBuilding.blueprint.resourceWhiskey;
            
            // Build building
            _selectedTile.Build(_selectedBuilding);
            
            // Close menu
            Close();
        }
    }

    public void RotateClockwise() {
        _selectedTile.RotateObject(90f);
    }

    public void RotateCounterClockwise() {
        _selectedTile.RotateObject(-90f);
    }

    public void Close() {
        buildMenu.Deactivate();
        _selectedTile = null;
        _selectedBuilding = null;
    }
    
    #endregion
}