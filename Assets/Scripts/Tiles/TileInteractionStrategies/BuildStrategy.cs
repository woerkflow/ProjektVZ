public class BuildStrategy : ITileInteractionStrategy {
    
    public bool CanInteract(Tile tile)
        => tile.selectedBuilding 
           && tile.enemySpawner.state.GetType().ToString() == "BuildState"
           && tile.playerManager.HasEnoughResources(tile.selectedBuilding.GetBluePrint().resources);
    
    public void Interact(Tile tile) {

        // Manage Resources
        Resources buildCosts = tile.selectedBuilding.GetBluePrint().resources;
        tile.playerManager.SubtractResources(buildCosts);
        
        // Manage Effects & Sounds
        tile.PlayEffect(tile.GetReplaceEffect());
        tile.PlaySound(tile.GetBuildAudioClip());
        
        // Manage Tile Object
        Tile.DestroyTileObject(tile.tileObject);
        tile.ReplaceObject(tile.selectedBuilding);
    }
}