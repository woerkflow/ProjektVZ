using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {
    
    public static BuildManager Instance;
    
    [Header("Prefabs")]
    public Building flameLauncher;
    public Building bladeLauncher;
    public Building pumpkinSpawner;
    public Building chickenSpawner;
    public Building fenceObstacle;
    public Building toxicWasteBeamer;

    [Header("Menu")]
    public UIMenu buildMenu;
    
    private readonly List<GameObject> _buildings = new();
    private Building _selectedBuilding;
    private PlayerManager _playerManager;
    private Tile _selectedTile;

    
    #region Unity Methods

    private void Awake() {
        if (Instance) {
            Debug.LogWarning("More than one BuildManager instance found!");
            return;
        }
        Instance = this;
    }

    private void Start() {
        _playerManager = PlayerManager.Instance;
        buildMenu.Deactivate();
    }

    #endregion

    
    #region Public Methods

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

    public List<GameObject> GetBuildings() => _buildings;

    public void AddBuilding(GameObject building) {
        _buildings.Add(building);
    }

    public void RemoveBuilding(GameObject building) {
        _buildings.Remove(building);
    }

    #endregion
    
    
    #region Menu Button Methods

    public void SelectFlameLauncher() {
        SelectBuildingToBuild(flameLauncher);
    }
    
    public void SelectBladeLauncher() {
        SelectBuildingToBuild(bladeLauncher);
    }
    
    public void SelectPumpkinSpawner() {
        SelectBuildingToBuild(pumpkinSpawner);
    }
    
    public void SelectTntChickenSpawner() {
        SelectBuildingToBuild(chickenSpawner);
    }
    
    public void SelectFenceObstacle() {
        SelectBuildingToBuild(fenceObstacle);
    }

    public void SelectToxicWasteBeamer() {
        SelectBuildingToBuild(toxicWasteBeamer);
    }

    public void Build() {
        
        if (!CanBuild()) {
            return;
        }
        Resources cost = new Resources {
            wood = _selectedBuilding.blueprint.resourceWood,
            waste = _selectedBuilding.blueprint.resourceWaste,
            whiskey = _selectedBuilding.blueprint.resourceWhiskey
        };
        DeductResources(cost);
        
        _selectedTile.AddResources( 
            new Resources {
                wood = cost.wood,
                waste = cost.waste,
                whiskey = cost.whiskey
            }
        );
        _selectedTile.Build(_selectedBuilding);
        CloseMenu();
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
    
    
    #region Private Methods

    private bool CanBuild() {
        
        if (!_selectedBuilding) {
            return false;
        }
        return _playerManager.HasEnoughResources( 
            new Resources {
                wood = _selectedBuilding.blueprint.resourceWood,
                waste = _selectedBuilding.blueprint.resourceWaste,
                whiskey = _selectedBuilding.blueprint.resourceWhiskey
            }
        );
    }

    private void DeductResources(Resources cost) {
        _playerManager.SubtractResources(cost);
    }

    #endregion
}