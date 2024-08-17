public class UpgradeStrategy : ITileInteractionStrategy {

    public bool CanInteract(Tile tile)
        => tile.tileObject.blueprint.buildingUpgradePrefab 
            && tile.enemySpawner.state.GetType().ToString() == "BuildState"
            && tile.playerManager.HasEnoughResources(tile.tileObject.blueprint.buildingUpgradePrefab.blueprint.resources);

    public void Interact(Tile tile) {
        
        // Manage Resources
        Resources buildCosts = tile.tileObject.blueprint.buildingUpgradePrefab.blueprint.resources;
        Resources repairCosts = Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding);
        tile.playerManager.SubtractResources(buildCosts);
        tile.AddResources(new Resources {
            wood = buildCosts.wood - repairCosts.wood,
            waste = buildCosts.waste - repairCosts.waste,
            whiskey = buildCosts.whiskey - repairCosts.whiskey
        });
        
        // Manage Effects & Sounds
        tile.PlayEffect(tile.replaceEffect);
        tile.PlaySound(tile.buildAudioClip);
        
        // Manage Tile Object
        tile.objectRotation = tile.tileObject.transform.rotation;
        tile.DestroyObject();
        TileObject upgradeBuilding = tile.tileObject.blueprint.buildingUpgradePrefab.GetComponent<TileObject>();
        tile.ReplaceObject(upgradeBuilding);
    }
}