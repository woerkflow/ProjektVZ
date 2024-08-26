using UnityEngine;

public class TileObject : MonoBehaviour {
    
    [Header("Tile Object")]
    [SerializeField] private TileObjectBlueprint blueprint;
    
    private Tile _parentTile;
    
    
    #region Public Methods

    public TileObjectBlueprint GetBluePrint() => blueprint;

    public Tile GetParentTile() => _parentTile;

    public void SetParentTile(Tile tile) {
        _parentTile = tile;
    }

    #endregion
}