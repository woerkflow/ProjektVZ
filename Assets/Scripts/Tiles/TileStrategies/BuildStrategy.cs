public class BuildStrategy : ITileStrategy {
    
    public bool CanInteract(Tile tile)
        => tile.selectedBuilding 
           && tile.enemySpawner.state.GetType().ToString() == "BuildState"
           && tile.playerManager.HasEnoughResources(tile.selectedBuilding.blueprint.resources);
    
    public void Interact(Tile tile) {
        Resources buildCosts = tile.selectedBuilding.blueprint.resources;
        tile.playerManager.SubtractResources(buildCosts);
        tile.AddResources(buildCosts);
        tile.ReplaceObject(tile.selectedBuilding);
    }
}