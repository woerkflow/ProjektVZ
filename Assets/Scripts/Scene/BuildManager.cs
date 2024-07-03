using UnityEngine;

public class BuildManager : MonoBehaviour {
    
    public static BuildManager Instance;
    public BuildingBlueprint flameLauncher;
    public BuildingBlueprint bladeLauncher;
    public BuildingBlueprint pumpkinSpawner;
    public BuildingBlueprint fenceObstacle;
    
    private BuildingBlueprint _selectedBuilding;
    private Tile _selectedTile;
    
    
    #region Unity methods
    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("More than one BuildManager at once;");
        } else {
            Instance = this;
        }
    }
    
    #endregion
    
    
    #region Public class methods
    
    public void SelectTile(Tile tile) {
        _selectedTile = tile;
    }
    
    #endregion
    
    
    #region Private class methods
    
    private bool canBuild => _selectedBuilding != null;
    
    private void SelectBuildingToBuild(BuildingBlueprint building) {
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
            Close();
        }
    }

    public void Close() {
        _selectedTile.CloseMenu();
        _selectedTile = null;
        _selectedBuilding = null;
    }
    
    #endregion
}