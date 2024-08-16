public class DestroyStrategy : ITileInteractionStrategy {

    public bool CanInteract(Tile tile)
        => tile.tileObject;

    public void Interact(Tile tile) {
        
        // Manage Resources
        Resources gain = tile.resources;
        Resources costs = Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding);
        tile.playerManager.AddResources(new Resources {
            wood = gain.wood - costs.wood,
            waste = gain.waste - costs.waste,
            whiskey = gain.whiskey - costs.whiskey
        });
        tile.ClearResources();
        
        // Manage Tile Object
        tile.DestroyObject();
    }
}