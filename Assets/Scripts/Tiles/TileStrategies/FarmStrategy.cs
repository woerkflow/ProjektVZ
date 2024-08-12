public class FarmStrategy : ITileStrategy {
    
    public bool CanInteract(Tile tile)   
        => tile.enemySpawner.state == RoundState.Build
           && tile.enemySpawner.GetTime() >= tile.tileObject.blueprint.timeCosts 
           && tile.HasResources(tile.tileObject.blueprint.resources);
    
    public void Interact(Tile tile) {
        tile.playerManager.AddResources(tile.resources);
        tile.ClearResources();
        tile.enemySpawner.SetTimer(tile.enemySpawner.GetTime() - tile.tileObject.blueprint.timeCosts);
        tile.tileObject.DestroyObject();
    }
}