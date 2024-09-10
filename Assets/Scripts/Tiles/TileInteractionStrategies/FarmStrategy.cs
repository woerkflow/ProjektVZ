public class FarmStrategy : ITileInteractionStrategy {

    public bool CanInteract(Tile tile)
        => tile.enemySpawner.state.GetType().ToString() == "BuildState"
           && tile.enemySpawner.buildCountDown >= tile.tileObject.GetResources().time;
    
    public void Interact(Tile tile) {
        
        // Manage Resources
        tile.playerManager.AddResources(tile.tileObject.GetResources());
        
        // Manage Timer
        tile.enemySpawner.SetTimer(tile.enemySpawner.buildCountDown - tile.tileObject.GetResources().time);
        
        // Manage Effects & Sounds
        tile.PlayEffect(tile.GetReplaceEffect());
        tile.PlaySound(tile.GetFarmAudioClip());
        
        // Manage Tile Object
        tile.DestroyObject();
    }
}