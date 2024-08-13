public class BuildStrategy : ITileStrategy {
    
    public bool CanInteract(Tile tile)
        => tile.selectedBuilding 
           && tile.enemySpawner.state.GetType().ToString() == "BuildState"
           && tile.playerManager.HasEnoughResources(tile.selectedBuilding.blueprint.resources);
    
    public void Interact(Tile tile) {
        Resources costs = tile.selectedBuilding.blueprint.resources;
        tile.playerManager.SubtractResources(costs);
        tile.AddResources(costs);
        tile.ReplaceObject(tile.selectedBuilding);
    }
}