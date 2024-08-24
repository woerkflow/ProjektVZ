public class DestroyStrategy : ITileInteractionStrategy {

    public bool CanInteract(Tile tile)
        => tile.tileObject;

    public void Interact(Tile tile) {
        
        // Manage Resources
        Resources gain = tile.tileObject.blueprint.resources;
        Resources costs = Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding);
        tile.playerManager.AddResources(new Resources {
            wood = gain.wood - costs.wood,
            waste = gain.waste - costs.waste,
            whiskey = gain.whiskey - costs.whiskey
        });
        
        // Manage Effects & Sounds
        tile.PlayEffect(tile.replaceEffect);
        tile.PlaySound(tile.destroyAudioClip);
        
        // Manage Tile Object
        tile.DestroyObject();
    }
}