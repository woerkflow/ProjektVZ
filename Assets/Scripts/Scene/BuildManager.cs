using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {
    
    public static BuildManager Instance;
    
    [Header("Prefabs")]
    public Building flameLauncher;
    public Building bladeLauncher;
    public Building pumpkinSpawner;
    public Building fenceObstacle;
    public Building toxicWasteBlaster;

    [Header("Menu")]
    public UIMenu buildMenu;
    
    [HideInInspector]
    public List<GameObject> buildings;
    
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
    }

    public void ActivateMenu() {
        buildMenu.Activate();
    }

    public bool MenuIsActive() {
        return buildMenu.IsActive();
    }
    
    public void SelectBuildingToBuild(Building building) {
        _selectedBuilding = building;
    }

    public void AddBuilding(GameObject building) {
        buildings.Add(building);
    }

    public void RemoveBuilding(GameObject building) {
        buildings.Remove(building);
    }

    public List<GameObject> GetBuildings() {
        return buildings;
    }
    
    #endregion
    
    
    #region Private class methods

    private static bool CanBuild(PlayerManager playerManager, Building selectedBuilding) {
        return selectedBuilding != null
               && playerManager.GetResourceWood() >= selectedBuilding.blueprint.resourceWood
               && playerManager.GetResourceWaste() >= selectedBuilding.blueprint.resourceWaste
               && playerManager.GetResourceWhiskey() >= selectedBuilding.blueprint.resourceWhiskey;
    }
    
    #endregion
    
    
    #region Menu button methods
    
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

    public void SelectToxicWasteBlaster() {
        SelectBuildingToBuild(toxicWasteBlaster);
    }

    public void Build() {

        if (CanBuild(_playerManager, _selectedBuilding)) {

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
            CloseMenu();
        }
    }

    public void RotateClockwise() {
        _selectedTile.RotateObject(90f);
    }

    public void RotateCounterClockwise() {
        _selectedTile.RotateObject(-90f);
    }

    public void CloseMenu() {
        buildMenu.Deactivate();
        _selectedTile = null;
        _selectedBuilding = null;
    }
    
    #endregion
}