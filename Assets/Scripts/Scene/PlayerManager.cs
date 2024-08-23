using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
    public Resources resources { get; private set; }
    
    private readonly List<Building> _buildings = new();
    
    
    #region Unity Methods
    
    private void Start() {
        resources = new Resources {
            wood = 0,
            waste = 0,
            whiskey = 0
        };
    }
    
    #endregion
    
    
    #region Resources Management Methods

    public Resources GetResources() => resources;
    
    public void AddResources(Resources resourcesToAdd) {
        resources = new Resources {
            wood = resources.wood + resourcesToAdd.wood,
            waste = resources.waste + resourcesToAdd.waste,
            whiskey = resources.whiskey + resourcesToAdd.whiskey
        };
    }
    
    public void SubtractResources(Resources resourcesToSubtract) {
        resources = new Resources {
            wood = resources.wood - resourcesToSubtract.wood,
            waste = resources.waste - resourcesToSubtract.waste,
            whiskey = resources.whiskey - resourcesToSubtract.whiskey
        };
    }
    
    public bool HasEnoughResources(Resources requiredResources) {
        return resources.wood >= requiredResources.wood &&
               resources.waste >= requiredResources.waste &&
               resources.whiskey >= requiredResources.whiskey;
    }
    
    #endregion
    
    
    #region Buildings Management Methods
    
    public List<Building> GetBuildings() => _buildings;
    
    public void AddBuilding(Building building) {
        _buildings.Add(building);
    }
    
    public void RemoveBuilding(Building building) {
        _buildings.Remove(building);
    }
    
    #endregion
}