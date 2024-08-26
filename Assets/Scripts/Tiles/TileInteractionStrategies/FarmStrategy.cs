public class FarmStrategy : ITileInteractionStrategy {

    public bool CanInteract(Tile tile)
        => tile.enemySpawner.state.GetType().ToString() == "BuildState"
           && tile.enemySpawner.buildCountDown >= tile.tileObject.GetBluePrint().timeCosts;
    
    public void Interact(Tile tile) {
        
        // Manage Resources
        tile.playerManager.AddResources(tile.tileObject.GetBluePrint().resources);
        
        // Manage Timer
        tile.enemySpawner.SetTimer(tile.enemySpawner.buildCountDown - tile.tileObject.GetBluePrint().timeCosts);
        tile.objectRotation = tile.tileObject.transform.rotation;
        
        // Manage Effects & Sounds
        tile.PlayEffect(tile.GetReplaceEffect());
        tile.PlaySound(tile.GetFarmAudioClip());
        
        // Manage Tile Object
        tile.DestroyObject();
    }
}