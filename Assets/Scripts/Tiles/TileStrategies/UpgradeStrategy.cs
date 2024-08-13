public class UpgradeStrategy : ITileStrategy {

    public bool CanInteract(Tile tile)
        => tile.tileObject.blueprint.buildingUpgradePrefab 
            && tile.enemySpawner.state.GetType().ToString() == "BuildState" 
            && tile.playerManager.HasEnoughResources(tile.tileObject.blueprint.buildingUpgradePrefab.blueprint.resources);

    public void Interact(Tile tile) {
        Resources costs = tile.tileObject.blueprint.buildingUpgradePrefab.blueprint.resources;
        TileObject upgradeBuilding = tile.tileObject.blueprint.buildingUpgradePrefab.GetComponent<TileObject>();
        tile.playerManager.SubtractResources(costs);
        tile.AddResources(costs);
        tile.ReplaceObject(upgradeBuilding);
    }
}