using UnityEngine;

public class BuildManager : MonoBehaviour {
    
    public static BuildManager Instance;
    
    [Header("Building Blueprints")]
    public Building flameLauncher;
    public Building bladeLauncher;
    public Building pumpkinSpawner;
    public Building fenceObstacle;

    [Header("Menu")] 
    public UIMenu buildMenu;
    
    private Building _selectedBuilding;
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
        buildMenu.Deactivate();
    }

    #endregion
    
    
    #region Public class methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
        buildMenu.Activate();
    }
    
    #endregion
    
    
    #region Private class methods
    
    private bool canBuild => _selectedBuilding != null;
    
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

        if (canBuild) {
            _selectedTile.Build(_selectedBuilding);
            buildMenu.Deactivate();
        }
    }

    public void Close() {
        buildMenu.Deactivate();
        _selectedTile = null;
        _selectedBuilding = null;
    }
    
    #endregion
}