using UnityEngine;

public class TileObject : MonoBehaviour {
    
    [Header("Tile Object")]
    public TileObjectBlueprint blueprint;
    
    private Tile _parentTile;

    public void SetParentTile(Tile parent) {
        _parentTile = parent;
    }
    
    public void DestroyObject() {
        _parentTile.ReplaceObject(
            blueprint.ruin.GetComponent<TileObject>()
        );
    }
}