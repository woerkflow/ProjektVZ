public class BuildStrategy : ITileInteractionStrategy {
    
    public bool CanInteract(Tile tile)
        => tile.selectedBuilding 
           && tile.enemySpawner.state.GetType().ToString() == "BuildState"
           && tile.playerManager.HasEnoughResources(tile.selectedBuilding.blueprint.resources);
    
    public void Interact(Tile tile) {

        // Manage Resources
        Resources buildCosts = tile.selectedBuilding.blueprint.resources;
        tile.playerManager.SubtractResources(buildCosts);
        tile.AddResources(buildCosts);
        
        // Manage Tile Object
        tile.StartEffect();
        tile.ReplaceObject(tile.selectedBuilding);
    }
}