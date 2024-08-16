public class RepairStrategy : ITileStrategy {

    public bool CanInteract(Tile tile)
        => tile.tileObjectBuilding.GetHealth() < tile.tileObjectBuilding.maxHealth 
            && tile.playerManager.HasEnoughResources(Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding));

    public void Interact(Tile tile) {
        Resources repairCosts = Tile.GetRepairCosts(tile.tileObject, tile.tileObjectBuilding);
        tile.playerManager.SubtractResources(repairCosts);
        tile.tileObjectBuilding.SetHealth(tile.tileObjectBuilding.maxHealth);
    }
}