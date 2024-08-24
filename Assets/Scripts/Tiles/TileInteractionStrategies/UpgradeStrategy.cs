public class UpgradeStrategy : ITileInteractionStrategy {
    
    private Resources _buildCosts;
    private Resources _repairCosts;

    public bool CanInteract(Tile tile) {
        _buildCosts = tile.tileObject.blueprint.buildingUpgradePrefab.blueprint.resources;
        _repairCosts = Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding);
        
        return tile.tileObject.blueprint.buildingUpgradePrefab 
               && tile.enemySpawner.state.GetType().ToString() == "BuildState" 
               && tile.playerManager.HasEnoughResources( 
                   new Resources {
                       wood = _buildCosts.wood + _repairCosts.wood,
                       waste = _buildCosts.waste + _repairCosts.waste,
                       whiskey = _buildCosts.whiskey + _repairCosts.whiskey
                   }
               );
    }
    
    public void Interact(Tile tile) {
        
        // Manage Resources
        tile.playerManager.SubtractResources( 
            new Resources {
                wood = _buildCosts.wood + _repairCosts.wood,
                waste = _buildCosts.waste + _repairCosts.waste,
                whiskey = _buildCosts.whiskey + _repairCosts.whiskey
            }
        );
        
        // Manage Effects & Sounds
        tile.PlayEffect(tile.replaceEffect);
        tile.PlaySound(tile.upgradeAudioClip);
        
        // Manage Tile Object
        TileObject upgradeBuilding = tile.tileObject.blueprint.buildingUpgradePrefab.GetComponent<TileObject>();
        tile.objectRotation = tile.tileObject.transform.rotation;
        tile.DestroyObject();
        tile.ReplaceObject(upgradeBuilding);
    }
}