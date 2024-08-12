public class DestroyStrategy : ITileStrategy {

    public bool CanInteract(Tile tile)
        => tile.tileObject;

    public void Interact(Tile tile) {
        tile.tileObject.DestroyObject();
    }
}