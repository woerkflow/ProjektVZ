public class FarmStrategy : ITileInteractionStrategy {
    
    public bool CanInteract(Tile tile)   
        => tile.enemySpawner.state.GetType().ToString() == "BuildState"
           && tile.enemySpawner.buildCountDown >= tile.tileObject.blueprint.timeCosts 
           && tile.HasResources(tile.tileObject.blueprint.resources);
    
    public void Interact(Tile tile) {
        
        // Manage Resources
        tile.playerManager.AddResources(tile.resources);
        tile.ClearResources();
        
        // Manage Timer
        tile.enemySpawner.SetTimer(tile.enemySpawner.buildCountDown - tile.tileObject.blueprint.timeCosts);
        tile.objectRotation = tile.tileObject.transform.rotation;
        
        // Manage Tile Object
        tile.StartEffect();
        tile.DestroyObject();
    }
}