public class RepairStrategy : ITileInteractionStrategy {

    public bool CanInteract(Tile tile)
        => tile.tileObjectBuilding.GetHealth() < tile.tileObjectBuilding.maxHealth 
            && tile.playerManager.HasEnoughResources(Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding));

    public void Interact(Tile tile) {
        
        // Manage Resources
        Resources repairCosts = Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding);
        tile.playerManager.SubtractResources(repairCosts);
        
        // Manage Effects & Sounds
        tile.PlayEffect(tile.replaceEffect);
        tile.PlaySound(tile.buildAudioClip);
        
        // Manage Tile Object
        tile.tileObjectBuilding.SetHealth(tile.tileObjectBuilding.maxHealth);
    }
}