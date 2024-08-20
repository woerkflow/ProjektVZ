public interface ITileInteractionStrategy {
    
    bool CanInteract(Tile tile);
    void Interact(Tile tile);
}
