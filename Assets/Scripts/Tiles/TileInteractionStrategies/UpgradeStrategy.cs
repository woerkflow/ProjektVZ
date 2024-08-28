public class UpgradeStrategy : ITileInteractionStrategy {
    
    private Resources _buildCosts;
    private Resources _repairCosts;

    public bool CanInteract(Tile tile) {
        _buildCosts = tile.tileObject.GetBluePrint().buildingUpgradePrefab.GetBluePrint().resources;
        _repairCosts = Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding);
        
        return tile.tileObject.GetBluePrint().buildingUpgradePrefab 
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
        tile.PlayEffect(tile.GetReplaceEffect());
        tile.PlaySound(tile.GetUpgradeAudioClip());
        
        // Manage Tile Object
        tile.DestroyObject();
        TileObject upgradeBuilding = tile.tileObject.GetBluePrint().buildingUpgradePrefab.GetComponent<TileObject>();
        tile.ReplaceObject(upgradeBuilding);
    }
}