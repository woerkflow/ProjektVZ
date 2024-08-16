public class DestroyStrategy : ITileStrategy {

    public bool CanInteract(Tile tile)
        => tile.tileObject;

    public void Interact(Tile tile) {
        Resources gain = tile.resources;
        Resources costs = Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding);
        
        tile.playerManager.AddResources(
            new Resources {
                wood = gain.wood - costs.wood,
                waste = gain.waste - costs.waste,
                whiskey = gain.whiskey - costs.whiskey
            }
        );
        tile.ClearResources();
        tile.tileObject.DestroyObject();
    }
}