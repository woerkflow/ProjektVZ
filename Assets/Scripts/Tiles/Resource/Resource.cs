public class Resource : TileObject {
    
    #region Public class methods
    
    public void Destroy() {
        parentTile.ReplaceObject(
            blueprint.ruin.GetComponent<TileObject>()
        );
    }
    
    #endregion
}
