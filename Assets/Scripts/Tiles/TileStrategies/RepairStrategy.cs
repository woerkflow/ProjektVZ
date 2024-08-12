public class RepairStrategy : ITileStrategy {

    public bool CanInteract(Tile tile)
        => tile.tileObjectBuilding.GetHealth() < tile.tileObjectBuilding.maxHealth 
            && tile.playerManager.HasEnoughResources(Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding));

    public void Interact(Tile tile) {
        tile.playerManager.SubtractResources(Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding));
        tile.tileObjectBuilding.SetHealth(tile.tileObjectBuilding.maxHealth);
    }
}