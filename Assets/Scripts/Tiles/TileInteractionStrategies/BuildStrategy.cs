public class BuildStrategy : ITileInteractionStrategy {
    
    public bool CanInteract(Tile tile)
        => tile.selectedBuilding 
           && tile.enemySpawner.state.GetType().ToString() == "BuildState"
           && tile.playerManager.HasEnoughResources(tile.selectedBuilding.blueprint.resources);
    
    public void Interact(Tile tile) {

        // Manage Resources
        Resources buildCosts = tile.selectedBuilding.blueprint.resources;
        tile.playerManager.SubtractResources(buildCosts);
        
        // Manage Effects & Sounds
        tile.PlayEffect(tile.replaceEffect);
        tile.PlaySound(tile.buildAudioClip);
        
        // Manage Tile Object
        Tile.DestroyTileObject(tile.tileObject);
        tile.ReplaceObject(tile.selectedBuilding);
    }
}